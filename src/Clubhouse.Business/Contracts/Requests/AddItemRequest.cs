using System.ComponentModel.DataAnnotations;

namespace Clubhouse.Business.Contracts.Requests;

public class AddItemRequest
{
    [Required(AllowEmptyStrings = false, ErrorMessage = "Item name is required")]
    public string Name { get; set; }

    [Required(AllowEmptyStrings = false, ErrorMessage = "Item price is required")]
    public decimal Price { get; set; }
}