using MediatR;
using Microsoft.EntityFrameworkCore;
using SmartHOA.Application.Common.Interfaces;

namespace SmartHOA.Application.Fees.Queries.GetUnpaidFees;

public record UnpaidFeeDto(Guid Id, decimal Amount, string Description, DateTime DueDate);

public record GetUnpaidFeesQuery(Guid ApartmentId) : IRequest<List<UnpaidFeeDto>>;

public class GetUnpaidFeesQueryHandler : IRequestHandler<GetUnpaidFeesQuery, List<UnpaidFeeDto>>
{
    private readonly IApplicationDbContext _context;

    public GetUnpaidFeesQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }
    
    public async Task<List<UnpaidFeeDto>> Handle(GetUnpaidFeesQuery request, CancellationToken cancellationToken)
    {
        return await _context.Fees
            .Where(f => f.ApartmentId == request.ApartmentId && !f.IsPaid)
            .Select(f => new UnpaidFeeDto(f.Id, f.Amount, f.Description, f.DueDate))
            .ToListAsync(cancellationToken);
    }
}