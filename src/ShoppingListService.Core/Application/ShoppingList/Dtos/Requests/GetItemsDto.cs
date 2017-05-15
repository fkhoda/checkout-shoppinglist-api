namespace ShoppingListService.Core.Application.ShoppingList.Dtos.Requests
{
    public class GetItemsDto : BaseRequestDto
    {
        public int? PageSize { get; set; }

        public int? PageNumber { get; set; }
    }
}
