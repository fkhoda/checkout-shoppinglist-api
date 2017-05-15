namespace ShoppingListService.Core.Application.ShoppingList.Dtos.Responses
{
    using System.Collections.Generic;

    public sealed class ShoppingListDto
    {
        public long Count { get; set; }

        public IEnumerable<ShoppingListItemDto> Items { get; set; } = new List<ShoppingListItemDto>();
    }
}
