using MediatR;
using Microsoft.EntityFrameworkCore;
using SmartHOA.Application.Common.Exceptions;
using SmartHOA.Application.Common.Interfaces;

namespace SmartHOA.Application.Apartments.Queries.GetApartaments;

public record ApartmentDto(
    Guid Id,
    string Number,
    int Floor,
    string BuildingName,
    string Name);

public record GetApartmentQuery(Guid ApartmentId) : IRequest<ApartmentDto>;

public class GetApartmentQueryHandler : IRequestHandler<GetApartmentQuery, ApartmentDto>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;
    
    public GetApartmentQueryHandler(IApplicationDbContext context, ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<ApartmentDto> Handle(GetApartmentQuery request, CancellationToken cancellationToken)
    {
        var apartment = await _context.Apartments
            .Include(a => a.Building)
            .FirstOrDefaultAsync(a => a.Id == request.ApartmentId, cancellationToken);
        
        if (apartment == null)
            throw new NotFoundException("Апартамент", request.ApartmentId);
        
        bool isAdmin = _currentUserService.Role == "Admin";
        bool isManager = apartment.Building != null && apartment.Building.ManagerId == _currentUserService.UserId;
        bool isOwner = apartment.OwnerId == _currentUserService.UserId;
        
        if (!isAdmin && !isManager && !isOwner)
        {
            throw new ForbiddenAccessException("Нямате права да преглеждате данните за този апартамент.");
        }

        var buildingName = apartment.Building?.Name ?? string.Empty;
        return new ApartmentDto(apartment.Id, apartment.Number, apartment.Floor, buildingName, buildingName);
    }
}