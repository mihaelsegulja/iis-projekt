using IISNotionSearch.Repository.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace IISNotionSearch.Repository;

public static class DependencyInjection
{
    public static IServiceCollection AddRepository(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(
                configuration.GetConnectionString("Db"), 
                x => x.MigrationsAssembly("IISNotionSearch.Repository")));

        services.AddScoped<IISNotionSearch.Domain.Interfaces.IUserRepository, UserRepository>();

        return services;
    }
}
