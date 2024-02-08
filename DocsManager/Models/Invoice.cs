using System.ComponentModel.DataAnnotations.Schema;

namespace DocsManager.Models;

public class Invoice
{
    public int InvoiceId { get; set; }
    
    public DateTime InvoiceDate { get; set; }
    [ForeignKey("Client")] public Client InvoiceClient { get; set; }
    public ICollection<InvoiceItem> Items { get; set; }
    [ForeignKey("User")] public User InvoiceUser { get; set; }
}