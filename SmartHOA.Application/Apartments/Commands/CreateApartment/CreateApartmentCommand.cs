using MediatR;
using Microsoft.EntityFrameworkCore;
using SmartHOA.Application.Common.Exceptions;
using SmartHOA.Application.Common.Interfaces;
using SmartHOA.Domain.Entities;

namespace SmartHOA.Application.Apartments.Commands;

public record CreateApartmentCommand(
    string Number,
    int Floor,
    Guid BuildingId) : IRequest<Guid>;

public class CreateApartmentCommandHandler : IRequestHandler<CreateApartmentCommand, Guid>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;
    
    public CreateApartmentCommandHandler(IApplicationDbContext context, ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<Guid> Handle(CreateApartmentCommand request, CancellationToken cancellationToken)
    {
        var building = await _context.Buildings
            .FirstOrDefaultAsync(b => b.Id == request.BuildingId, cancellationToken);
        
        if (building == null) 
            throw new NotFoundException("Сграда", request.BuildingId);
        
        if (_currentUserService.Role != "Admin" && building.ManagerId != _currentUserService.UserId)
        {
            throw new ForbiddenAccessException("Нямате права да добавяте апартаменти към тази сграда, тъй като не сте нейният управител.");
        }

        var exists = await _context.Apartments
            .AnyAsync(a => a.BuildingId == request.BuildingId && a.Number == request.Number, cancellationToken);

        if (exists)
        {
            throw new BadRequestException($"Апартамент с номер '{request.Number}' вече съществува в тази сграда.");
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