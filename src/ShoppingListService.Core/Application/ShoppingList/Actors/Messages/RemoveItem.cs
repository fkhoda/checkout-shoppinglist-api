namespace ShoppingListService.Core.Application.ShoppingList.Actors.Messages
{
    public sealed class RemoveItem : ShoppingListMessage
    {
        public RemoveItem(string customerId, string name)
            : base(customerId)
        {
            Name = name;
        }

        public string Name { get; }
    }
}