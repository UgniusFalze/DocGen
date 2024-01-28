/*
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.Run();
*/

using DocsManager.Utils.DocsGenerator;
using PuppeteerSharp;


var testModel = new Model("Mark");
var template = new Template<Model>("<div>Hi @Model.Name</div>", testModel);
var result = template.RenderTemplate();
result.Wait();
var html = result.Result;

public readonly struct Model(string name)
{
    public string Name { get; init; } = name;
}