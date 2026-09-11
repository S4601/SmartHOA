using MediatR;
using Microsoft.EntityFrameworkCore;
using SmartHOA.Application.Buildings.Queries.GetBuildings;
using SmartHOA.Application.Common.Interfaces;

namespace SmartHOA.Application.Apartments.Queries.GetApartaments;

public record ApartmentDto (
    Guid Id,
    string Number,
    int Floor,
    string Name);


public record GetApartmentQuery(Guid ApartmentId, Guid CurrentUserId, string? CurrentUserRole) : IRequest<ApartmentDto>;

public class GetApartamentQueryHandler : IRequestHandler<GetApartmentQuery, ApartmentDto>
{
    private readonly IApplicationDbContext _context;
    
    public GetApartamentQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ApartmentDto> Handle(GetApartmentQuery request, CancellationToken cancellationToken)
    {
        var apartment = await _context.Apartments
            .Include(a => a.Building)
            .FirstOrDefaultAsync(a => a.Id == request.ApartmentId, cancellationToken);
        
        if (apartment == null)
            throw new Exception("Апартаментът не е намерен.");
        
        bool isAdmin = request.CurrentUserRole == "Admin";
        bool isOwner = apartment.OwnerId == request.CurrentUserId;
        
        
        if (!isAdmin && !isOwner)
        {
            throw new UnauthorizedAccessException("Нямате права да преглеждате данните за този апартамент.");
        }

        return new ApartmentDto(apartment.Id, apartment.Number, apartment.Floor, apartment.Building.Name);
    }
}