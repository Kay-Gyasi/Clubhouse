using Clubhouse.Data.Entities;
using Clubhouse.Data.Repositories.Base;
using Clubhouse.Shared.Contracts;

namespace Clubhouse.Data.Repositories.Interfaces;

public interface IUserRepository : IRepository<User>
{
    (byte[] PasswordHash, byte[] PasswordKey)? CreatePassword(string password);
    Task<User?> Authenticate(LoginRequest request);
}