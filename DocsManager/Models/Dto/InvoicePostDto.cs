namespace DocsManager.Models.Dto;

public class InvoicePostDto
{
    public string InvoiceDate { get; set; }
    public int ClientId { get; set; }
    
    public int SeriesNumber { get; set; }
    
    public List<ItemDto> Items { get; set; }
}