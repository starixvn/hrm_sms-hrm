namespace HRM.UserControl.WebAPI
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Fanex.Data;
    using Fanex.Data.SqlServer;
    using HRM.UserControl.API.Core;
    using HRM.UserControl.API.Core.Models;
    using HRM.UserControl.Core.Extensions;
    using HRM.UserControl.WebAPI.Middlewares;
    using Microsoft.AspNetCore.Authentication.JwtBearer;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.HttpOverrides;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Microsoft.IdentityModel.Tokens;

    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            Dictionary<string, ConnectionConfiguration> connections = Configuration
                .GetSection("ConnectionStrings")
                .GetChildren()
                .ToDictionary(connection => connection.Key,
                              connection => new ConnectionConfiguration(connection.Key, connection.Value));

            DbSettingProviderManager
               .StartNewSession()
               .Use(connections)
               .WithSqlServer()
               .UseDefaultDbSettingProvider(resourcePath: $"~/App_Data/SpNames.xml")
               .Run();

            services.Configure<ForwardedHeadersOptions>(options =>
            {
                options.ForwardedHeaders =
                    ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
            });

            services.AddControllers();
            services.Configure<AuthenticationSettings>(options => Configuration.GetSection("AuthenticationSettings"));
            services.AddSingleton(Configuration.GetSection("AuthenticationSettings").Get<AuthenticationSettings>());
            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(x =>
            {
                byte[] secretKey = Encoding.ASCII.GetBytes(Configuration.GetSection("AuthenticationSettings:SecretKey").Value);
                x.RequireHttpsMetadata = false;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(secretKey),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
            });

            services.InjectCoreDI();
            services.AddTransient<IHttpContextAccessor, HttpContextAccessor>();

            services.RegisterDI();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseCors(x => x
            .SetIsOriginAllowed(origin => true)
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials());

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseMiddleware<SiteValidationMiddleware>();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}