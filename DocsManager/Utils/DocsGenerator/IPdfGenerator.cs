using DocsManager.Models.Dto;

namespace DocsManager.Utils.DocsGenerator;

public interface IPdfGenerator
{
    public byte[] GenerateInvoicePdf(InvoiceDto invoiceDto);
}