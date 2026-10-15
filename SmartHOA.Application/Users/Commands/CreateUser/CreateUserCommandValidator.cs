using FluentValidation;

namespace SmartHOA.Application.Users.Commands.CreateUser;

public class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>
{
    public CreateUserCommandValidator()
    {
        RuleFor(v => v.FirstName)
            .NotEmpty().WithMessage("Името е задължително.")
            .MaximumLength(50).WithMessage("Името не може да надвишава 50 символа.");

        RuleFor(v => v.LastName)
            .NotEmpty().WithMessage("Фамилията е задължителна.")
            .MaximumLength(50).WithMessage("Фамилията не може да надвишава 50 символа.");

        RuleFor(v => v.Email)
            .NotEmpty().WithMessage("Имейлът е задължителен.")
            .EmailAddress().WithMessage("Моля, въведете валиден имейл адрес.")
            .MaximumLength(100).WithMessage("Имейлът не може да надвишава 100 символа.");

        RuleFor(v => v.Password)
            .NotEmpty().WithMessage("Паролата е задължителна.")
            .MinimumLength(6).WithMessage("Паролата трябва да бъде поне 6 символа.");

        RuleFor(v => v.UserType)
            .IsInEnum().WithMessage("Невалиден тип потребител.");
    }
}
