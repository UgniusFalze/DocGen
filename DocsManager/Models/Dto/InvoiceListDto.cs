namespace DocsManager.Models.Dto;

public record InvoiceListDto(int InvoiceId, DateTime InvoiceDate, string ClientName, decimal TotalSum);