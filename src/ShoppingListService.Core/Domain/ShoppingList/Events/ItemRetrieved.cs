namespace ShoppingListService.Core.Domain.ShoppingList.Events
{
    using ShoppingListService.Core.Domain.ShoppingList.Models;

    public sealed class ItemRetrieved : ShoppingListEvent
    {
        public ItemRetrieved(string name, int quantity, Status status = Status.ItemFound)
            : base(status)
        {
            Name = name;
            Quantity = quantity;
        }

        public string Name { get; }

        public int Quantity { get; }
    }
}