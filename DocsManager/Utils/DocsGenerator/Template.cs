using System.Reflection;
using RazorLight;

namespace DocsManager.Utils.DocsGenerator;

public class Template<T>(string docTemplate, T docModel)
{
    private string DocTemplate { get; set; } = docTemplate;
    private T DocModel { get; set; } = docModel;

    public async Task<string> RenderTemplate()
    {
        var engine = new RazorLightEngineBuilder()
            .UseEmbeddedResourcesProject(Assembly.GetEntryAssembly())
            .Build();
        var result = await engine.CompileRenderStringAsync(
            "cacheKey",
            DocTemplate,
            DocModel);
        return result;
    }
}