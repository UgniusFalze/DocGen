namespace DocsManager.Models.Dto;

public record InvoicesGridDto(IEnumerable<InvoiceListDto> Invoices, decimal InvoicesTotal);