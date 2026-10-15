using MediatR;
using Microsoft.EntityFrameworkCore;
using SmartHOA.Application.Common.Interfaces;
using SmartHOA.Domain.Entities;
using SmartHOA.Domain.Enums;

namespace SmartHOA.Application.Fees.Commands.GenerateMonthlyFees;

public record GenerateMonthlyFeesCommand(int? Year = null, int? Month = null) : IRequest;

public class GenerateMonthlyFeesCommandHandler : IRequestHandler<GenerateMonthlyFeesCommand>
{
    private readonly IApplicationDbContext _context;
    
    public GenerateMonthlyFeesCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task Handle(GenerateMonthlyFeesCommand request, CancellationToken cancellationToken)
    {
        var now = DateTime.UtcNow;
        var year = request.Year ?? now.Year;
        var month = request.Month ?? now.Month;

        var monthStart = new DateTime(year, month, 1, 0, 0, 0, DateTimeKind.Utc);
        var monthEnd = monthStart.AddMonths(1);
        var dueDate = new DateTime(year, month, Math.Min(15, DateTime.DaysInMonth(year, month)), 23, 59, 59, DateTimeKind.Utc);
        var currentMonthName = monthStart.ToString("MMMM yyyy");

        // Извличаме вече съществуващите такси за този период, за да гарантираме идемпотентност (без дублиране)
        var existingFees = await _context.Fees
            .Where(f => f.DueDate >= monthStart && f.DueDate < monthEnd)
            .Select(f => new { f.ApartmentId, f.Type })
            .ToListAsync(cancellationToken);

        var existingSet = new HashSet<(Guid ApartmentId, FeeType Type)>(
            existingFees.Select(f => (f.ApartmentId, f.Type)));

        // Извличаме сградите с техните тарифи и апартаменти
        var buildings = await _context.Buildings
            .Include(b => b.Apartments)
            .ToListAsync(cancellationToken);

        foreach (var building in buildings)
        {
            foreach (var apt in building.Apartments)
            {
                // 1. Такса поддръжка (базирана на обитатели и домашни любимци по тарифа на сградата)
                if (!existingSet.Contains((apt.Id, FeeType.Maintenance)))
                {
                    decimal occupantsTotal = apt.OccupantsCount * building.BaseOccupantFee;
                    decimal petsTotal = apt.HasPets ? building.PetFee : 0m;
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
                            DateFeeIncurred = now
                        };
                        
                        _context.Fees.Add(maintenanceFee);
                        existingSet.Add((apt.Id, FeeType.Maintenance));
                    }
                }

                // 2. Фонд "Ремонт" (базиран на процент идеални части по тарифа на сградата)
                if (!existingSet.Contains((apt.Id, FeeType.RepairFund)) && apt.IdealParts > 0)
                {
                    decimal repairAmount = apt.IdealParts * building.RepairFundRatePerIdealPart;

                    if (repairAmount > 0)
                    {
                        var repairFee = new MonthlyFee
                        {
                            Amount = repairAmount,
                            Description = $"Фонд ремонт за {currentMonthName}",
                            DueDate = dueDate,
                            Type = FeeType.RepairFund,
                            ApartmentId = apt.Id,
                            DateFeeIncurred = now
                        };

                        _context.Fees.Add(repairFee);
                        existingSet.Add((apt.Id, FeeType.RepairFund));
                    }
                }
            }
        }

        await _context.SaveChangesAsync(cancellationToken);
    }
}