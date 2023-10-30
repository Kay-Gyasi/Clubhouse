using Clubhouse.Data.Entities.Base;

namespace Clubhouse.Data.Entities;

public class Payment : Entity
{
    public string? BillId { get; set; }
    public Bill? Bill { get; set; }
    public decimal Amount { get; set; }
}