using Atheneum.Interface;
using Atheneum.Services;
using JeerowayWiki.Images.Middleware;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace JeerowayWiki.Images
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie();

            // services.AddDbContext<ApplicationContext>(options =>
            //     options.UseSqlServer(Configuration.GetConnectionString("AppConnection")));
            //
            // services.AddDbContext<ImagesContext>(options =>
            //     options.UseSqlServer(Configuration.GetConnectionString("ImgConnection")));

            services.AddTransient<RolesValidation>();
            services.AddTransient<AuthService>();
            services.AddTransient<IUsersService, UsersService>();
            services.AddTransient<IAlbumsService, AlbumsService>();
            services.AddTransient<IImgService, ImgService>();

            //services.AddControllers();
            //services.AddMvc().AddRazorOptions(options => options.AllowRecompilingViewsOnFileChange = true); ;
            services.AddControllersWithViews();
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
            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseAuthorization();

            //app.UseEndpoints(endpoints =>
            //{
            //    endpoints.MapControllers();
            //});
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "img",
                    pattern: "Img/View/{albumId}/{id}",
                    defaults: new { controller = "Img", action = "View" });

                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}",
                    defaults: new { controller="Home", action = "Index" });

                endpoints.MapControllers();
                //endpoints.MapRazorPages();
            });
        }
    }
}
