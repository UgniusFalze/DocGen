namespace DocsManager.Utils.DocsGenerator;

public interface IHtmlGenerator
{
    public Task<string> RenderTemplate<T>(T docModel, string htmlTemplate);
}