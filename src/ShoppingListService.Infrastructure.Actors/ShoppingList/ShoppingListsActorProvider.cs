namespace ShoppingListService.Infrastructure.Actors.ShoppingList
{
    using Proto;
    using Proto.Persistence;

    using ShoppingListService.Core.Application.ShoppingList.Actors;
    using ShoppingListService.Infrastructure.Actor.Monitoring;

    public class ShoppingListsActorProvider : IShoppingListsActorProvider
    {
        public dynamic ActorInstance { get; }

        public ShoppingListsActorProvider(IProvider persistenceProvider, IMonitoringProvider monitoringProvider)
        {
            var props = Actor.FromProducer(() => new ShoppingListsActor(persistenceProvider, monitoringProvider))
                .WithReceiveMiddleware(Monitoring.ForReceiveMiddlewareUsing(monitoringProvider))
                .WithSenderMiddleware(Monitoring.ForSenderMiddlewareUsing(monitoringProvider));

            ActorInstance = Actor.SpawnNamed(props, "ShoppingLists");
        }
    }
}
