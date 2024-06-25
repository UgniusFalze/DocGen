using System.Globalization;
using System.Security.Claims;
using DocsManager.Models;
using DocsManager.Models.Dto;
using DocsManager.Services.IntegerToWordsConverter;
using DocsManager.Services.Invoice;
using DocsManager.Utils.DocsGenerator;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DocsManager.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class PdfDocumentController(
    IPdfGenerator pdfGenerator,
    DocsManagementContext _docsManagementContext,
    IInvoiceService invoiceService,
    IntegerToWordsConverter itwc) : ControllerBase
{
    private Guid? GetUserGuid()
    {
        var user = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (user == null) return null;

        return Guid.Parse(user);
    }

    [HttpGet("downloadFile")]
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