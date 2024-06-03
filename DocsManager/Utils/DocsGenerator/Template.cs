using System.Reflection;
using RazorLight;

namespace DocsManager.Utils.DocsGenerator;

public class Template : IHtmlGenerator
{
    public async Task<string> RenderTemplate<T>(T docModel, string docTemplate)
    {
        var engine = new RazorLightEngineBuilder()
            .UseEmbeddedResourcesProject(Assembly.GetEntryAssembly())
            .Build();
        var result = await engine.CompileRenderStringAsync(
            "cacheKey",
            docTemplate,
            docModel);
        return result;
    }
}