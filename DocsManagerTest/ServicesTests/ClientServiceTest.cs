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
        var client = await clientService.GetClients(0);
        Assert.That(client.Count(), Is.EqualTo(10));
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
    [TestCase(3,"40cf7e27-5de2-4251-b030-9e0335803c58", ClientDeleteResult.Success)]
    [TestCase(88, "40cf7e27-5de2-4251-b030-9e0335803c58", ClientDeleteResult.NoClient)]
    [TestCase(4, "e255553a-1ce4-4f32-9c56-276b24096a4e", ClientDeleteResult.HasNonUserInvoices)]
    public async Task Test_Correctly_Deletes_Client(int id, string userId, ClientDeleteResult result)
    {
        var guid = Guid.Parse(userId);
        var clientService = GetService();
        var deleteResult = await clientService.DeleteClient(id, guid);
        Assert.That(deleteResult, Is.EqualTo(result));
    }

    private ClientService GetService()
    {;
        return new ClientService(DbContext);
    }
}