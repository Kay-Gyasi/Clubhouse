﻿using Akka.Actor;
using Akka.DependencyInjection;
using Akka.Routing;
using System.Collections.Concurrent;

namespace Clubhouse.Business.Actors;

public static class TopLevelActors
{
    private static ConcurrentDictionary<string, IActorRef> _actorRegistry = new();
    private static SupervisorStrategy _defaultSupervisorStrategy = null!;

    public static IActorRef GetActor<T>(string name = "") where T : BaseActor
    {
        _ = name ?? throw new ArgumentNullException(nameof(name));

        string actorFullName = GetActorFullName<T>(name);

        if (_actorRegistry.TryGetValue(actorFullName, out var actorInstance))
        {
            return actorInstance;
        }

        throw new ArgumentOutOfRangeException(nameof(actorFullName),
            $"\"{actorFullName}\" not created or registered");
    }

    public static bool RegisterActor<T>(ActorSystem actorSystem, string name = "") where T : BaseActor
    {
        string actorFullName = GetActorFullName<T>(name);

        var actor = CreateNewActor<T>(actorSystem, actorFullName);

        return _actorRegistry.TryAdd(actorFullName, actor);
    }

    public static bool RegisterActorWithRouter<T>(ActorSystem actorSystem, int numberOfInstance, int upperBound, string name = "") where T : BaseActor
    {
        if (numberOfInstance >= upperBound)
            throw new ArgumentOutOfRangeException(nameof(numberOfInstance),
                "numberOfInstance should be >= upperBound");

        string actorFullName = GetActorFullName<T>(name);

        var actor = CreateNewActorWithRouter<T>(actorSystem, numberOfInstance, upperBound, actorFullName);

        return _actorRegistry.TryAdd(actorFullName, actor);
    }

    private static IActorRef CreateNewActor<T>(ActorSystem actorSystem, string name) where T : BaseActor
    {
        return actorSystem.ActorOf(
            DependencyResolver
                .For(actorSystem)
                .Props<T>()
                .WithSupervisorStrategy(actorSystem.DefaultSupervisorStrategy()), name);
    }

    private static IActorRef CreateNewActorWithRouter<T>(ActorSystem actorSystem, int numberOfInstance, int upperBound, string name) where T : BaseActor
    {
        return actorSystem.ActorOf(
            DependencyResolver
                .For(actorSystem)
                .Props<T>()
                .WithSupervisorStrategy(actorSystem.DefaultSupervisorStrategy())
                .WithRouter(new RoundRobinPool(numberOfInstance, new DefaultResizer(numberOfInstance, upperBound))),
            name);
    }

    public static SupervisorStrategy DefaultSupervisorStrategy(this ActorSystem actorSystem)
    {
        return _defaultSupervisorStrategy ??= new OneForOneStrategy(
            3, TimeSpan.FromSeconds(3), ex =>
            {
                if (ex is not ActorInitializationException)
                    return Directive.Resume;

                actorSystem?.Terminate().Wait(1000);

                return Directive.Stop;
            });
    }

    private static string GetActorFullName<T>(string name) where T : BaseActor
    {
        return $"{name.Trim()}_{typeof(T).Name}";
    }

}
