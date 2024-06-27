using DocGenLibaryTest.factories;
using DocsManager.Models;
using Microsoft.CodeAnalysis.Elfie.Serialization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Testcontainers.PostgreSql;

namespace DocGenLibaryTest;
[TestFixture]
public abstract class BaseTest
{
    protected DocsManagementContext DbContext { set; get; }
    private WebAppIntegrationFactory WebAppIntegrationFactory { get; set; }
    [OneTimeSetUp]
    public async Task InitDatabase()
    {
        WebAppIntegrationFactory = new WebAppIntegrationFactory();
        await WebAppIntegrationFactory.Initialize();
        DbContext = WebAppIntegrationFactory.Services.CreateScope().ServiceProvider.GetRequiredService<DocsManagementContext>();
        await DbContext.Database.MigrateAsync();
    }
    [SetUp]
    public async Task PopulateDb()
    {
        await DbContext.Database.ExecuteSqlRawAsync("TRUNCATE TABLE \"Invoices\" RESTART IDENTITY CASCADE");
        await DbContext.Database.ExecuteSqlRawAsync("TRUNCATE TABLE \"Users\" RESTART IDENTITY CASCADE");
        await DbContext.Database.ExecuteSqlRawAsync("TRUNCATE TABLE \"Clients\" RESTART IDENTITY CASCADE");
        await DbContext.Database.ExecuteSqlRawAsync(GetUserPopulateSql());
        await DbContext.Database.ExecuteSqlRawAsync(GetClientPopulateSql());
        await DbContext.Database.ExecuteSqlRawAsync(GetInvoicePopulateSql());
    }
    
    [OneTimeTearDown]
    public async Task TearDown()
    {
        await WebAppIntegrationFactory.TearDown();
    }

    private static string GetPopulateSql(string filePath)
    {
        var baseDirectory = Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).Parent.Parent.Parent.ToString();
        var path = Path.Combine(baseDirectory, filePath);
        StreamReader sr = new StreamReader(path);
        var sql = sr.ReadToEnd();
        return sql;
    }

    private static string GetClientPopulateSql()
    {
        return GetPopulateSql("dataPopulation/clientsTest.sql");
    }
    
    private static string GetUserPopulateSql()
    {
        return GetPopulateSql("dataPopulation/userTest.sql");
    }
    
    private static string GetInvoicePopulateSql()
    {
        return GetPopulateSql("dataPopulation/invoicesTest.sql");
    }
}