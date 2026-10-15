using MediatR;
using Microsoft.EntityFrameworkCore;
using SmartHOA.Application.Common.Exceptions;
using SmartHOA.Application.Common.Interfaces;

namespace SmartHOA.Application.Buildings.Queries.GetBuildingById;

public record BuildingDetailDto(
    Guid Id,
    string Name,
    string Address,
    string City,
    int FloorCount,
    int ApartmentCountPerFloor,
    string ManagerName);

public record GetBuildingByIdQuery(Guid Id) : IRequest<BuildingDetailDto>;

public class GetBuildingByIdQueryHandler : IRequestHandler<GetBuildingByIdQuery, BuildingDetailDto>
{
    private readonly IApplicationDbContext _context;

    public GetBuildingByIdQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<BuildingDetailDto> Handle(GetBuildingByIdQuery request, CancellationToken cancellationToken)
    {
        var building = await _context.Buildings
            .AsNoTracking()
            .Include(b => b.Manager)
            .FirstOrDefaultAsync(b => b.Id == request.Id, cancellationToken);
        
        if (building == null) 
            throw new NotFoundException("Сграда", request.Id);

        return new BuildingDetailDto(
            building.Id,
            building.Name,
            building.Address,
            building.City,
            building.FloorCount,
            building.ApartmentCountPerFloor,
            building.Manager != null ? $"{building.Manager.FirstName} {building.Manager.LastName}" : string.Empty
        );
    }
}