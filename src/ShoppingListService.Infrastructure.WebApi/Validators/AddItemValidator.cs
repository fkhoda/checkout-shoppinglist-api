namespace ShoppingListService.Infrastructure.WebApi.Validators
{
    using FluentValidation;

    using ShoppingListService.Core.Application.ShoppingList.Dtos.Requests;

    public class AddItemValidator : AbstractValidator<AddItemDto>
    {
        public AddItemValidator()
        {
            RuleFor(m => m.Name).NotEmpty().WithMessage("Item should have a name").WithErrorCode("2001");
            RuleFor(m => m.Quantity).GreaterThan(0).WithMessage("Quantity must be greater than zero").WithErrorCode("2002");
        }
    }
}
