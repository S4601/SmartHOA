using Microsoft.EntityFrameworkCore;
using SmartHOA.Domain.Entities;

namespace SmartHOA.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<User> Users { get; }
    DbSet<Building> Buildings { get; }
    DbSet<Apartment> Apartments { get; }
    DbSet<MonthlyFee> Fees { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}