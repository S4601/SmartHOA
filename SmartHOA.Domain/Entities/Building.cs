using SmartHOA.Domain.Common;

namespace SmartHOA.Domain.Entities;

public class Building : BaseEntity
{
    public required string Name { get; set; }
    public required string Address { get; set; }
    public required string City { get; set; }
    
    //Домакин
    public required Guid ManagerId { get; set; }
    public User? Manager { get; set; }

    public ICollection<Apartment> Apartments { get; set; } = new List<Apartment>();

}