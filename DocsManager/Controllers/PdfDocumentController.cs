using DocsManager.Utils.DocsGenerator;
using Microsoft.AspNetCore.Mvc;

namespace DocsManager.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PdfDocumentController(IPdfGenerator pdfGenerator, IHtmlGenerator htmlGenerator) : ControllerBase
    {
        public readonly struct Model(string name)
        {
            public string Name { get; init; } = name;
        };

        [HttpGet("downloadFile")]
        public async Task<IActionResult> DownloadFile([FromQuery] string name)
        {
            var model = new Model(name);
            var template = htmlGenerator.RenderTemplate(model, "<div>Hi @Model.Name</div>");
            var pdfs = await template.ContinueWith(templateString => pdfGenerator.GeneratePdf(templateString.Result));
            return await pdfs.ContinueWith(pdf => File(pdf.Result, "application/pdf"));
        }
    }
}