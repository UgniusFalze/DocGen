using DocsManager.Models;
using DocsManager.Utils.DocsGenerator;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<DocsManagementContext>(optionsBuilder =>
{
    optionsBuilder.UseNpgsql(@"Host=192.168.1.115;Database=docsmanagement;Username=postgres;Password=homeassistant");
});
builder.Services.AddScoped<IHtmlGenerator, Template>();

builder.Services.AddScoped<IPdfGenerator, HtmlToPdf>();
builder.Services.AddControllers();
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapControllers();
app.Run();
