namespace DocsManager.Models.Dto;

public class ClientGridDto
{
    public ClientGridDto(int clientId, string buyerName, string buyerAddress, string buyerCode, string? vatCode)
    {
        ClientId = clientId;
        BuyerName = buyerName;
        BuyerAddress = buyerAddress;
        BuyerCode = buyerCode;
        VatCode = vatCode;
    }

    public int ClientId { get; }
    public string BuyerName { get; }
    public string BuyerAddress { get; }
    public string BuyerCode { get; }
    public string? VatCode { get; }
}