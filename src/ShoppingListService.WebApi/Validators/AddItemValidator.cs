namespace ShoppingListService.WebApi.Validators
{
    using FluentValidation;

    using ShoppingListService.Core.Application.ShoppingList.Dtos.Requests;

    public class AddItemValidator : AbstractValidator<AddItemDto>
    {
        public AddItemValidator()
        {
            this.RuleFor(m => m.Name).NotEmpty().WithMessage("Item should have a name").WithErrorCode("2001");
            this.RuleFor(m => m.Quantity).GreaterThan(0).WithMessage("Quantity must be greater than zero").WithErrorCode("2002");
        }
    }
}
