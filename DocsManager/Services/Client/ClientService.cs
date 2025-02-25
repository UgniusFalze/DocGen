using DocsManager.Models;
using DocsManager.Models.Dto;
using FluentResults;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace DocsManager.Services.Client;

public class ClientService(DocsManagementContext context) : IClientService
{
    private const int ClientPageSize = 10;

    public async Task<IEnumerable<Models.Client>> GetClients(int page, string? search)
    {
        page *= ClientPageSize;
        IQueryable<Models.Client> clients = context.Clients;
        if (!string.IsNullOrWhiteSpace(search))
            clients = clients.Where(x => x.BuyerName.ToLower().Contains(search.ToLower()));
        ;
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

    public async Task<Result<Models.Client>> InsertClient(Models.Client client)
    {
        client.VatCode = client.VatCode == "" ? null : client.VatCode;
        context.Clients.Add(client);
        try
        {
            await context.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            if (ex.InnerException is not PostgresException sqlException) throw;
            if (sqlException.SqlState == "23505") return Result.Fail("Client with id already exists");

            throw;
        }

        return client;
    }

    public async Task<bool> UpdateClient(Models.Client client)
    {
        client.VatCode = client.VatCode == "" ? null : client.VatCode;
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
        var hasNonUserInvoices =
            await context.Invoices.AnyAsync(invoice =>
                invoice.InvoiceClientId == id && invoice.InvoiceUserId != userId);
        if (hasNonUserInvoices) return ClientDeleteResult.HasNonUserInvoices;


        context.Clients.Remove(client);
        await context.SaveChangesAsync();

        return ClientDeleteResult.Success;
    }

    public async Task<int> GetClientCount()
    {
        return await context.Clients.CountAsync();
    }

    public bool ClientExists(int id)
    {
        return context.Clients.Any(e => e.ClientId == id);
    }
}