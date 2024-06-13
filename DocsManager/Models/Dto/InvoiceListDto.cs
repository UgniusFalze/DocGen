namespace DocsManager.Models.Dto;

public record InvoiceListDto(int InvoiceId, DateTime InvoiceDate, bool IsPayed, string ClientName, decimal TotalSum);