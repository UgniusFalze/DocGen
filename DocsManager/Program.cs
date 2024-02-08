using System.Security.Claims;
using System.Text;
using DocsManager.Models;
using DocsManager.Utils.DocsGenerator;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(x =>
    {
        x.IncludeErrorDetails = true;
        x.Authority = "http://localhost:8080/realms/DocsManagement";
        x.MetadataAddress =
            "http://localhost:8080/realms/DocsManagement/.well-known/openid-configuration"; 
        x.RequireHttpsMetadata = false;
        x.TokenValidationParameters = new TokenValidationParameters
        {
            RoleClaimType = "groups",
            //NameClaimType = $"{configuration["Keycloak:name_claim"]}",
            //ValidAudience = "DocsManagementReact,account", 
            // https://stackoverflow.com/questions/60306175/bearer-error-invalid-token-error-description-the-issuer-is-invalid
            ValidateIssuer = false,
            ValidateAudience = false
        };
    });

builder.Services.AddAuthorization(o =>
{
    o.DefaultPolicy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();
});

builder.Services.AddCors(options => options.AddPolicy(name: "DocManagerUi",
    policy => { policy.WithOrigins("http://localhost:5173").AllowAnyMethod().AllowAnyHeader(); }));
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddDbContext<DocsManagementContext>(optionsBuilder =>
{
    optionsBuilder.UseNpgsql(
        @"Host=192.168.1.115;Database=docsmanagement;Username=postgres;Password=homeassistant");
});
builder.Services.AddScoped<IHtmlGenerator, Template>();
builder.Services.AddScoped<IPdfGenerator, HtmlToPdf>();
builder.Services.AddControllers();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Projects-admin Swagger", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please provide JWT with bearer (Bearer {jwt token})",
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                },
            },
            new List<string>()
        }
    });
});
var app = builder.Build();

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

app.Run();