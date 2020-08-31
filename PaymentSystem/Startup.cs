using System;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Npgsql;
using PaymentSystem.Database;
using PaymentSystem.Services.Implementations;
using PaymentSystem.Services.Interfaces;

namespace PaymentSystem
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
            services.AddTransient<INotifier, StubNotifier>();
            services.AddScoped<IPaymentRepository, DbPaymentRepository>();
            services.AddTransient<IPassowrdHasher, BCryptPasswordHasher>();
            services.AddScoped<IUserRepository, DbUserRepository>();
            services.AddTransient<ICardValidator, CardValidator>();

            Func<NpgsqlConnectionStringBuilder> builderFactory = () => {
                var csBuilder = new NpgsqlConnectionStringBuilder();
                    csBuilder.MaxPoolSize = 10;
                    csBuilder.SslMode = SslMode.Require;
                    csBuilder.TrustServerCertificate = true;

                    csBuilder.Host = "ec2-54-75-244-161.eu-west-1.compute.amazonaws.com";
                    csBuilder.Port = 5432;
                    csBuilder.Database = "dfbncsesv2csih";
                    csBuilder.Username = "midyteqgkntodz";
                    csBuilder.Password = "d94919d747edb6d596b5145264e3396a5a034f706e1be49dc1c6da83b6447872";
                return csBuilder;
            };

            services.AddMvcCore()
                .AddApiExplorer();
            services.AddDbContext<PaymentContext>(
                builder => builder
                            .UseLazyLoadingProxies()
                            .UseNpgsql(builderFactory().ToString())
            );

            services.AddDbContext<UserContext>(
                builder => builder
                            .UseLazyLoadingProxies()
                            .UseNpgsql(builderFactory().ToString())
            );
            
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Payment system API", Version = "v1" });
            });

            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options => {
                    options.LoginPath = "/api/user/login";
                    options.LogoutPath = "/api/user/logout";
                });
            services.AddControllers();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseDeveloperExceptionPage();
            app.UseExceptionHandler("/Error");
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseHttpsRedirection();

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Payment system API v1");
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
