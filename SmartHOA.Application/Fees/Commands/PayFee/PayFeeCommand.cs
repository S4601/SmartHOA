using MediatR;
using Microsoft.EntityFrameworkCore;
using SmartHOA.Application.Common.Exceptions;
using SmartHOA.Application.Common.Interfaces;

namespace SmartHOA.Application.Fees.Commands.PayFee;

public record PayFeeCommand(Guid FeeId) : IRequest;

public class PayFeeCommandHandler : IRequestHandler<PayFeeCommand>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public PayFeeCommandHandler(IApplicationDbContext context, ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }
    
    public async Task Handle(PayFeeCommand request, CancellationToken cancellationToken)
    {
        var fee = await _context.Fees
            .Include(f => f.Apartment)
                .ThenInclude(a => a!.Building)
            .FirstOrDefaultAsync(f => f.Id == request.FeeId, cancellationToken);

        if (fee == null)
        {
            throw new NotFoundException("Такса", request.FeeId); 
        }

        bool isAdmin = _currentUserService.Role == "Admin";
        bool isManager = fee.Apartment?.Building != null && fee.Apartment.Building.ManagerId == _currentUserService.UserId;
        bool isOwner = fee.Apartment != null && fee.Apartment.OwnerId == _currentUserService.UserId;

        if (!isAdmin && !isManager && !isOwner)
        {
            throw new ForbiddenAccessException("Нямате права да платите или отбелязвате като платена тази такса.");
        }

        if (fee.IsPaid)
        {
            throw new InvalidOperationException("Тази такса вече е платена.");
        }
        
        fee.MarkAsPaid();

        await _context.SaveChangesAsync(cancellationToken);
    }
}