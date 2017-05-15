namespace ShoppingListService.Infrastructure.WebApi.Validators
{
    using FluentValidation;

    using ShoppingListService.Core.Application.ShoppingList.Dtos.Requests;

    public class UpdateQuantityValidator : AbstractValidator<UpdateQuantityDto>
    {
        public UpdateQuantityValidator()
        {
            RuleFor(m => m.Quantity).GreaterThan(0).WithMessage("Quantity must be greater than zero").WithErrorCode("2002");
        }
    }
}
