using System.Text;
using System.Text.Json.Serialization;
using IISNotionSearch.API.Abstractions.Exceptions;
using IISNotionSearch.API.Extensions;
using IISNotionSearch.API.GraphQL;
using IISNotionSearch.Application;
using IISNotionSearch.Application.Interfaces.Services;
using IISNotionSearch.Infrastructure;
using IISNotionSearch.Infrastructure.Grpc;
using IISNotionSearch.Infrastructure.Services;
using IISNotionSearch.Repository;
using SoapCore;

var builder = WebApplication.CreateBuilder(args);

Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwagger();

builder.Services.AddRepository(builder.Configuration);
builder.Services.AddApplication(builder.Configuration);
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddNotionServiceSwitch(builder.Configuration);

builder.Services.AddGrpc();
var grpcAddress = builder.Configuration["Grpc:WeatherServiceUrl"];
builder.Services.AddGrpcClient<WeatherService.WeatherServiceClient>(options =>
{
    options.Address = new Uri(grpcAddress!);
});

builder.Services.AddJwtAuthentication(builder.Configuration);

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    });
builder.Services.Configure<RouteOptions>(options =>
{
    options.LowercaseUrls = true;
});

builder.Services.AddSoapCore();
builder.Services.AddScoped<INotionSoapService, NotionSoapService>();

builder.Services.AddHttpContextAccessor();

builder.Services
    .AddGraphQLServer()
    .AddQueryType<NotionQuery>()
    .AddMutationType<NotionMutation>()
    .AddAuthorization();

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

var app = builder.Build();

app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    
    app.ApplyMigrations();
    await app.SeedDataAsync();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapGraphQL();
app.MapGrpcService<DhmzGrpcService>();
app.UseSoapEndpoint<INotionSoapService>("/Soap/NotionService.asmx", new SoapEncoderOptions());
app.Run();
