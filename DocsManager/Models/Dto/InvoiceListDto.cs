namespace DocsManager.Models.Dto;

public record InvoiceListDto
{
    public InvoiceListDto(int invoiceId, DateTime invoiceDate, string clientName)
    {
        InvoiceId = invoiceId;
        InvoiceDate = invoiceDate;
        ClientName = clientName;
    }


    public int InvoiceId { get; }
    public DateTime InvoiceDate { get; }
    public string ClientName { get; }
}