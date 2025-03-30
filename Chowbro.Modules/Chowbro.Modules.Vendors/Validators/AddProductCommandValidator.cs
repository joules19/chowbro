using Chowbro.Modules.Vendors.Commands;
using FluentValidation;
using static Chowbro.Core.Enums.Product;

public class AddProductCommandValidator : AbstractValidator<AddProductCommand>
{
    public AddProductCommandValidator()
    {
        RuleFor(x => x.VendorId).NotEmpty();
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Description).MaximumLength(500);
        RuleFor(x => x.BasePrice).GreaterThan(0);
        RuleFor(x => x.CategoryId).NotEmpty();
        RuleFor(x => x.PreparationTime).GreaterThanOrEqualTo(0);

        RuleForEach(x => x.OptionCategories).SetValidator(new ProductOptionCategoryDtoValidator());
        RuleForEach(x => x.Images).Must(f => f.Length <= 5 * 1024 * 1024) // 5MB
            .WithMessage("Image size must be less than 5MB");
    }
}

public class ProductOptionCategoryDtoValidator : AbstractValidator<ProductOptionCategoryDto>
{
    public ProductOptionCategoryDtoValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(50);
        RuleFor(x => x.SelectionType).IsInEnum();

        When(x => x.SelectionType == OptionSelectionType.Multiple, () =>
        {
            RuleFor(x => x.MinOptions).GreaterThanOrEqualTo(1)
                .LessThanOrEqualTo(x => x.MaxOptions);
            RuleFor(x => x.MaxOptions).GreaterThanOrEqualTo(x => x.MinOptions ?? 1);
        });

        RuleForEach(x => x.Options).SetValidator(new ProductOptionDtoValidator());
    }
}

public class ProductOptionDtoValidator : AbstractValidator<ProductOptionDto>
{
    public ProductOptionDtoValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(50);
        RuleFor(x => x.AdditionalPrice).GreaterThanOrEqualTo(0);
    }
}