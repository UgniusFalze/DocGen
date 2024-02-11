using System.Security.Claims;
using System.Text;
using DocsManager.Models;
using DocsManager.Models.Dto;
using DocsManager.Utils.DocsGenerator;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DocsManager.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PdfDocumentController(IPdfGenerator pdfGenerator, IHtmlGenerator htmlGenerator, DocsManagementContext _docsManagementContext) : ControllerBase
    {
        
        private Guid? GetUserGuid()
        {
            var user = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (user == null)
            {
                return null;
            }

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
                    BuyerName = x.InvoiceClient.BuyerName,
                    Date = x.InvoiceDate,
                    FreelanceWorkId = x.InvoiceUser.FreelanceWorkId,
                    Name = x.InvoiceUser.FirstName +  " " + x.InvoiceUser.LastName,
                    Products = x.Items.Select(x => new ItemDto(x)).ToList()
                    
                })
                .FirstOrDefaultAsync();
            if (invoice == null)
            {
                return NotFound();
            }
            var htmlString = _docsManagementContext.Templates.Where(t => t.TemplateModel == "invoice").ToList();
            var template = htmlGenerator.RenderTemplate(invoice, htmlString.First().HtmlString);
            var pdfs = await template.ContinueWith(templateString => pdfGenerator.GeneratePdf(templateString.Result));
            return await pdfs.ContinueWith(pdf =>
            {
                var result = File(pdf.Result, "application/pdf");
                result.FileDownloadName = "Invoice.pdf";
                return result;
            });
        }
    }
}