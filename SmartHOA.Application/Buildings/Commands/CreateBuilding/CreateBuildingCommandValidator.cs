using FluentValidation;

namespace SmartHOA.Application.Buildings.Commands.CreateBuilding;

public class CreateBuildingCommandValidator : AbstractValidator<CreateBuildingCommand>
{
    public CreateBuildingCommandValidator()
    {
        RuleFor(v => v.Name).NotEmpty().WithMessage("The name of the building cannot be empty.")
            .MaximumLength(100).WithMessage("The name of the building cannot be longer than 100 characters.");

        RuleFor(v => v.Address).NotEmpty().WithMessage("The address cannot be empty.");
        
        RuleFor(v => v.ManagerId).NotEmpty().WithMessage("The manager id cannot be empty.");
    }
}