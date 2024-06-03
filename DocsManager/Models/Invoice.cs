using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DocsManager.Models;

[Index(nameof(InvoiceUserId), nameof(SeriesNumber), IsUnique = true)]
public class Invoice
{
    public int InvoiceId { get; set; }

    public DateTime InvoiceDate { get; set; }

    [ForeignKey("Client")] public int InvoiceClientId { get; set; }

    public Client InvoiceClient { get; set; }
    public ICollection<InvoiceItem> Items { get; set; }

    [ForeignKey("User")] public Guid InvoiceUserId { get; set; }

    public User InvoiceUser { get; set; }
    public int SeriesNumber { get; set; }
}