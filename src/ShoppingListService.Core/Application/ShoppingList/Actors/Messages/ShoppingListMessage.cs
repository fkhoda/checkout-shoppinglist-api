namespace ShoppingListService.Core.Application.ShoppingList.Actors.Messages
{
    public abstract class ShoppingListMessage
    {
        public ShoppingListMessage(string customerId)
        {
            CustomerId = customerId;
        }

        public string CustomerId { get; set; }
    }
}