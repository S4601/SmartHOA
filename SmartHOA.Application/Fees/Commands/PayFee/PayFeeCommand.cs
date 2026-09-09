using MediatR;
using Microsoft.EntityFrameworkCore;
using SmartHOA.Application.Common.Interfaces;

namespace SmartHOA.Application.Fees.Commands.PayFee;

public record PayFeeCommand(Guid FeeId) : IRequest;

public class PayFeeCommandHandler : IRequestHandler<PayFeeCommand>
{
    private readonly IApplicationDbContext _context;

    public PayFeeCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }
    
    public async Task Handle(PayFeeCommand request, CancellationToken cancellationToken)
    {
        var fee = await _context.Fees
            .FirstOrDefaultAsync(f => f.Id == request.FeeId, cancellationToken);

        if (fee == null)
        {
            throw new Exception("Таксата не е намерена."); 
        }

        if (fee.IsPaid)
        {
            throw new InvalidOperationException("Тази такса вече е платена.");
        }
        
        fee.MarkAsPaid();

        await _context.SaveChangesAsync(cancellationToken);
    }
}