using Atheneum.Entity;
using Atheneum.EntityImg;
using Atheneum.Interface;
using Atheneum.Services;
using JeerowayWiki.Images.Middleware;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

namespace JeerowayWiki.Images.Configs;

public static class ServicesExtensions
{
    public static IServiceCollection ConfigureServices(this IServiceCollection services, IConfiguration config)
    {
        services
            .AddAppOptions(config)
            .ConfigureCors()
            .AddAuthentication()
            .AddDbContext(config)
            .AddAppServices()
            .AddControllersWithViews();

        return services;
    }

    private static IServiceCollection AddAppOptions(this IServiceCollection services, IConfiguration config)
    {
        // services.AddOptions<GptOptions>().Bind(config.GetSection("GptOptions"));
        // services.AddOptions<QdrantOptions>().Bind(config.GetSection("QdrantOptions"));

        return services;
    }

    private static IServiceCollection AddAuthentication(this IServiceCollection services)
    {
        
        services
            .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie();

        return services;
    }

    private static IServiceCollection ConfigureCors(this IServiceCollection services)
    {
        services.AddCors(c => c.AddPolicy("AllowSpecificOrigin", corsBuilder =>
        {
            corsBuilder
                .WithOrigins("http://localhost:4200")
                .AllowCredentials()
                .AllowAnyMethod()
                .AllowAnyHeader();
        }));

        return services;
    }
    
    private static IServiceCollection AddDbContext(this IServiceCollection services, IConfiguration config)
    {
        var connString = config.GetConnectionString("AppConnection");
        var connStringImg = config.GetConnectionString("ImgConnection");
        var serverVersion = new MySqlServerVersion(new Version(8, 0, 31));
        
        services.AddDbContext<ApplicationContext>(opt => opt.UseMySql(
            connString!,
            serverVersion,
            x => x.MigrationsAssembly("Atheneum")
        ));
        
        services.AddDbContext<ImagesContext>(opt => opt.UseMySql(
            connStringImg!,
            serverVersion,
            x => x.MigrationsAssembly("Atheneum")
        ));

        return services;
    }
    
    private static IServiceCollection AddAppServices(this IServiceCollection services)
    {
        services.AddTransient<RolesValidation>();
        services.AddTransient<AuthService>();
        services.AddTransient<UsersService>();
        services.AddTransient<AlbumsService>();
        services.AddTransient<ImgService>();
        return services;
    }
}