namespace DocsManager.Models.Dto;

public class InvoiceUpdatePost
{
    public string InvoiceDate { get; set; }
    public int InvoiceClientId { get; set; }
    public int SeriesNumber { get; set; }
}