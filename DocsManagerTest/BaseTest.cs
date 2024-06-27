using System.Text;
using DocGenLibaryTest.factories;
using DocsManager.Models;
using Microsoft.CodeAnalysis.Elfie.Serialization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Primitives;
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
        await DbContext.Database.ExecuteSqlRawAsync(GetTruncateSql("InvoiceItems"));
        await DbContext.Database.ExecuteSqlRawAsync(GetTruncateSql("Invoices"));
        await DbContext.Database.ExecuteSqlRawAsync(GetTruncateSql("Users"));
        await DbContext.Database.ExecuteSqlRawAsync(GetTruncateSql("Clients"));
        await DbContext.Database.ExecuteSqlRawAsync(GetUserPopulateSql());
        await DbContext.Database.ExecuteSqlRawAsync(GetClientPopulateSql());
        await DbContext.Database.ExecuteSqlRawAsync(GetInvoicePopulateSql());
        await DbContext.Database.ExecuteSqlRawAsync(GetInvoiceItemsPopulateSql());
        DbContext.ChangeTracker.Clear();
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
    
    private static string GetInvoiceItemsPopulateSql()
    {
        return GetPopulateSql("dataPopulation/invoicesItemsTest.sql");
    }

    private static string GetTruncateSql(string tableName)
    {
        var result = new StringBuilder()
            .Append("TRUNCATE TABLE \"")
            .Append(tableName)
            .Append("\" RESTART IDENTITY CASCADE")
            .ToString();
        return result;
    }
}