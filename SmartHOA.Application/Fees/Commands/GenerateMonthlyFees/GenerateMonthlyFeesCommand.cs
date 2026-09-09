using MediatR;
using Microsoft.EntityFrameworkCore;
using SmartHOA.Application.Common.Interfaces;
using SmartHOA.Domain.Entities;
using SmartHOA.Domain.Enums;

namespace SmartHOA.Application.Fees.Commands.GenerateMonthlyFees;

public record GenerateMonthlyFeesCommand : IRequest;

public class GenerateMonthlyFeesCommandHandler : IRequestHandler<GenerateMonthlyFeesCommand>
{
    private readonly IApplicationDbContext _context;
    
    public GenerateMonthlyFeesCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task Handle(GenerateMonthlyFeesCommand request, CancellationToken cancellationToken)
    {
        var aprtments = await _context.Apartments.ToListAsync(cancellationToken);
        
        var dueDate = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 15);
        var currentMonthName = DateTime.UtcNow.ToString("MMMM yyyy");

        foreach (var apt in aprtments)
        {
            decimal occupantsTotal = apt.OccupantsCount * 15m;
            decimal petsTotal = apt.HasPets ? 15m : 0;
            decimal totalMaintenanceAmount = occupantsTotal + petsTotal;

            if (totalMaintenanceAmount > 0)
            {
                var maintenanceFee = new MonthlyFee
                {
                    Amount = totalMaintenanceAmount,
                    Description = $"Такса поддръжка за {currentMonthName}",
                    DueDate = dueDate,
                    Type = FeeType.Maintenance,
                    ApartmentId = apt.Id,
                    DateFeeIncurred = DateTime.UtcNow
                };
                
                _context.Fees.Add(maintenanceFee);
            }

            if (apt.IdealParts > 0)
            {
                var repairFee = new MonthlyFee
                {
                    Amount = apt.IdealParts * 1.50m,
                    Description = $"Фонд ремонт за {currentMonthName}",
                    DueDate = dueDate,
                    Type = FeeType.RepairFund,
                    ApartmentId = apt.Id,
                    DateFeeIncurred = DateTime.UtcNow
                };

                _context.Fees.Add(repairFee);
            }
        }
        await _context.SaveChangesAsync(cancellationToken);
    }
}