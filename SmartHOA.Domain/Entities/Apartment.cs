using SmartHOA.Domain.Common;

namespace SmartHOA.Domain.Entities;

public class Apartment : BaseEntity
{
    public required string Number { get; set; }
    public required int Floor { get; set; }
    
    //Бизнес данни за изчисление на таксите
    public int OccupantsCount { get; set; } = 0;
    public bool HasPets { get; set; } = false;  // Има ли куче (често се таксува като 1 обитател)
    public decimal IdealParts { get; set; }     // Процент идеални части (за фонд ремонт)
    
    public required Guid BuildingId { get; set; }
    public Building? Building { get; set; }
    
    public Guid? OwnerId { get; set; }
    public User? Owner { get; set; }
    
    public ICollection<MonthlyFee> Fees { get; set; } = new List<MonthlyFee>();
}