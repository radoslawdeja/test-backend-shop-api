using FluentValidation;
using Test.Shop.Application.DTO;
using Test.Shop.Application.Validators.Interfaces;

namespace Test.Shop.Application.Validators
{
    public class ShopDetailsAddValidator : AbstractValidator<ShopDetailsAddDto>, IShopDetailsAddValidator
    {
        public ShopDetailsAddValidator()
        {
            RuleFor(x => x.Name).NotEmpty();
            RuleFor(x => x.CategoryId).NotNull().GreaterThan(0);

            RuleFor(x => x.Description)
                .MaximumLength(255)
                .WithMessage("Description maximum length is 255 characters.");
        }
    }
}
