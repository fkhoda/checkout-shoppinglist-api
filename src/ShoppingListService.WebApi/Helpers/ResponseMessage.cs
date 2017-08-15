namespace ShoppingListService.WebApi.Helpers
{
    using Microsoft.AspNetCore.Mvc;

    using ShoppingListService.Core.Application.ShoppingList.Dtos.Responses;
    using ShoppingListService.Core.Domain.ShoppingList.Models;

    public static class ResponseMessage
    {
        public static OkObjectResult Ok(Status status, string message)
        {
            return new OkObjectResult(new ResponseMessageDto(status, message));
        }

        public static CreatedAtActionResult CreatedAtAction(string actionName, string controllerName, object routeValues, Status status, string message)
        {
            return new CreatedAtActionResult(actionName, controllerName, routeValues, new ResponseMessageDto(status, message));
        }

        public static NotFoundObjectResult NotFound(Status status, string message)
        {
            return new NotFoundObjectResult(new ResponseMessageDto(status, message));
        }

        public static BadRequestObjectResult BadRequest(Status status, string message)
        {
            return new BadRequestObjectResult(new ResponseMessageDto(status, message));
        }

        public static BadRequestObjectResult BadRequest(string code, string message)
        {
            return new BadRequestObjectResult(new ResponseMessageDto(code, message));
        }
    }
}
