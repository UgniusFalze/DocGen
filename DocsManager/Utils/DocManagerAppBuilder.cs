using DocsManager.Models;
using DocsManager.Services.Client;
using DocsManager.Services.IntegerToWordsConverter;
using DocsManager.Services.Invoice;
using DocsManager.Utils.DocsGenerator;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
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
        QuestPDF.Settings.License = LicenseType.Community;
        return this;
    }

    public DocManagerAppBuilder ConfigureLogging()
    {
        _applicationBuilder.Host.UseSerilog(((context, loggerConfiguration) => loggerConfiguration.ReadFrom.Configuration(_applicationConfig)));
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
                    ValidateIssuerSigningKey = false,
                    SignatureValidator = delegate(string token, TokenValidationParameters parameters)
                    {
                        var jwt = new JsonWebToken(token);

                        return jwt;
                    },
                    ValidateIssuer = true,
                    ValidateLifetime = true,
                    ValidIssuer = _applicationConfig["auth:authorityServer"],
                    ValidateAudience = false
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
            policy => { policy.WithOrigins(_applicationConfig["frontend:server"]).AllowAnyMethod().AllowAnyHeader(); }));
        return this;
    }

    public DocManagerAppBuilder ConfigureEndpoints()
    {
        _applicationBuilder.Services.AddEndpointsApiExplorer();
        return this;
    }

    public DocManagerAppBuilder ConfigureDbContext()
    {
        
        _applicationBuilder.Services.AddDbContext<DocsManagementContext>(optionsBuilder =>
        {
            optionsBuilder.UseNpgsql(_applicationConfig["dbstring"]);
        });

        return this;
    }

    public DocManagerAppBuilder ConfigureInternalServices()
    {
        _applicationBuilder.Services.AddScoped<IPdfGenerator, PdfGenerator>();
        _applicationBuilder.Services.AddScoped<IntegerToWordsConverter, LithuanianIntegerToWords>();
        _applicationBuilder.Services.AddScoped<IClientService, ClientService>();
        _applicationBuilder.Services.AddScoped<IInvoiceService, InvoiceService>();
        return this;
    }

    public DocManagerAppBuilder ConfigureControllers()
    {
        _applicationBuilder.Services.AddControllers();
        _applicationBuilder.Services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo() { Title = "DocsManager Swagger", Version = "v1" });
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
        return this;
    }

    public WebApplication Build()
    {
        return _applicationBuilder.Build();
    }
    
}