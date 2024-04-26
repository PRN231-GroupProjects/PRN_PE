using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Repository.Interfaces;
using Repository.Persistence;
using Repository.Repository;

namespace Repository;

public static class ConfigureServices
{
    public static IServiceCollection AddRepositoryServices(this IServiceCollection services)
    {
        services.AddDbContext();
        services.AddScoped<PRN231_SU23_StudentGroupDBContext>();
        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddAutoMapper(Assembly.GetExecutingAssembly());

        return services;
    }
    
    private static IServiceCollection AddDbContext(this IServiceCollection services)
    {
        services.AddDbContext<PRN231_SU23_StudentGroupDBContext>(options => options.UseSqlServer(GetConnectionString()));
        return services;
    }
    
    private static string GetConnectionString()
    {
        var config = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", true, true)
            .Build();
        var strConn = config["ConnectionString:PRN231_SU23_StudentGroupDB"];

        return strConn;
    }
}