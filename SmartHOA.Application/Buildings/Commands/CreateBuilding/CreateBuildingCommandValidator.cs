using FluentValidation;

namespace SmartHOA.Application.Buildings.Commands.CreateBuilding;

public class CreateBuildingCommandValidator : AbstractValidator<CreateBuildingCommand>
{
    public CreateBuildingCommandValidator()
    {
        RuleFor(v => v.Name)
            .NotEmpty().WithMessage("Името на сградата е задължително.")
            .MaximumLength(100).WithMessage("Името на сградата не може да надвишава 100 символа.");

        RuleFor(v => v.Address)
            .NotEmpty().WithMessage("Адресът на сградата е задължителен.")
            .MaximumLength(250).WithMessage("Адресът не може да надвишава 250 символа.");

        RuleFor(v => v.City)
            .NotEmpty().WithMessage("Градът е задължителен.")
            .MaximumLength(100).WithMessage("Градът не може да надвишава 100 символа.");

        RuleFor(v => v.FloorCount)
            .GreaterThan(0).WithMessage("Броят етажи трябва да бъде по-голям от 0.");

        RuleFor(v => v.ApartmentCount)
            .GreaterThan(0).WithMessage("Броят апартаменти трябва да бъде по-голям от 0.");

        RuleFor(v => v.ManagerId)
            .NotEmpty().WithMessage("Идентификаторът на домоуправителя е задължителен.");
    }
}