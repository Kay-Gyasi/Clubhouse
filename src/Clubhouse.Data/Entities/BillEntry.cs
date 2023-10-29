using Clubhouse.Data.Entities.Base;

namespace Clubhouse.Data.Entities;

public class BillEntry : Entity
{
    public BillEntry(string itemId, string billId, int quantity)
    {
        ItemId = itemId;
        Quantity = quantity;
        BillId = billId;
    }

    public string ItemId { get; set; }
    public Item Item { get; set; }
    public string BillId { get; set; }
    public Bill Bill { get; set; }
    public int Quantity { get; set; }
}