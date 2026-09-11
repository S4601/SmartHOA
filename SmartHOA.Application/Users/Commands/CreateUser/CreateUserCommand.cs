using MediatR;
using SmartHOA.Application.Common.Interfaces;
using SmartHOA.Domain.Entities;
using SmartHOA.Domain.Enums;

namespace SmartHOA.Application.Users.Commands.CreateUser;

public record CreateUserCommand(
    string FirstName,
    string LastName,
    string Email,
    string? PhoneNumber,
    UserType UserType,
    string Password) : IRequest<Guid>;

public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, Guid>
{
    private readonly IApplicationDbContext _context;
    
    public CreateUserCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }
    
    public async Task<Guid> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        var user = new User
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            PhoneNumber = request.PhoneNumber,
            UserType = request.UserType,
            PasswordHash = request.Password
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync(cancellationToken);

        return user.Id;
    }
}