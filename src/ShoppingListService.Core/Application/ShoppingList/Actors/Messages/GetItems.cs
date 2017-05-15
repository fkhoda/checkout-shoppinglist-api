namespace ShoppingListService.Core.Application.ShoppingList.Actors.Messages
{
    public sealed class GetItems : ShoppingListMessage
    {
        public GetItems(string customerId, int? pageNumber, int? pageSize)
            : base(customerId)
        {
            PageNumber = pageNumber ?? 1;
            PageSize = pageSize ?? 10;
        }

        public int PageSize { get; }

        public int PageNumber { get; }
    }
}