namespace DocsManager.Models.Dto;

public class InvoiceDto
{
    public int SeriesNumber { get; set; }
    public string Date { get; set; }
    public string Name { get; set; }
    public string Address { get; set; }
    public string PersonalId { get; set; }
    public string FreelanceWorkId { get; set; }
    public string BankNumber { get; set; }
    public string BankName { get; set; }
    public string BuyerName { get; set; }
    public string BuyerAddress { get; set; }
    public string BuyerCode { get; set; }
    public string? VatCode { get; set; }
    public List<ItemDto> Products { get; set; }
    public string SumInWords { get; set; }
    public string NameWithInitials { get; set; }
    public string TotalMoney { get; set; }
}