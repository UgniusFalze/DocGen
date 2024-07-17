using DocsManager.Models.Dto;

namespace DocsManager.Services.DocsGenerator;

public interface IPdfGenerator
{
    public byte[] GenerateInvoicePdf(InvoiceDto invoiceDto);
}