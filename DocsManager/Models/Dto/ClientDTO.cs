namespace DocsManager.Models.Dto;

public record ClientDTO
{
    public ClientDTO(int clientId, string clientName)
    {
        ClientId = clientId;
        ClientName = clientName;
    }

    public int ClientId { get; }
    public string ClientName { get; }

}