using System.ComponentModel.DataAnnotations.Schema;

namespace DocsManager.Models;

public class InvoiceItem
{
    public int InvoiceItemId { get; set; }

    [ForeignKey("Invoice")] public int InvoiceId { get; set; }

    public Invoice Invoice { get; set; }

    public string Name { get; set; }
    public string UnitOfMeasurement { get; set; }
    public int Units { get; set; }
    public decimal PriceOfUnit { get; set; }
}