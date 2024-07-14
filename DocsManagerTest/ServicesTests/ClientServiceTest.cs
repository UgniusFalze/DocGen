using DocsManager.Models;
using DocsManager.Models.Dto;
using DocsManager.Services.Client;

namespace DocGenLibaryTest.ServicesTests;

public class ClientServiceTest : BaseTest
{
    [Test]
    [NonParallelizable]
    public async Task Test_Correctly_Returns_Gets_Paged_Clients()
    {
        var clientService = GetService();
        var client = await clientService.GetClients(0, null);
        Assert.That(client.Count(), Is.EqualTo(10));
    }

    [Test]
    [NonParallelizable]
    [TestCase("Pan", "Panam")]
    [TestCase("che", "Nitzsche?")]
    public async Task Test_Correctly_Filters_Clients(string search, string fullName)
    {
        var clientService = GetService();
        var client = await clientService.GetClients(0, search);
        Assert.That(client.First().BuyerName, Is.EqualTo(fullName));
    }

    [NonParallelizable]
    [TestCase(1, "NameOaa")]
    [TestCase(7, "Au/ra")]
    public async Task Test_Correctly_Returns_Selectable_Clients(int clientId, string clientName)
    {
        var clientService = GetService();
        var selectableClients = await clientService.GetSelectableClients();
        Assert.That(selectableClients.Contains(new ClientDTO(clientId, clientName)), Is.EqualTo(true));
    }

    [NonParallelizable]
    [TestCase(3, "40cf7e27-5de2-4251-b030-9e0335803c58", ClientDeleteResult.Success)]
    [TestCase(88, "40cf7e27-5de2-4251-b030-9e0335803c58", ClientDeleteResult.NoClient)]
    [TestCase(4, "e255553a-1ce4-4f32-9c56-276b24096a4e", ClientDeleteResult.HasNonUserInvoices)]
    public async Task Test_Correctly_Deletes_Client(int id, string userId, ClientDeleteResult result)
    {
        var guid = Guid.Parse(userId);
        var clientService = GetService();
        var deleteResult = await clientService.DeleteClient(id, guid);
        Assert.That(deleteResult, Is.EqualTo(result));
    }
    
    [NonParallelizable]
    [Test]
    public async Task Test_Inserts_Client() {
        var clientService = GetService();
        var clientInsert = new Client(){BuyerAddress = "123 Main Street", BuyerCode = "9999", BuyerName = "John Doe"};
        var insertResult = await clientService.InsertClient(clientInsert);
        Assert.That(insertResult, Is.Not.Null);
    }
    
    [NonParallelizable]
    [Test]
    public async Task Test_Returns_Null_On_Duplication() {
        var clientService = GetService();
        var clientInsert = new Client(){BuyerAddress = "123 Main Street", BuyerCode = "557", BuyerName = "John Doe"};
        var insertResult = await clientService.InsertClient(clientInsert);
        Assert.That(insertResult, Is.Null);
    }

    private ClientService GetService()
    {
        ;
        return new ClientService(DbContext);
    }
}