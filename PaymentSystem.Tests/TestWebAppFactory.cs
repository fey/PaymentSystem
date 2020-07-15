using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PaymentSystem.Database;
using System;
using System.Linq;

namespace PaymentSystem.Tests
{
    public class TestWebAppFactory<TStartup>: WebApplicationFactory<TStartup> where TStartup: class
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
                {
                    var paynebtsDbDescriptor = services.SingleOrDefault(
                        d => d.ServiceType ==
                            typeof(DbContextOptions<PaymentContext>)
                    );
                    if (paynebtsDbDescriptor != null)
                        services.Remove(paynebtsDbDescriptor);
                    var userDbDescriptor = services.SingleOrDefault(
                        d => d.ServiceType ==
                            typeof(DbContextOptions<UserContext>)
                    );
                    if (userDbDescriptor != null)
                        services.Remove(userDbDescriptor);

                    Guid startupId = Guid.NewGuid();
                    services.AddDbContext<PaymentContext>(
                        builder => builder
                                    .UseLazyLoadingProxies()
                                    .UseInMemoryDatabase($"Payments{startupId}")
                    );

                    services.AddDbContext<UserContext>(
                        builder => builder
                                    .UseInMemoryDatabase($"Users{startupId}")
                    );
                }
            );
        }
    }
}