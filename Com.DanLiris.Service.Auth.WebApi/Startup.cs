using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Com.DanLiris.Service.Auth.Lib.Services;
using Com.DanLiris.Service.Auth.Lib;
using Microsoft.AspNetCore.Mvc;
using IdentityServer4.Validation;
using IdentityServer4.AccessTokenValidation;
using IdentityModel;
using Com.DanLiris.Service.Auth.Lib.Authentication;
using IdentityServer4.Services;
using System;

namespace Com.DanLiris.Service.Auth.WebApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            string connectionString = Configuration.GetConnectionString("DefaultConnection") ?? Configuration["DefaultConnection"];
            string authority = Configuration.GetConnectionString("Authority") ?? Configuration["Authority"];
            string clientId = Configuration.GetConnectionString("ClientId") ?? Configuration["ClientId"];
            string secret = Configuration.GetConnectionString("Secret") ?? Configuration["Secret"];
            string env = Configuration.GetConnectionString("ASPNETCORE_ENVIRONMENT") ?? Configuration["ASPNETCORE_ENVIRONMENT"];

            Config.Authority = authority;
            Config.Secret = secret;
            Config.ClientId = clientId;

            services
                .AddDbContext<AuthDbContext>(options => options.UseSqlServer(connectionString))
                .AddTransient<AccountService>()
                .AddTransient<AccountRoleService>()
                .AddTransient<PermissionService>()
                .AddTransient<RoleService>()
                .AddTransient<IProfileService, ProfileService>()
                .AddTransient<IResourceOwnerPasswordValidator, ResourceOwnerPasswordValidator>();

            services
               .AddApiVersioning(options =>
               {
                   options.ReportApiVersions = true;
                   options.AssumeDefaultVersionWhenUnspecified = true;
                   options.DefaultApiVersion = new ApiVersion(1, 1);
               });

            if (env.Equals("Test"))
            {
                services.AddIdentityServer()
                    .AddDeveloperSigningCredential()
                    .AddInMemoryIdentityResources(Config.GetIdentityResources())
                    .AddInMemoryApiResources(Config.GetApiResources())
                    .AddInMemoryClients(Config.GetClients())
                    .AddTestUsers(Config.GetTestUsers());
            }
            else
            {
                services.AddIdentityServer()
                    .AddDeveloperSigningCredential()
                    .AddInMemoryIdentityResources(Config.GetIdentityResources())
                    .AddInMemoryApiResources(Config.GetApiResources())
                    .AddInMemoryClients(Config.GetClients())
                    .AddProfileService<ProfileService>();
            }

            services.AddAuthentication(IdentityServerAuthenticationDefaults.AuthenticationScheme)
                .AddIdentityServerAuthentication(options =>
                {
                    options.ApiName = "com.danliris.service";
                    options.ApiSecret = secret;
                    options.Authority = authority;
                    options.RequireHttpsMetadata = false;
                    options.NameClaimType = JwtClaimTypes.Name;
                });

            services.AddCors(o => o.AddPolicy("AuthPolicy", builder =>
            {
                builder.WithOrigins("http://localhost:9000")
                       .WithMethods("GET", "POST", "PUT", "DELETE", "OPTIONS")
                       .AllowAnyHeader();
            }));

            services
                .AddMvcCore()
                .AddAuthorization()
                .AddJsonFormatters();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            using (var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetService<AuthDbContext>();
                context.Database.Migrate();
            }

            app.UseCors("AuthPolicy");
            app.UseIdentityServer();
            app.UseAuthentication();
            app.UseMvc();
        }
    }
}
