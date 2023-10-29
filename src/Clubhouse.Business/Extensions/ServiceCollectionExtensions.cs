using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using Akka.Actor;
using Akka.DependencyInjection;
using Clubhouse.Business.Actors;
using Clubhouse.Business.Authentication;
using Clubhouse.Business.Options;
using Clubhouse.Business.Services.Interfaces;
using Clubhouse.Business.Services.Providers;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace Clubhouse.Business.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddBusinessLayer(this IServiceCollection services,
        IConfiguration config)
    {
        services.AddActorSystem(c => config.GetSection(nameof(ActorConfig)))
            .AddBearerAuth(config)
            .AddScoped<IJwtService, JwtService>();

        services.AddScoped<IUserService, UserService>()
            .AddScoped<IInitializationService, InitializationService>();

        services.AddFluentValidationAutoValidation()
            .AddFluentValidationClientsideAdapters()
            .AddValidatorsFromAssembly(typeof(ServiceCollectionExtensions).Assembly);
        return services;
    }

    private static IServiceCollection AddBearerAuth(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<BearerTokenConfig>(configuration.GetSection(nameof(BearerTokenConfig)));
        var bearerConfig = new BearerTokenConfig();
        configuration.GetSection(nameof(BearerTokenConfig)).Bind(bearerConfig);

        services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(x =>
            {
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = bearerConfig.Issuer,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(bearerConfig.SigningKey)),
                    ValidAudience = bearerConfig.Audience,
                    ValidateAudience = true,
                    ValidateLifetime = false,
                    RequireExpirationTime = true,
                    //ValidAlgorithms = new[] {SecurityAlgorithms.HmacSha256Signature},
                    //ClockSkew = TimeSpan.FromMinutes(1)
                };
            });
        return services;
    }


    private static IServiceCollection AddActorSystem(
        this IServiceCollection services,
        Action<ActorConfig> configure)
    {
        services.Configure(configure);
        var actorConfig = new ActorConfig();
        configure.Invoke(actorConfig);

        var actorSystemName = Regex.Replace(Assembly.GetExecutingAssembly().GetName().Name ?? "ActorSystemName", @"[^a-zA-Z\s]+", "");

        services.AddSingleton(sp =>
        {
            var actorSystemSetup = BootstrapSetup
                .Create()
                .And(DependencyResolverSetup
                    .Create(sp));

            var actorSystem = ActorSystem
                .Create(actorSystemName, actorSystemSetup);

            TopLevelActors.RegisterActor<MainActor>(actorSystem);

            //TopLevelActors.RegisterActorWithRouter<SendCallbackActor>(
            //    actorSystem,
            //    actorConfig.SendCallbackActorConfig.NumberOfInstances,
            //    actorConfig.SendCallbackActorConfig.UpperBound);

            return actorSystem;
        });

        return services;
    }
}
