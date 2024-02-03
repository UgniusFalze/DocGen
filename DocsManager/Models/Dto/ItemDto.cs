namespace DocsManager.Models.Dto;

public class ItemDto
{
    public ItemDto(InvoiceItem invoiceItem)
    {
        Id = invoiceItem.InvoiceId;
        Name = invoiceItem.Name;
        UnitOfMeasurement = invoiceItem.UnitOfMeasurement;
        Units = invoiceItem.Units;
        PriceOfUnit = invoiceItem.PriceOfUnit;
    }

    public int Id { get; }
    public string Name { get; }
    public string UnitOfMeasurement { get; }
    public int Units { get; }
    public decimal PriceOfUnit { get; }
    public decimal TotalPrice => Units * PriceOfUnit;
}