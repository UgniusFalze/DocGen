using System.Reflection;
using System.Text.Json.Serialization;
using DocsManager.Models;
using DocsManager.Services.Client;
using DocsManager.Services.IntegerToWordsConverter;
using DocsManager.Services.Invoice;
using DocsManager.Services.User;
using DocsManager.Utils.DocsGenerator;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Npgsql;
using QuestPDF;
using QuestPDF.Infrastructure;
using Serilog;

namespace DocsManager.Utils;

public class DocManagerAppBuilder
{
    private readonly WebApplicationBuilder _applicationBuilder;

    private readonly ConfigurationManager _applicationConfig;

    public DocManagerAppBuilder(string[] args)
    {
        _applicationBuilder = WebApplication.CreateBuilder(args);
        _applicationConfig = _applicationBuilder.Configuration;
    }

    public DocManagerAppBuilder ConfigureQuestPdf()
    {
        Settings.License = LicenseType.Community;
        return this;
    }

    public DocManagerAppBuilder ConfigureLogging()
    {
        _applicationBuilder.Host.UseSerilog((context, loggerConfiguration) =>
            loggerConfiguration.ReadFrom.Configuration(_applicationConfig));
        return this;
    }

    public DocManagerAppBuilder ConfigureAuth()
    {
        _applicationBuilder.Services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(x =>
            {
                x.IncludeErrorDetails = true;
                x.Authority = _applicationConfig["auth:authorityServer"];
                x.MetadataAddress = _applicationConfig["auth:authorityServer"] + "/.well-known/openid-configuration";
                x.Audience = "api";
                x.RequireHttpsMetadata = false;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    RoleClaimType = "groups",
                    ValidateIssuer = true,
                    ValidateLifetime = true,
                    ValidIssuer = _applicationConfig["auth:authorityServer"],
                    ValidateAudience = true,
                    ValidAudience = _applicationConfig["auth:validAudience"]
                };
            });

        _applicationBuilder.Services.AddAuthorization(o =>
        {
            o.DefaultPolicy = new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .Build();
        });

        return this;
    }

    public DocManagerAppBuilder ConfigureCors()
    {
        _applicationBuilder.Services.AddCors(options => options.AddPolicy("DocManagerUi",
            policy =>
            {
                policy.WithOrigins(_applicationConfig["frontend:server"]).AllowAnyMethod().AllowAnyHeader();
            }));
        return this;
    }

    public DocManagerAppBuilder ConfigureEndpoints()
    {
        _applicationBuilder.Services.AddEndpointsApiExplorer();
        return this;
    }

    public DocManagerAppBuilder ConfigureDbContext()
    {
        var dbstring = _applicationConfig["dbstring"];
        var databaseUrl = Environment.GetEnvironmentVariable("DATABASE_URL");
        if (databaseUrl != null)
        {
            var databaseUri = new Uri(databaseUrl);
            var userInfo = databaseUri.UserInfo.Split(':');
            var builder = new NpgsqlConnectionStringBuilder
            {
                Host = databaseUri.Host,
                Port = databaseUri.Port,
                Username = userInfo[0],
                Password = userInfo[1],
                Database = databaseUri.LocalPath.TrimStart('/'),
                SslMode = SslMode.Allow
            };

            dbstring = builder.ToString();
        }

        _applicationBuilder.Services.AddDbContext<DocsManagementContext>(optionsBuilder =>
        {
            optionsBuilder.UseNpgsql(dbstring);
        });

        return this;
    }

    public DocManagerAppBuilder ConfigureInternalServices()
    {
        _applicationBuilder.Services.AddScoped<IPdfGenerator, PdfGenerator>();
        _applicationBuilder.Services.AddScoped<IntegerToWordsConverter, LithuanianIntegerToWords>();
        _applicationBuilder.Services.AddScoped<IClientService, ClientService>();
        _applicationBuilder.Services.AddScoped<IInvoiceService, InvoiceService>();
        _applicationBuilder.Services.AddScoped<IUserService, UserService>();
        return this;
    }

    public DocManagerAppBuilder ConfigureControllers()
    {
        _applicationBuilder.Services.AddControllers().AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        });
        _applicationBuilder.Services.AddSwaggerGen(c =>
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
            var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
        });
        return this;
    }

    public WebApplication Build()
    {
        return _applicationBuilder.Build();
    }
}