namespace ShoppingListService.Core.Application.ShoppingList.Actors
{
    public interface IShoppingListsActorProvider
    {
        dynamic ActorInstance { get; }
    }
}