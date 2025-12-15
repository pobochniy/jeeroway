using Atheneum.Entity;
using Atheneum.EntityImg;
using Atheneum.Interface;
using Atheneum.Services;
using JeerowayWiki.Images.Configs;
using JeerowayWiki.Images.Middleware;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace JeerowayWiki.Images;

public class Startup(IConfiguration configuration)
{
    private IConfiguration Configuration { get; } = configuration;

    public void ConfigureServices(IServiceCollection services)
    {
        services.ConfigureServices(Configuration);


        //services.AddMvc().AddRazorOptions(options => options.AllowRecompilingViewsOnFileChange = true); ;
        
        // services.AddSwaggerGen(c =>
        // {
        //     c.SwaggerDoc("v1", new OpenApiInfo { Title = "JeerowayWiki.Images", Version = "v1" });
        // });
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        app.UseRouting();

        // app.UseSwagger();
        // app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "JeerowayWiki.Images v1"));

        app.UseStaticFiles();
        
        // Redirect HTTP to HTTPS in production
        if (!env.IsDevelopment())
        {
            app.UseHttpsRedirection();
        }
        
        app.UseAuthentication();
        app.UseAuthorization();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllerRoute(
                name: "img",
                pattern: "Img/View/{albumId}/{id}",
                defaults: new { controller = "Img", action = "View" });

            endpoints.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");
        
            endpoints.MapControllers();
        });
    }
}