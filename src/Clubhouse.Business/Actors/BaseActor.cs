﻿using Akka.Actor;
using Akka.DependencyInjection;

namespace Clubhouse.Business.Actors;

public class BaseActor : ReceiveActor
{
    protected void Publish(object @event)
    {
        Context.Dispatcher.EventStream.Publish(@event);
    }

    protected void ReceiveAsync<TMessage, TActor>(bool poisonPill = false, string? name = null)
        where TActor : BaseActor
        => ReceiveAsync<TMessage>(async message => await CreateActorAsync<TActor, TMessage>(message, poisonPill, name));


    private async Task CreateActorAsync<TActor, TMessage>(TMessage message, bool poisonPill = false, string? name = null)
        where TActor : BaseActor
    {
        var actorRef = Context.ActorOf(
            DependencyResolver
                .For(Context.System)
                .Props<TActor>()
                .WithSupervisorStrategy(GetDefaultSupervisorStrategy),
            GetActorName(name));

        actorRef.Forward(message);

        if (poisonPill)
        {
            actorRef.Tell(PoisonPill.Instance);
        }

        await Task.CompletedTask;

        static string GetActorName(string? name)
        {
            return $"hubtel-{typeof(TActor).Name.ToLower()}{(string.IsNullOrEmpty(name) ? "" : "-" + name.ToLower())}-{Guid.NewGuid():N}";
        }
    }

    private static SupervisorStrategy GetDefaultSupervisorStrategy => new OneForOneStrategy(
        3, TimeSpan.FromSeconds(3), ex =>
        {
            if (!(ex is ActorInitializationException))
                return Directive.Resume;

            Context.System.Terminate().Wait(1000);

            return Directive.Stop;
        });
}