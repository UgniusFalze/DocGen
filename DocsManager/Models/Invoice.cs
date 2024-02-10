using System.ComponentModel.DataAnnotations.Schema;
using DocsManager.Models.Dto;

namespace DocsManager.Models;

public class Invoice
{
    public int InvoiceId { get; set; }
    
    public DateTime InvoiceDate { get; set; }
    [ForeignKey("Client")] 
    public int InvoiceClientId { get; set; }
    public Client InvoiceClient { get; set; }
    public ICollection<InvoiceItem> Items { get; set; }
    [ForeignKey("User")] 
    public Guid InvoiceUserId { get; set; }
    public User InvoiceUser { get; set; }

}