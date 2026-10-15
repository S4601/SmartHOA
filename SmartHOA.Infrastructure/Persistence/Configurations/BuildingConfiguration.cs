using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartHOA.Domain.Entities;

namespace SmartHOA.Infrastructure.Persistence.Configurations;

public class BuildingConfiguration : IEntityTypeConfiguration<Building>
{
    public void Configure(EntityTypeBuilder<Building> builder)
    {
        builder.ToTable("Buildings");

        builder.HasKey(b => b.Id);

        builder.Property(b => b.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(b => b.Address)
            .IsRequired()
            .HasMaxLength(250);

        builder.Property(b => b.BaseOccupantFee)
            .HasPrecision(18, 2)
            .HasDefaultValue(15.00m);

        builder.Property(b => b.PetFee)
            .HasPrecision(18, 2)
            .HasDefaultValue(15.00m);

        builder.Property(b => b.RepairFundRatePerIdealPart)
            .HasPrecision(18, 2)
            .HasDefaultValue(1.50m);

        // Конфигуриране на релацията (Един Домоуправител има Много Сгради)
        builder.HasOne(b => b.Manager)
            .WithMany(u => u.ManagedBuildings)
            .HasForeignKey(b => b.ManagerId)
            .OnDelete(DeleteBehavior.Restrict); // Предпазва от изтриване на потребител, ако управлява сграда
    }
}