namespace DocsManager.Models;

public class Client
{
    public int ClientId { get; set; }
    public string BuyerName { get; set; }
    public string BuyerAddress { get; set; }
    public string BuyerCode { get; set; }
    
    public string? VatCode { get; set; }
}