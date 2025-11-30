using Atheneum.Entity;
using Atheneum.Interface;
using Atheneum.Services;
using Jeeroway.Api.middleware;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

namespace Jeeroway.Api.configs;

public static class ServicesExtensions
{
    public static IServiceCollection ConfigureServices(this IServiceCollection services, IConfiguration config)
    {
        services
            .AddAppOptions(config)
            .ConfigureCors()
            .AddAuthentication()
            .AddControllersWithSwagger()
            // .RegisterMiddlewares()
            .AddDbContext(config)
            // .AddLocalQueue(config)
            .AddAppServices();

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
            .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, o =>
            {
                o.Cookie.HttpOnly = true;
                o.Events = new CookieAuthenticationEvents
                {
                    OnRedirectToLogin = redirectContext =>
                    {
                        redirectContext.HttpContext.Response.StatusCode = 401;
                        return Task.CompletedTask;
                    }
                };
            });

        return services;
    }

    private static IServiceCollection AddControllersWithSwagger(this IServiceCollection services)
    {
        services
            .AddControllers();
            // .AddJsonOptions(options =>
            // {
            //     options.JsonSerializerOptions.Converters.Add(new DateOnlyJsonConverter());
            //     options.JsonSerializerOptions.Converters.Add(new TimeOnlyJsonConverter());
            // });
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();

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
        var serverVersion = new MySqlServerVersion(new Version(8, 0, 31));
        services.AddDbContext<ApplicationContext>(opt => opt.UseMySql(
            connString!,
            serverVersion,
            x => x.MigrationsAssembly("Atheneum")
        ));

        return services;
    }
    
    // private static IServiceCollection AddLocalQueue(this IServiceCollection services, IConfiguration config)
    // {
    //     var connString = config.GetConnectionString("AppConnection");
    //     services.AddMySqlCommandsStorage((sp, c) =>
    //     {
    //         c.ConnectionString = connString!;
    //         c.TableName = nameof(Atheneum.Entity.LocalQueue);
    //     });
    //     services
    //         .AddLocalCommandQueue()
    //         .AddTransient<EpicCommandHandler>()
    //         .AddLocalCommandQueueWorker(c =>
    //         {
    //             c.PrefetchCount = 10;
    //             c.AddCommandHandler<EpicCommand, EpicCommandHandler>();
    //         });
    //
    //     return services;
    // }
    
    private static IServiceCollection AddAppServices(this IServiceCollection services)
    {
        services.AddTransient<RolesValidation>();
        services.AddTransient<AuthService>();
        services.AddTransient<IChatService, ChatService>();
        services.AddTransient<UsersService>();

        return services;
    }
}