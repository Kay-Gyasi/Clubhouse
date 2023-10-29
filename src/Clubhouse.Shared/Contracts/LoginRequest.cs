using System.ComponentModel.DataAnnotations;

namespace Clubhouse.Shared.Contracts;

public class LoginRequest
{
    [Required(AllowEmptyStrings = false, ErrorMessage = "PhoneNumber is required")]
    public string PhoneNumber { get; set; }

    [Required(AllowEmptyStrings = false, ErrorMessage = "Password is required")]
    public string Password { get; set; }
}