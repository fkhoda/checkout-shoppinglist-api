namespace ShoppingListService.Core.Domain.ShoppingList.Events
{
    using ShoppingListService.Core.Domain.ShoppingList.Models;

    public sealed class ItemAdded : ShoppingListEvent
    {
        public ItemAdded(string name, int quantity, Status status = Status.ItemAdded)
            : base(status)
        {
            Name = name;
            Quantity = quantity;
        }

        public string Name { get; }

        public int Quantity { get; }
    }
}