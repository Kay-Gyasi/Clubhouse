using Clubhouse.Data.Entities.Base;

namespace Clubhouse.Data.Entities;

public class Role : Entity
{
    public Role(string name)
    {
        Name = name;
    }

    public string Name { get; set; }

    private readonly List<User> _users = new List<User>();
    public IReadOnlyList<User> Users => _users.AsReadOnly();
}