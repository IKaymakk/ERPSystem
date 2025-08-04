using ERPSystem.Core.Interfaces;
using ERPSystem.Core.MappingProfiles;
using ERPSystem.Infrastructure.DBContext;
using ERPSystem.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ERPSystem.Infrastructure.Registration;

public static class InfrastructureServiceRegistration
{
    public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<ErpDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

        services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
        services.AddScoped(typeof(IUserRepository), typeof(UserRepository));
        services.AddScoped(typeof(IRoleRepository), typeof(RoleRepository));
        services.AddScoped(typeof(ICategoryRepository), typeof(CategoryRepository));

        services.AddAutoMapper(cfg =>
        {
            cfg.AddMaps(typeof(UserMappingProfile).Assembly); 
        });
        return services;
    }
}