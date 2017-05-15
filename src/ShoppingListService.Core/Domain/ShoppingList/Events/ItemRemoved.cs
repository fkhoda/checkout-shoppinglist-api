namespace ShoppingListService.Core.Domain.ShoppingList.Events
{
    using ShoppingListService.Core.Domain.ShoppingList.Models;

    public sealed class ItemRemoved : ShoppingListEvent
    {
        public ItemRemoved(string name, Status status = Status.ItemRemoved)
            : base(status)
        {
            Name = name;
        }

        public string Name { get; }
    }
}