namespace ShoppingListService.Core.Domain.ShoppingList.Models
{
    public enum Status
    {
        ItemAdded = 3000,
        ItemRemoved,
        QuantityUpdated,
        ItemFound,
        ItemNotFound,
        UnexpectedError
    }
}