using System.ComponentModel.DataAnnotations;

namespace Clubhouse.Business.Contracts.Requests;

public class UpdateItemRequest
{
    [Required(AllowEmptyStrings = false, ErrorMessage = "Item id is required")]
    public string Id { get; set; }
    public string? Name { get; set; }
    public decimal? Price { get; set; }
}