namespace ShoppingListService.Core.Domain.ShoppingList.Events
{
    using ShoppingListService.Core.Domain.ShoppingList.Models;

    public class ShoppingListEvent
    {
        public ShoppingListEvent(Status status)
        {
            this.Status = status;
        }

        public Status Status { get; set; }
    }
}
