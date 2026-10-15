using MediatR;
using Microsoft.EntityFrameworkCore;
using SmartHOA.Application.Common.Interfaces;

namespace SmartHOA.Application.Apartments.Queries.GetApartaments;

public record GetApartamentsQuery : IRequest<List<ApartmentDto>>;

public class GetApartamentsQueryHandler : IRequestHandler<GetApartamentsQuery, List<ApartmentDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public GetApartamentsQueryHandler(IApplicationDbContext context, ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<List<ApartmentDto>> Handle(GetApartamentsQuery request, CancellationToken cancellationToken)
    {
        var query = _context.Apartments.AsNoTracking().AsQueryable();
        
        var role = _currentUserService.Role;
        var currentUserId = _currentUserService.UserId;

        if (role == "Admin")
        {
            // Админ вижда всички апартаменти
        }
        else if (role == "Manager")
        {
            // Домоуправител вижда апартаментите в сградите, които управлява
            query = query.Where(a => a.Building != null && a.Building.ManagerId == currentUserId);
        }
        else 
        {
            // Собственик / Обитател вижда само своите апартаменти
            query = query.Where(a => a.OwnerId == currentUserId);
        }

        var apartments = await query
            .Select(a => new ApartmentDto(
                a.Id,
                a.Number,
                a.Floor,
                a.Building != null ? a.Building.Name : string.Empty,
                a.Building != null ? a.Building.Name : string.Empty))
            .ToListAsync(cancellationToken);
        
        return apartments;
    }
}