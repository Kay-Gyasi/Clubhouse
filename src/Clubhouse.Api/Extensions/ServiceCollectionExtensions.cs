using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using Clubhouse.Api.Options;
using Clubhouse.Business.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.OpenApi.Models;

namespace Clubhouse.Api.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSwaggerGen(
        this IServiceCollection services,
        IConfiguration configuration,
        string SchemeName)
    {
        services.Configure<ApiDocsConfig>(c => configuration.GetSection(nameof(ApiDocsConfig)).Bind(c));

        services.ConfigureOptions<ConfigureSwaggerOptions>();

        var projName = Assembly.GetExecutingAssembly().GetName().Name;

        services.AddSwaggerGen(c =>
        {
            c.EnableAnnotations();

            c.AddSecurityDefinition(SchemeName, new()
            {
                Description = $@"Enter '[schemeName]' [space] and then your token in the text input below.<br/>
                      Example: '{SchemeName} 12345abcdef'",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.ApiKey,
                Scheme = SchemeName
            });

            c.AddSecurityRequirement(new()
            {
                {
                    new ()
                    {
                        Reference = new ()
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = SchemeName
                        },
                        Scheme = "oauth2",
                        Name = SchemeName,
                        In = ParameterLocation.Header,
                    },
                    Array.Empty<string>()
                }
            });

            c.DocumentFilter<AdditionalParametersDocumentFilter>();

            c.ResolveConflictingActions(descriptions => descriptions.First());

            var xmlFile = $"{projName}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            c.IncludeXmlComments(xmlPath);
        });

        return services;
    }

    public static IServiceCollection AddApiVersioning(this IServiceCollection services, int version)
    {
        services.AddApiVersioning(opt =>
        {
            opt.DefaultApiVersion = new ApiVersion(version, 0);
            opt.AssumeDefaultVersionWhenUnspecified = true;
            opt.ReportApiVersions = true;
            opt.ApiVersionReader = ApiVersionReader.Combine(
                new UrlSegmentApiVersionReader(),
                new HeaderApiVersionReader("x-api-version"),
                new MediaTypeApiVersionReader("x-api-version"));
        });

        services.AddVersionedApiExplorer(setup =>
        {
            setup.GroupNameFormat = "'v'VVV";
            setup.SubstituteApiVersionInUrl = true;
        });

        return services;
    }


    public static IServiceCollection AddAppControllers(this IServiceCollection services)
    {
        services.Configure<RouteOptions>(options => options.LowercaseUrls = true);

        services.AddControllers(options =>
            {
                options.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true;
            })
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
                options.JsonSerializerOptions.WriteIndented = true;
                options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
            })
            .ConfigureApiBehaviorOptions(options =>
            {
                options.InvalidModelStateResponseFactory = InvalidModelStateHandler;
            });

        return services;

        static IActionResult InvalidModelStateHandler(ActionContext context)
        {
            return new BadRequestObjectResult(new ApiResponse<object>(
                Message: "Validation Errors",
                Code: 400,
                Errors: context.ModelState
                    .Where(modelError => modelError.Value?.Errors?.Count > 0)
                    .Select(modelError => new ErrorResponse(
                        Field: modelError.Key,
                        ErrorMessage: modelError.Value?.Errors?.FirstOrDefault()?.ErrorMessage ?? "Invalid Request"))));
        }

    }
}