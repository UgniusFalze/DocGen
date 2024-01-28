using DocsManager.Utils.DocsGenerator;

namespace DocGenLibaryTest;

public readonly struct Model(string name)
{
    public string Name { get; init; } = name;
}

public class Tests
{

    [SetUp]
    public void Setup()
    {
    }

    [Test(ExpectedResult = "<div>Hi Mark</div>")]
    public async Task<string> Test_Correctly_Renders_Html_With_Variables()
    {
        var testModel = new Model("Mark");
        var template = new Template();
        var result = await template.RenderTemplate(testModel,"<div>Hi @Model.Name</div>");
        return result;
    }
}