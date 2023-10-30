using Clubhouse.Data.Entities.Base;

namespace Clubhouse.Data.Entities;

public class User : Entity
{
    public string Username { get; set; }
    public string PhoneNumber { get; set; }
    public string? Email { get; set; }
    public byte[] Password { get; set; }
    public byte[]? PasswordKey { get; set; }
    public string? BillId { get; set; }
    public Bill? Bill { get; set; }

    private readonly List<Role> _roles = new List<Role>();
    public IReadOnlyList<Role> Roles => _roles.AsReadOnly();

    public User AddToRoles(IEnumerable<Role> roles)
    {
        _roles.AddRange(roles);
        return this;
    }
}