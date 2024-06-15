using DocsManager.Models;
using DocsManager.Utils;
using Microsoft.EntityFrameworkCore;

var docManagerAppBuilder = new DocManagerAppBuilder(args)
    .ConfigureQuestPdf()
    .ConfigureLogging()
    .ConfigureAuth()
    .ConfigureEndpoints()
    .ConfigureDbContext()
    .ConfigureInternalServices()
    .ConfigureControllers();

var app = docManagerAppBuilder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    var context = services.GetRequiredService<DocsManagementContext>();
    if (context.Database.GetPendingMigrations().Any()) context.Database.Migrate();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.UseCors("DocManagerUi");
app.MapControllers();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseExceptionHandler("/error");

app.Run();