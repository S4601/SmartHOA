using MediatR;
using SmartHOA.Application.Common.Interfaces;
using SmartHOA.Domain.Entities;

namespace SmartHOA.Application.Buildings.Commands.CreateBuilding;

public record CreateBuildingCommand(
    string Name,
    string Address,
    string City,
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
        var building = new Building
        {
            Name = request.Name,
            Address = request.Address,
            City = request.City,
            ManagerId = request.ManagerId
        };
        
        _context.Buildings.Add(building);
        
        await _context.SaveChangesAsync(cancellationToken);
        return building.Id;
    }
}