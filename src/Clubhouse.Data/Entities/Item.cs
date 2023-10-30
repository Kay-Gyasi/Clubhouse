using Clubhouse.Data.Entities.Base;

namespace Clubhouse.Data.Entities;

public class Item : Entity
{
    public required string Name { get; set; }
    public required decimal Price { get; set; }

    private readonly List<BillEntry> _billEntries = new List<BillEntry>();
    public IReadOnlyList<BillEntry> BillEntries => _billEntries.AsReadOnly();
}