using System;
using FluentValidation;

namespace Catalog.Application.Features.Produts.Commands.CreateProduct
{
	public class CreateProductCommandValidator: AbstractValidator<CreateProductCommand>
    {
		public CreateProductCommandValidator()
		{
            RuleFor(v => v.ProductName)
            .NotEmpty()
            .MaximumLength(250);
        }
	}
}

