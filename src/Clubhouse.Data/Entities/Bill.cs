using Clubhouse.Data.Entities.Base;

namespace Clubhouse.Data.Entities;

public class Bill : Entity
{
    public Bill(string userId)
    {
        UserId = userId;
        Status = BillStatus.Active;
    }

    public string UserId { get; set; }
    public User User { get; set; }
    public BillStatus Status { get; set; }
    public decimal TotalCost => _billEntries.Sum(x => x.Item.Price * x.Quantity);

    private readonly List<BillEntry> _billEntries = new List<BillEntry>();
    public IReadOnlyList<BillEntry> BillEntries => _billEntries.AsReadOnly();
}

public enum BillStatus
{
    Active,
    Paid
}