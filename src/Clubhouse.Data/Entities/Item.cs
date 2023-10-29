using Clubhouse.Data.Entities.Base;

namespace Clubhouse.Data.Entities;

public class Item : Entity
{
    public Item(string name, decimal price = 0)
    {
        Name = name;
        Price = price;
    }

    public string Name { get; set; }
    public decimal Price { get; set; }

    private readonly List<BillEntry> _billEntries = new List<BillEntry>();
    public IReadOnlyList<BillEntry> BillEntries => _billEntries.AsReadOnly();
}