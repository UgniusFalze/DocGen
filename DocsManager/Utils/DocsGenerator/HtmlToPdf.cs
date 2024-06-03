using PuppeteerSharp;
using PuppeteerSharp.Media;

namespace DocsManager.Utils.DocsGenerator;

public class HtmlToPdf : IPdfGenerator
{
    public async Task<byte[]> GeneratePdf(string html)
    {
        using var browserFetcher = new BrowserFetcher();
        await browserFetcher.DownloadAsync();
        var browser = await Puppeteer.LaunchAsync(new LaunchOptions
        {
            Headless = true, Args = ["--no-sandbox"]
        });
        await using var page = await browser.NewPageAsync();
        await page.SetContentAsync(html);
        var options = new PdfOptions
        {
            Format = PaperFormat.A4
        };
        return await page.PdfDataAsync(options);
    }
}