using System.Globalization;
using System.Security.Claims;
using DocsManager.Models;
using DocsManager.Models.Dto;
using DocsManager.Services.IntegerToWordsConverter;
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
        var invoice = await _docsManagementContext.Invoices
            .Where(x => x.InvoiceUserId == userId)
            .Where(x => x.SeriesNumber == invoiceId)
            .Select(x => new InvoiceDto
            {
                SeriesNumber = x.SeriesNumber,
                Address = x.InvoiceUser.Address,
                BankName = x.InvoiceUser.BankName,
                BankNumber = x.InvoiceUser.BankNumber,
                BuyerAddress = x.InvoiceClient.BuyerAddress,
                BuyerCode = x.InvoiceClient.BuyerCode,
                VatCode = x.InvoiceClient.VatCode,
                BuyerName = x.InvoiceClient.BuyerName,
                Date = x.InvoiceDate.ToString("yyyy MM dd"),
                FreelanceWorkId = x.InvoiceUser.FreelanceWorkId,
                Name = x.InvoiceUser.FirstName + " " + x.InvoiceUser.LastName,
                Products = x.Items.Select(x => new ItemDto(x)).ToList()
            })
            .FirstOrDefaultAsync();
        if (invoice == null) return NotFound();
        var totalCost = invoice.Products.Sum(product => product.TotalPrice);
        invoice.TotalMoney = totalCost.ToString("N2", CultureInfo.CreateSpecificCulture("lt-LT"));
        invoice.SumInWords = itwc.ConvertSumToWords(totalCost);
        var pdf = pdfGenerator.GenerateInvoicePdf(invoice);
        var result = File(pdf, "application/pdf");
        result.FileDownloadName = "Invoice.pdf";
        return result;
    }
}