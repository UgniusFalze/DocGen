using Newtonsoft.Json;

namespace DocsManager.Models.Dto;

public class ItemDto
{
    public ItemDto(InvoiceItem invoiceItem)
    {
        InvoiceItemId = invoiceItem.InvoiceId;
        Name = invoiceItem.Name;
        UnitOfMeasurement = invoiceItem.UnitOfMeasurement;
        Units = invoiceItem.Units;
        PriceOfUnit = invoiceItem.PriceOfUnit;
    }

    public int InvoiceItemId { get; set; }
    public string Name { get; set; }
    public string UnitOfMeasurement { get; set; }
    public int Units { get; set; }
    public decimal PriceOfUnit { get; set; }
    public decimal TotalPrice => Units * PriceOfUnit;
}