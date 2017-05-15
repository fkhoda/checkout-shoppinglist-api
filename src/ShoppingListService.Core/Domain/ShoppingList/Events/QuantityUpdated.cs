namespace ShoppingListService.Core.Domain.ShoppingList.Events
{
    using ShoppingListService.Core.Domain.ShoppingList.Models;

    public sealed class QuantityUpdated : ShoppingListEvent
    {
        public QuantityUpdated(string name, int quantity, Status status = Status.QuantityUpdated)
            : base(status)
        {
            Name = name;
            Quantity = quantity;
        }

        public string Name { get; }

        public int Quantity { get; }
    }
}