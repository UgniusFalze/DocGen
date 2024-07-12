namespace DocsManager.Models.Dto;

public record ClientDTO
{
    public ClientDTO(int clientId, string clientName)
    {
        Id = clientId;
        Label = clientName;
    }

    public int Id { get; }
    public string Label { get; }
}