using MediatR;
using Microsoft.EntityFrameworkCore;
using SmartHOA.Application.Common.Interfaces;

namespace SmartHOA.Application.Buildings.Queries.GetBuildings;

public record BuildingDto(
    Guid Id,
    string Name,
    string Address,
    string ManagerName);

public record GetBuildingsQuery : IRequest<List<BuildingDto>>;

public class GetBuildingsQueryHandler : IRequestHandler<GetBuildingsQuery, List<BuildingDto>>
{
    private readonly IApplicationDbContext _context;

    public GetBuildingsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<BuildingDto>> Handle(GetBuildingsQuery request, CancellationToken cancellationToken)
    {
        return await _context.Buildings
            .Include(b => b.Manager)
            .Select(b => new BuildingDto(
                b.Id,
                b.Name,
                b.Address,
                b.Manager != null ? $"{b.Manager.FirstName} {b.Manager.LastName}" : string.Empty
                )).ToListAsync(cancellationToken);
    }
}