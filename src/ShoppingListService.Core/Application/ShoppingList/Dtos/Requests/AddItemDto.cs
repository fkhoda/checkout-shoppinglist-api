namespace ShoppingListService.Core.Application.ShoppingList.Dtos.Requests
{
    public sealed class AddItemDto : BaseRequestDto
    {
        public string Name { get; set; }

        public int Quantity { get; set; }
    }
}
