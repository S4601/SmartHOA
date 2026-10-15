using MediatR;
using Microsoft.EntityFrameworkCore;
using SmartHOA.Application.Common.Exceptions;
using SmartHOA.Application.Common.Interfaces;

namespace SmartHOA.Application.Fees.Queries.GetUnpaidFees;

public record UnpaidFeeDto(Guid Id, decimal Amount, string Description, DateTime DueDate);

public record GetUnpaidFeesQuery(Guid ApartmentId) : IRequest<List<UnpaidFeeDto>>;

public class GetUnpaidFeesQueryHandler : IRequestHandler<GetUnpaidFeesQuery, List<UnpaidFeeDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public GetUnpaidFeesQueryHandler(IApplicationDbContext context, ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }
    
    public async Task<List<UnpaidFeeDto>> Handle(GetUnpaidFeesQuery request, CancellationToken cancellationToken)
    {
        var apartment = await _context.Apartments
            .Include(a => a.Building)
            .FirstOrDefaultAsync(a => a.Id == request.ApartmentId, cancellationToken);

        if (apartment == null)
        {
            throw new NotFoundException("Апартамент", request.ApartmentId);
        }

        bool isAdmin = _currentUserService.Role == "Admin";
        bool isManager = apartment.Building != null && apartment.Building.ManagerId == _currentUserService.UserId;
        bool isOwner = apartment.OwnerId == _currentUserService.UserId;

        if (!isAdmin && !isManager && !isOwner)
        {
            throw new ForbiddenAccessException("Нямате права да преглеждате задълженията за този апартамент.");
        }

        return await _context.Fees
            .AsNoTracking()
            .Where(f => f.ApartmentId == request.ApartmentId && !f.IsPaid)
            .Select(f => new UnpaidFeeDto(f.Id, f.Amount, f.Description, f.DueDate))
            .ToListAsync(cancellationToken);
    }
}