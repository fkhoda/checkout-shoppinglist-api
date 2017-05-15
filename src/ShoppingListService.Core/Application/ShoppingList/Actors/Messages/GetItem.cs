namespace ShoppingListService.Core.Application.ShoppingList.Actors.Messages
{
    public sealed class GetItem : ShoppingListMessage
    {
        public GetItem(string customerId, string name)
            : base(customerId)
        {
            Name = name;
        }

        public string Name { get; }
    }
}