using Clubhouse.Data.Entities.Base;
using Clubhouse.Data.Extensions;

namespace Clubhouse.Data.Entities;

public class User : Entity
{
    public User(string username, string phoneNumber)
    {
        Username = username;
        PhoneNumber = phoneNumber;
        Roles = (new List<string>
        {
            CommonConstants.UserRoles.Member
        }).Serialize();
    }

    public string Username { get; set; }
    public string PhoneNumber { get; set; }
    public string? Email { get; set; }
    public byte[]? Password { get; set; }
    public byte[]? PasswordHash { get; set; }
    public string Roles { get; set; }

    public User AddToRoles(IEnumerable<string> roles)
    {
        var userRoles = Roles.Deserialize<List<string>>() ?? new List<string>();
        userRoles.AddRange(roles);
        Roles = userRoles.Serialize();
        return this;
    }
}