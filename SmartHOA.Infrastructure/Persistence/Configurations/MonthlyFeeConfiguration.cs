using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartHOA.Domain.Entities;
using SmartHOA.Domain.Enums;

namespace SmartHOA.Infrastructure.Persistence.Configurations;

public class MonthlyFeeConfiguration : IEntityTypeConfiguration<MonthlyFee>
{
    public void Configure(EntityTypeBuilder<MonthlyFee> builder)
    {
        builder.ToTable("MonthlyFees");

        builder.HasKey(f => f.Id);
        
        builder.Property(f => f.Amount)
            .HasPrecision(18, 2);

        builder.Property(f => f.Description)
            .IsRequired()
            .HasMaxLength(200);
        
        builder.Property(f => f.Type)
            .HasConversion<string>()
            .IsRequired();

       
        builder.HasOne(f => f.Apartment)
            .WithMany(a => a.Fees)
            .HasForeignKey(f => f.ApartmentId)
            .OnDelete(DeleteBehavior.Cascade);
    }
    
}