using PuppeteerSharp;

namespace DocsManager.Utils.DocsGenerator;

public class HtmlToPdf
{
    public static async Task<byte[]> GeneratePdf(string html)
    {
        using var browserFetcher = new BrowserFetcher();
        await browserFetcher.DownloadAsync();
        var browser = await Puppeteer.LaunchAsync(new LaunchOptions
        {
            Headless = true
        });
        using (var page = await browser.NewPageAsync())
        {
            await page.SetContentAsync(html);
            return await page.PdfDataAsync();
        }
    }
}