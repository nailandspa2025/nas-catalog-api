
using FluentValidation;
namespace Catalog.Application.Features.Stores.Commands.CreateStore
{
    public class CreateStoreCommandValidator:  AbstractValidator<CreateStoreCommand>
	{
		public CreateStoreCommandValidator()
		{
            RuleFor(v => v.StoreName)
                .NotEmpty()
                .MaximumLength(250);

            RuleFor(v => v.AddressStore)
               .NotEmpty()
               .MaximumLength(500);

            //RuleFor(v => v.OwnerId)
            //    .NotEmpty();

            //RuleFor(v => v.Lat)
            //    .NotEmpty();

            //RuleFor(v => v.Lng)
            //    .NotEmpty();

            RuleFor(v => v.Hotline)
                .NotEmpty()
                .MaximumLength(20);

            //RuleFor(v => v.OpenTime)
            //    .NotEmpty();

            //RuleFor(v => v.CloseTime)
            //    .NotEmpty();

        }
	}
}

