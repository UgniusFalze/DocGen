using DocsManager.Models;
using DocsManager.Models.Dto;
using Microsoft.EntityFrameworkCore;

namespace DocsManager.Services.Client;

public class ClientService(DocsManagementContext context) : IClientService
{
    private const int ClientPageSize = 10;

    public async Task<IEnumerable<Models.Client>> GetClients(int page, string? search)
    {
        page *= ClientPageSize;
        IQueryable<Models.Client> clients = context.Clients;
        if (!string.IsNullOrWhiteSpace(search))
        {
            clients = clients.Where(x => x.BuyerName.ToLower().Contains(search.ToLower()));
        };
        clients = clients.OrderBy(client => client.ClientId).Skip(page).Take(ClientPageSize);
        return await clients.ToListAsync();
    }

    public async Task<IEnumerable<ClientDTO>> GetSelectableClients()
    {
        var clients = context.Clients.Select(client => new ClientDTO(client.ClientId, client.BuyerName));
        return await clients.ToListAsync();
    }

    public async Task<Models.Client?> GetClient(int clientId)
    {
        var client = await context.Clients.FindAsync(clientId);
        return client;
    }

    public async Task<Models.Client> InsertClient(Models.Client client)
    {
        context.Clients.Add(client);
        await context.SaveChangesAsync();
        return client;
    }

    public async Task<bool> UpdateClient(Models.Client client)
    {
        context.Entry(client).State = EntityState.Modified;

        try
        {
            await context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!ClientExists(client.ClientId))
                return false;
            throw;
        }

        return true;
    }

    public async Task<ClientDeleteResult> DeleteClient(int id, Guid userId)
    {
        var client = await context.Clients.FindAsync(id);
        if (client == null) return ClientDeleteResult.NoClient;
        var hasNonUserInvoices = await context.Invoices.AnyAsync(invoice => invoice.InvoiceClientId == id && invoice.InvoiceUserId != userId);
        if (hasNonUserInvoices) return ClientDeleteResult.HasNonUserInvoices;
        
        
        context.Clients.Remove(client);
        await context.SaveChangesAsync();

        return ClientDeleteResult.Success;
    }

    public bool ClientExists(int id)
    {
        return context.Clients.Any(e => e.ClientId == id);
    }
    
    public async Task<int> GetClientCount()
    {
        return await context.Clients.CountAsync();
    }

}