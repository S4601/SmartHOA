using MediatR;
using Microsoft.EntityFrameworkCore;
using SmartHOA.Application.Common.Interfaces;
using SmartHOA.Domain.Entities;

namespace SmartHOA.Application.Apartments.Queries.GetApartaments;

public record GetApartamentsQuery(Guid CurrentUserId, string? CurrentUserRole) : IRequest<List<ApartmentDto>>;

public class GetApartamentsQueryHandler : IRequestHandler<GetApartamentsQuery, List<ApartmentDto>>
{
    private readonly IApplicationDbContext _context;

    public GetApartamentsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<ApartmentDto>> Handle(GetApartamentsQuery request, CancellationToken cancellationToken)
    {
        var query = _context.Apartments.AsQueryable();
        
        bool isAdmin = request.CurrentUserRole == "Admin";
        bool isManager = request.CurrentUserRole == "Manager";
        
        if (isAdmin)
        {
            
        }
        else if (isManager)
        {
            query = query.Where(a => a.Building.ManagerId == request.CurrentUserId);
        }
        else 
        {
            query = query.Where(a => a.OwnerId == request.CurrentUserId);
        }

        var apartaments = await query
            .Select(a => new ApartmentDto(
                a.Id,
                a.Number,
                a.Floor,
                a.Building.Name)).ToListAsync(cancellationToken);
        
        return apartaments;
    }
}