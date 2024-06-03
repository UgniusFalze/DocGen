using System.Globalization;

namespace DocsManager.Models.Dto;

public class ItemDto
{
    public ItemDto(InvoiceItem invoiceItem)
    {
        InvoiceItemId = invoiceItem.InvoiceItemId;
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

    public string PriceOfUnitMoney => PriceOfUnit.ToString("N2", CultureInfo.CreateSpecificCulture("lt-LT"));
    public decimal TotalPrice => Units * PriceOfUnit;

    public string TotalMoney => TotalPrice.ToString("N2", CultureInfo.CreateSpecificCulture("lt-LT"));
}