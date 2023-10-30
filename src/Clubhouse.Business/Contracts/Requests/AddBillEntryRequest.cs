using System.ComponentModel.DataAnnotations;

namespace Clubhouse.Business.Contracts.Requests;

public class AddBillEntryRequest
{
    [Required(AllowEmptyStrings = false, ErrorMessage = "user not specified")]
    public string UserId { get; set; }

    [Required(ErrorMessage = "Not entries to add")]
    public IEnumerable<BillEntryDto> Entries { get; set; }
}

public class BillEntryDto
{
    public string ItemId { get; set; }
    public int Quantity { get; set; }
}