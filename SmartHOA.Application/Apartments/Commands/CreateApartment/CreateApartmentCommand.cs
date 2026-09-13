using MediatR;
using SmartHOA.Application.Common.Interfaces;
using SmartHOA.Domain.Entities;

namespace SmartHOA.Application.Apartments.Commands;

public record CreateApartmentCommand(
    string Number,
    int Floor,
    Guid BuildingId,
    Guid CurrentUserId,
    string CurrentUserRole) : IRequest<Guid>;

public class CreateApartmentCommandHandler : IRequestHandler<CreateApartmentCommand, Guid>
{
    private readonly IApplicationDbContext _context;
    
    public CreateApartmentCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Guid> Handle(CreateApartmentCommand request, CancellationToken cancellationToken)
    {
        var building = await _context.Buildings.FindAsync(request.BuildingId);
        
        if (building == null) 
            throw new Exception("Сградата не е намерена.");
        
        if (request.CurrentUserRole != "Admin" && building.ManagerId != request.CurrentUserId)
        {
            throw new UnauthorizedAccessException("Нямате права да добавяте апартаменти към тази сграда, тъй като не сте нейният управител.");
        }
        
        var apartment = new Apartment
        {
            Number = request.Number,
            Floor = request.Floor,
            BuildingId = request.BuildingId
        };

        _context.Apartments.Add(apartment);
        
        await _context.SaveChangesAsync(cancellationToken);
        return apartment.Id;
    }
}