using Clubhouse.Api.Extensions;
using Clubhouse.Business.Extensions;
using Clubhouse.Data;
using Clubhouse.Data.Extensions;
using Microsoft.AspNetCore.HttpLogging;

string corsPolicyName = "Clubhouse.Api.PolicyName";

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
var config = builder.Configuration;

services.AddBusinessLayer(config)
    .AddDataLayer(config);

services.AddHttpContextAccessor();

services.AddSwaggerGen(config, CommonConstants.AuthScheme.Bearer);

services.AddHealthChecks();

services.AddCors(options => options
    .AddPolicy(corsPolicyName, policy => policy
        .AllowAnyOrigin()
        .AllowAnyHeader()
        .AllowAnyMethod()));

services.AddAppControllers();

services.AddHttpLogging(options =>
{
    options.LoggingFields = HttpLoggingFields.All;
    options.RequestBodyLogLimit = 4096;
    options.ResponseBodyLogLimit = 4096;
});

services.AddApiVersioning(1);

var app = builder.Build();
await app.RunMigrationsAsync();

app.UseActorSystem();

app.UseHttpLogging();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseAppSwagger();

app.UseExceptionHandler(!app.Environment.IsProduction());

app.UseRouting();

app.UseCors(corsPolicyName);

app.UseAuthentication();

app.UseAuthorization();

app.MapHealthChecks("/health");
app.MapControllers();

await app.RunAsync();