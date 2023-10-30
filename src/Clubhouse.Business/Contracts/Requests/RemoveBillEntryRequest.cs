using System.ComponentModel.DataAnnotations;

namespace Clubhouse.Business.Contracts.Requests;

public class RemoveBillEntryRequest
{
    [Required(AllowEmptyStrings = false, ErrorMessage = "user not specified")]
    public string UserId { get; set; }

    [Required(ErrorMessage = "Not entries to add")]
    public BillEntryDto Entry { get; set; }
}