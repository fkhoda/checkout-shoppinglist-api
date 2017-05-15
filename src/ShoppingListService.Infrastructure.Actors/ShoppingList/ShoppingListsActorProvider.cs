namespace ShoppingListService.Infrastructure.Actors.ShoppingList
{
    using Proto;
    using Proto.Persistence;

    using ShoppingListService.Core.Application.ShoppingList.Actors;

    public class ShoppingListsActorProvider : IShoppingListsActorProvider
    {
        public dynamic ActorInstance { get; }

        public ShoppingListsActorProvider(IProvider persistenceProvider)
        {
            var props = Actor.FromProducer(() => new ShoppingListsActor(persistenceProvider));

            ActorInstance = Actor.SpawnNamed(props, "ShoppingLists");
        }
    }
}
