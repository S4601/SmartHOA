using SmartHOA.Domain.Common;

namespace SmartHOA.Domain.Entities;

public class User : BaseEntity
{
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public required string Email { get; set; }
    public string? PhoneNumber { get; set; }
    
    public required string PasswordHash { get; set; }

    public ICollection<Building> ManagedBuildings { get; set; } = new List<Building>();
    public ICollection<Apartment> OwnedApartments { get; set; } = new List<Apartment>();
}