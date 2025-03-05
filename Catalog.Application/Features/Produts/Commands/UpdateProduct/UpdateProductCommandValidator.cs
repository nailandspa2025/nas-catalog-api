using FluentValidation;

namespace Catalog.Application.Features.Produts.Commands.UpdateProduct
{
    public class UpdateProductCommandValidator: AbstractValidator<UpdateProductCommand>
    {
		public UpdateProductCommandValidator()
		{
            RuleFor(v => v.ProductName)
            .NotEmpty()
            .MaximumLength(250);
        }
	}
}