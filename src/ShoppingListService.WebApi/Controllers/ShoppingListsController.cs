namespace ShoppingListService.WebApi.Controllers
{
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Mvc;

    using Proto;

    using ShoppingListService.Core.Application.ShoppingList.Actors;
    using ShoppingListService.Core.Application.ShoppingList.Actors.Messages;
    using ShoppingListService.Core.Application.ShoppingList.Dtos.Requests;
    using ShoppingListService.Core.Application.ShoppingList.Dtos.Responses;
    using ShoppingListService.Core.Domain.ShoppingList.Events;
    using ShoppingListService.Core.Domain.ShoppingList.Models;
    using ShoppingListService.WebApi.Helpers;
    using ShoppingListService.WebApi.Validators;

    [Route("api/shoppingLists/{customerId}")]
    public class ShoppingListsController : Controller
    {
        private const string ItemAdded = "Item added successfully";
        private const string QuantityUpdated = "Quantity updated successfully";
        private const string ItemUpdatedWithNewQuantity = "Item successfully updated with new quantity";
        private const string ItemRemoved = "Item removed successfully";
        private const string ItemNotFound = "Item not found";
        private const string UnexpectedError = "Unexpected error";

        private readonly IShoppingListsActorProvider shoppingListsActorProvider;

        public ShoppingListsController(IShoppingListsActorProvider shoppingListsActorProvider)
        {
            this.shoppingListsActorProvider = shoppingListsActorProvider;
        }

        [HttpGet("items")]
        public async Task<ShoppingListDto> Get(string customerId, [FromQuery] GetItemsDto items)
        {
            var actor = (PID)this.shoppingListsActorProvider.ActorInstance;
            return await actor.RequestAsync<ShoppingListDto>(new GetItems(customerId, items.PageNumber, items.PageSize));
        }

        [HttpGet("items/{name}")]
        public async Task<IActionResult> Get(string customerId, string name)
        {
            var actor = (PID)this.shoppingListsActorProvider.ActorInstance;

            var @event = await actor.RequestAsync<ShoppingListEvent>(new GetItem(customerId, name));

            switch (@event.Status)
            {
                case Status.ItemFound: return this.Ok(new ShoppingListItemDto { Name = ((ItemRetrieved)@event).Name, Quantity = ((ItemRetrieved)@event).Quantity });
                case Status.ItemNotFound: return ResponseMessage.NotFound(@event.Status, ItemNotFound);
                default: return ResponseMessage.BadRequest(@event.Status, UnexpectedError);
            }
        }

        [HttpPost("items")]
        public async Task<IActionResult> Post(string customerId, [FromBody] AddItemDto item)
        {
            if (item == null)
            {
                return ResponseMessage.BadRequest(Status.UnexpectedError, UnexpectedError);
            }

            var validator = new AddItemValidator();
            var results = validator.Validate(item);

            if (!results.IsValid)
            {
                var error = results.Errors.FirstOrDefault();
                return ResponseMessage.BadRequest(error.ErrorCode, error.ErrorMessage);
            }

            var actor = (PID)this.shoppingListsActorProvider.ActorInstance;

            var @event = await actor.RequestAsync<ShoppingListEvent>(new AddItem(customerId, item.Name, item.Quantity));

            switch (@event.Status)
            {
                case Status.ItemAdded: return ResponseMessage.CreatedAtAction("Get", "ShoppingLists", new { name = item.Name }, @event.Status, ItemAdded);
                case Status.QuantityUpdated: return ResponseMessage.Ok(@event.Status, ItemUpdatedWithNewQuantity);
                default: return ResponseMessage.BadRequest(@event.Status, UnexpectedError);
            }
        }

        [HttpPut("items/{name}")]
        public async Task<IActionResult> UpdateQuantity(string customerId, string name, [FromBody] UpdateQuantityDto item)
        {
            if (item == null)
            {
                return ResponseMessage.BadRequest(Status.UnexpectedError, UnexpectedError);
            }

            var validator = new UpdateQuantityValidator();
            var results = validator.Validate(item);

            if (!results.IsValid)
            {
                var error = results.Errors.FirstOrDefault();
                return ResponseMessage.BadRequest(error.ErrorCode, error.ErrorMessage);
            }

            var actor = (PID)this.shoppingListsActorProvider.ActorInstance;

            var @event = await actor.RequestAsync<ShoppingListEvent>(new UpdateQuantity(customerId, name, item.Quantity));

            switch (@event.Status)
            {
                case Status.QuantityUpdated: return ResponseMessage.Ok(@event.Status, QuantityUpdated);
                case Status.ItemNotFound: return ResponseMessage.NotFound(@event.Status, ItemNotFound);
                default: return ResponseMessage.BadRequest(@event.Status, UnexpectedError);
            }
        }

        [HttpDelete("items/{name}")]
        public async Task<IActionResult> Delete(string customerId, string name)
        {
            var actor = (PID)this.shoppingListsActorProvider.ActorInstance;

            var @event = await actor.RequestAsync<ShoppingListEvent>(new RemoveItem(customerId, name));

            switch (@event.Status)
            {
                case Status.ItemRemoved: return ResponseMessage.Ok(@event.Status, ItemRemoved);
                case Status.ItemNotFound: return ResponseMessage.NotFound(@event.Status, ItemNotFound);
                default: return ResponseMessage.BadRequest(@event.Status, UnexpectedError);
            }
        }
    }
}
