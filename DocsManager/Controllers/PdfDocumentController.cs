using System.Globalization;
using DocsManager.Services.IntegerToWordsConverter;
using DocsManager.Services.Invoice;
using DocsManager.Utils.DocsGenerator;
using Microsoft.AspNetCore.Mvc;

namespace DocsManager.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PdfDocumentController(
    IPdfGenerator pdfGenerator,
    IInvoiceService invoiceService,
    IntegerToWordsConverter itwc) : ControllerWithUser
{

    /// <summary>
    /// Gets pdf generated from invoice
    /// </summary>
    /// <param name="invoiceId">Invoice series number</param>
    /// <returns>Generated pdf</returns>
    /// <response code="200">Returns generated pdf file</response>
    /// <response code="404">If invoice or user is not found</response>
    [HttpGet("downloadFile")]
    [Produces("application/pdf")]
    public async Task<IActionResult> DownloadFile(int invoiceId)
    {
        var userId = GetUserGuid();
        if (userId == null) return NotFound("User not found");
        var invoice = await invoiceService.GetInvoice(invoiceId, userId.Value);
        if (invoice == null) return NotFound("Invoice not found");
        var totalCost = invoice.Products.Sum(product => product.TotalPrice);
        invoice.TotalMoney = totalCost.ToString("N2", CultureInfo.CreateSpecificCulture("lt-LT"));
        invoice.SumInWords = itwc.ConvertSumToWords(totalCost);
        var pdf = pdfGenerator.GenerateInvoicePdf(invoice);
        var result = File(pdf, "application/pdf");
        result.FileDownloadName = "Invoice.pdf";
        return result;
    }
}