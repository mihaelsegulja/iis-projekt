using IISNotionSearch.Domain.Abstractions;
using IISNotionSearch.Repository.Repositories;
using IISNotionSearch.Domain.Interfaces;
using IISNotionSearch.Repository.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace IISNotionSearch.Repository;

public static class DependencyInjection
{
    public static IServiceCollection AddRepository(this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(
                connectionString,
                x => x.MigrationsAssembly("IISNotionSearch.Repository")));

        services.AddScoped(typeof(IBaseRepository<>), typeof(BaseRepository<>));
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
        services.AddScoped<INotionObjectRepository, NotionObjectRepository>();
        
        return services;
    }
}
