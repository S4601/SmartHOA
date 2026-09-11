using FluentValidation;

namespace SmartHOA.Application.Apartments.Commands;

public class CreateApartmentCommandValidator : AbstractValidator<CreateApartmentCommand>
{
    public CreateApartmentCommandValidator()
    {
        RuleFor(a => a.Number)
            .NotEmpty().WithMessage("Номерът на апартамента е задължителен.")
            .MaximumLength(10).WithMessage("Номерът не може да е по-дълъг от 10 символа.");
        
        RuleFor(a => a.Floor)
            .InclusiveBetween(-5, 150).WithMessage("Етажът трябва да бъде реално число между -5 и 150.");
        
        RuleFor(a => a.BuildingId)
            .NotEmpty().WithMessage("Идентификаторът на сградата е задължителен.");
    }
}