using DocsManager.Models.Dto;

namespace DocsManager.Services.Client;

public interface IClientService
{
    public Task<IEnumerable<Models.Client>> GetClients(int page, string? search);

    public Task<IEnumerable<ClientDTO>> GetSelectableClients();

    public Task<Models.Client?> GetClient(int clientId);

    public Task<Models.Client?> InsertClient(Models.Client client);

    public Task<bool> UpdateClient(Models.Client client);

    public Task<ClientDeleteResult> DeleteClient(int id, Guid userId);

    public Task<int> GetClientCount();
}