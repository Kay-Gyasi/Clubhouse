using Clubhouse.Data.Entities.Base;

namespace Clubhouse.Data.Entities;

public class Bill : Entity
{
    public required string UserId { get; set; }
    public User User { get; set; }
    public decimal TotalCost => _billEntries.Sum(x => x.Item.Price * x.Quantity);
    public decimal TotalPaid => _payments.Sum(x => x.Amount);
    public bool IsOwing => TotalCost > TotalPaid;

    private readonly List<BillEntry> _billEntries = new List<BillEntry>();
    public IReadOnlyList<BillEntry> BillEntries => _billEntries.AsReadOnly();

    private readonly List<Payment> _payments = new List<Payment>();
    public IReadOnlyList<Payment> Payments => _payments.AsReadOnly();

    public Bill AddEntries(IEnumerable<BillEntry> entries)
    {
        foreach (var billEntry in entries)
        {
            if (_billEntries.Any(x => x.ItemId == billEntry.ItemId))
            {
                var entry = _billEntries.First(x => x.ItemId == billEntry.ItemId);
                entry.Quantity += billEntry.Quantity;
                continue;
            }

            _billEntries.Add(billEntry);
        }

        return this;
    }

    public Bill RemoveEntry(BillEntry entry)
    {
        if (_billEntries.All(x => x.ItemId != entry.ItemId)) return this;
        
        var old = _billEntries.First(x => x.ItemId == entry.ItemId);
        old.Quantity -= entry.Quantity;
        if (old.Quantity <= 0) _billEntries.Remove(old);

        return this;
    }
}

public enum BillStatus
{
    Active,
    Paid
}