namespace ShoppingListService.Core.Application.ShoppingList.Dtos.Responses
{
    using System;

    using ShoppingListService.Core.Domain.ShoppingList.Models;

    public class ResponseMessageDto
    {
        public ResponseMessageDto(Status status, string message)
            : this(Convert.ToInt32(status), message)
        {
        }

        public ResponseMessageDto(string code, string message)
            : this(Convert.ToInt32(code), message)
        {
        }

        public ResponseMessageDto(int code, string message)
        {
            Code = code;
            Message = message;
        }

        public int Code { get; }

        public string Message { get; }
    }
}