using Microsoft.EntityFrameworkCore;
using SmartHOA.Application.Common.Interfaces;
using SmartHOA.Domain.Entities;

namespace SmartHOA.Infrastructure.Persistence;

public class ApplicationDbContext : DbContext,  IApplicationDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) 
        : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<Building> Buildings => Set<Building>();
    public DbSet<Apartment> Apartments => Set<Apartment>();
    public DbSet<MonthlyFee> Fees => Set<MonthlyFee>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }
}