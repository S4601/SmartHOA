using MediatR;
using Microsoft.EntityFrameworkCore;
using SmartHOA.Application.Common.Exceptions;
using SmartHOA.Application.Common.Interfaces;
using SmartHOA.Domain.Entities;

namespace SmartHOA.Application.Buildings.Commands.CreateBuilding;

public record CreateBuildingCommand(
    string Name,
    string Address,
    string City,
    int FloorCount,
    int ApartmentCount,
    Guid ManagerId) : IRequest<Guid>;

public class CreateBuildingCommandHandler : IRequestHandler<CreateBuildingCommand, Guid>
{
    private readonly IApplicationDbContext _context;

    public CreateBuildingCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Guid> Handle(CreateBuildingCommand request, CancellationToken cancellationToken)
    {
        var manager = await _context.Users
            .FirstOrDefaultAsync(u => u.Id == request.ManagerId, cancellationToken);

        if (manager == null)
        {
            throw new NotFoundException("Потребител (домоуправител)", request.ManagerId);
        }

        var building = new Building
        {
            Name = request.Name,
            Address = request.Address,
            City = request.City,
            FloorCount = request.FloorCount,
            ApartmentCountPerFloor = request.ApartmentCount,
            ManagerId = request.ManagerId
        };
        
        _context.Buildings.Add(building);
        
        await _context.SaveChangesAsync(cancellationToken);
        return building.Id;
    }
}