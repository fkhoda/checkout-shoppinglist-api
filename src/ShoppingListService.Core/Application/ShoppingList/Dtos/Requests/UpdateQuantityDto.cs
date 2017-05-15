namespace ShoppingListService.Core.Application.ShoppingList.Dtos.Requests
{
    public sealed class UpdateQuantityDto : BaseRequestDto
    {
        public int Quantity { get; set; }
    }
}
