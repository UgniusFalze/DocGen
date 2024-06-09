using System.Globalization;
using System.Net.Mime;
using DocsManager.Models;
using DocsManager.Services.IntegerToWordsConverter;
using DocsManager.Utils.DocsGenerator;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

Thread.CurrentThread.CurrentCulture = new CultureInfo("lt-LT");
var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;
builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(x =>
    {
        x.IncludeErrorDetails = true;
        x.Authority = configuration["auth:authorityServer"];
        x.MetadataAddress = configuration["auth:authorityServer"] + "/.well-known/openid-configuration";
        x.Audience = "api";
        x.RequireHttpsMetadata = false;
        x.TokenValidationParameters = new TokenValidationParameters
        {
            RoleClaimType = "groups",
            ValidateIssuerSigningKey = false,
            SignatureValidator = delegate(string token, TokenValidationParameters parameters)
            {
                var jwt = new JsonWebToken(token);

                return jwt;
            },
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidIssuer = configuration["auth:authorityServer"],
            ValidAudience = "DocsManagementReact"
        };
    });

builder.Services.AddAuthorization(o =>
{
    o.DefaultPolicy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();
});

builder.Services.AddCors(options => options.AddPolicy("DocManagerUi",
    policy => { policy.WithOrigins(configuration["frontend:server"]).AllowAnyMethod().AllowAnyHeader(); }));
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddDbContext<DocsManagementContext>(optionsBuilder =>
{
    optionsBuilder.UseNpgsql(configuration["dbstring"]);
});
builder.Services.AddScoped<IHtmlGenerator, Template>();
builder.Services.AddScoped<IPdfGenerator, HtmlToPdf>();
builder.Services.AddScoped<IntegerToWordsConverter, LithuanianIntegerToWords>();
builder.Services.AddControllers();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "DocsManager Swagger", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please provide JWT with bearer (Bearer {jwt token})",
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey
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
                }
            },
            new List<string>()
        }
    });
});
var app = builder.Build();

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

app.UseExceptionHandler(exceptionHandlerApp =>
{
    exceptionHandlerApp.Run(async context =>
    {
        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
        
        context.Response.ContentType = MediaTypeNames.Text.Plain;

        await context.Response.WriteAsync("An exception was thrown.");
    });
});

app.Run();