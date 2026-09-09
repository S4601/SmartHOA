using SmartHOA.Domain.Common;
using SmartHOA.Domain.Enums;

namespace SmartHOA.Domain.Entities;

public class MonthlyFee : BaseEntity
{
    public required decimal Amount { get; set; }
    public required string Description { get; set; }
    public required DateTime DueDate { get; set; }
    public required DateTime DateFeeIncurred { get; set; } = DateTime.UtcNow;
    public required FeeType Type { get; set; }

    public bool IsPaid { get; private set; } = false;
    public DateTime? PaidDate { get; private set; }
    
    public required Guid ApartmentId { get; set; }
    public Apartment? Apartment { get; set; }

    public void MarkAsPaid()
    {
        if (IsPaid)
            throw new InvalidOperationException("Cannot mark Paid as Paid");

        IsPaid = true;
        PaidDate = DateTime.UtcNow;
    }
}