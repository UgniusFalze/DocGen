namespace DocsManager.Utils.DocsGenerator;

public interface IPdfGenerator
{
    public Task<byte[]> GeneratePdf(string html);
}