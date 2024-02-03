namespace DocsManager.Models;

public class Invoice
{
    public int InvoiceId { get; set; }
    public string Name { get; set; }
    public string Address { get; set; }
    public string PersonalId { get; set; }
    public string FreelanceWorkId { get; set; }
    public string BankNumber { get; set; }
    public string BankName { get; set; }
    public string BuyerName { get; set; }
    public string BuyerAddress { get; set; }
    public string BuyerCode { get; set; }
    public ICollection<InvoiceItem> Items { get; set; }
}