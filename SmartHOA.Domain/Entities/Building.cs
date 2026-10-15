using SmartHOA.Domain.Common;

namespace SmartHOA.Domain.Entities;

public class Building : BaseEntity
{
    public required string Name { get; set; }
    public required string Address { get; set; }
    public required string City { get; set; }
    
    public required int FloorCount { get; set; }
    public required int ApartmentCountPerFloor { get; set; }
    
    // Тарифи за изчисляване на месечни такси
    public decimal BaseOccupantFee { get; set; } = 15.00m;
    public decimal PetFee { get; set; } = 15.00m;
    public decimal RepairFundRatePerIdealPart { get; set; } = 1.50m;

    // Домакин / Управител
    public required Guid ManagerId { get; set; }
    public User? Manager { get; set; }

    public ICollection<Apartment> Apartments { get; set; } = new List<Apartment>();
}