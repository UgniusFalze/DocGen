using System.Text;
using DocsManager.Models;
using DocsManager.Models.Dto;
using DocsManager.Utils.DocsGenerator;
using Microsoft.AspNetCore.Mvc;

namespace DocsManager.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PdfDocumentController(IPdfGenerator pdfGenerator, IHtmlGenerator htmlGenerator, DocsManagementContext _docsManagementContext) : ControllerBase
    {
        
        [HttpPost("downloadFile")]
        public async Task<IActionResult> DownloadFile([FromBody] Invoice invoice)
        {
            var htmlString = _docsManagementContext.Templates.Where(t => t.TemplateModel == "invoice").ToList();
            var model = new InvoiceDto(invoice);
            var template = htmlGenerator.RenderTemplate(model, htmlString.First().HtmlString);
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