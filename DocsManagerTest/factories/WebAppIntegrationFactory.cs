using DocsManager.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Testcontainers.PostgreSql;

namespace DocGenLibaryTest.factories;

public class WebAppIntegrationFactory:WebApplicationFactory<Program>
{
    private readonly PostgreSqlContainer _postgreSqlContainer = new PostgreSqlBuilder()
        .WithImage("postgres:15-alpine")
        .Build();

    public async Task Initialize()
    {
        await _postgreSqlContainer.StartAsync();
    }
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(services =>
        {
            var descriptorType =
                typeof(DbContextOptions<DocsManagementContext>);

            var descriptor = services
                .SingleOrDefault(s => s.ServiceType == descriptorType);

            if (descriptor is not null)
            {
                services.Remove(descriptor);
            }

            services.AddDbContext<DocsManagementContext>(options =>
                options
                    .EnableSensitiveDataLogging()
                    .UseNpgsql(_postgreSqlContainer.GetConnectionString()));
        });
        
    }

    public async Task TearDown()
    {
        await _postgreSqlContainer.StopAsync();
        await _postgreSqlContainer.DisposeAsync();
        await DisposeAsync();
    }
}