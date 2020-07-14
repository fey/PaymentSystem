using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
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

            services.AddMvcCore()
                .AddApiExplorer();

            services.AddDbContext<PaymentContext>(
                builder => builder
                            .UseLazyLoadingProxies()
                            .UseInMemoryDatabase("Payments")
            );

            services.AddDbContext<UserContext>(
                builder => builder
                            .UseInMemoryDatabase("Users")
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
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

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
