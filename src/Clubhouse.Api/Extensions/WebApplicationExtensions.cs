using Clubhouse.Api.Options;
using Clubhouse.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.SwaggerUI;
using System.Net;
using System.Reflection;
using Akka.Actor;
using Clubhouse.Business.Extensions;
using Clubhouse.Business.Models;
using Clubhouse.Data.Extensions;
using Microsoft.EntityFrameworkCore;

namespace Clubhouse.Api.Extensions;

public static class WebApplicationExtensions
{
    public static void UseActorSystem(this WebApplication app)
    {
        var actorSys = app.Services.GetRequiredService<ActorSystem>();

        _ = actorSys ?? throw new ArgumentNullException(nameof(actorSys));
    }

    public static void UseExceptionHandler(
    this WebApplication app,
    bool returnStackTrace = false)
    {
        var logger = app.Services.GetRequiredService<ILogger<Program>>();

        app.UseExceptionHandler(appError =>
        {
            appError.Run(async context =>
            {
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                context.Response.ContentType = "application/json";

                var contextFeature = context.Features.Get<IExceptionHandlerFeature>();

                if (contextFeature is null)
                {
                    return;
                }

                logger.LogError(contextFeature.Error, "Unhadled Exception Occured");

                var errors = Enumerable.Empty<ErrorResponse>();
                if (returnStackTrace)
                {
                    errors = new List<ErrorResponse>()
                    {
                        new (Field: contextFeature.Error?.Message ?? "Exception occured",
                            ErrorMessage: contextFeature.Error?.StackTrace ?? "Exception occured")
                    };
                }

                var response = new ApiResponse<object>(
                    Message: "Ooops, something really bad happened. Please try again later.",
                    Code: 500,
                    Errors: errors);

                var respJson = response.Serialize();

                context.Response.ContentLength = respJson.Length;

                await context.Response.WriteAsync(respJson);

            });
        });
    }

    public static void UseAppSwagger(this WebApplication app)
    {
        var apiDocsConfig = app.Services.GetRequiredService<IOptions<ApiDocsConfig>>().Value;

        var apiVersionDescription = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();

        if (apiDocsConfig.ShowSwaggerUi)
        {
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                var projName = Assembly.GetExecutingAssembly().GetName().Name;
                foreach (var description in apiVersionDescription.ApiVersionDescriptions.Reverse())
                {
                    c.SwaggerEndpoint(
                        $"/swagger/{description.GroupName}/swagger.json",
                        $"{projName} - {description.GroupName}");
                }

                var submitMethods = Array.Empty<SubmitMethod>();

                if (apiDocsConfig.EnableSwaggerTryIt && app.Environment.IsDevelopment())
                {
                    submitMethods = new SubmitMethod[]
                    {
                            SubmitMethod.Post,
                            SubmitMethod.Get,
                            SubmitMethod.Put,
                            SubmitMethod.Patch,
                            SubmitMethod.Delete,
                    };
                }

                c.SupportedSubmitMethods(submitMethods);
            });
        }

        if (apiDocsConfig.ShowRedocUi)
        {
            foreach (var description in apiVersionDescription.ApiVersionDescriptions.Reverse())
            {
                app.UseReDoc(options =>
                {
                    options.DocumentTitle = Assembly.GetExecutingAssembly().GetName().Name;
                    options.RoutePrefix = $"api-docs-{description.GroupName}";
                    options.SpecUrl = $"/swagger/{description.GroupName}/swagger.json";
                });
            }
        }
    }

    public static async Task RunMigrationsAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var logger = app.Services.GetRequiredService<ILogger<Program>>();
        var services = scope.ServiceProvider;
        try
        {
            var storageContext = services.GetRequiredService<ApplicationDbContext>();
            var pendingMigrations = await storageContext.Database.GetPendingMigrationsAsync();
            var count = pendingMigrations.Count();
            if (count > 0)
            {
                logger.LogInformation($"found {count} pending migrations to apply. will proceed to apply them");
                await storageContext.Database.MigrateAsync();
                logger.LogInformation($"done applying pending migrations");
            }
            else
            {
                logger.LogInformation($"no pending migrations found! :)");
            }

            //SeedDb(storageContext)
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while performing migration.");
            throw;
        };
    }

}