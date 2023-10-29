using Clubhouse.Data.Entities;
using Clubhouse.Data.Repositories.Base;
using Clubhouse.Data.Repositories.Interfaces;
using Microsoft.Extensions.Logging;
using System.Security.Cryptography;
using System.Text;
using Clubhouse.Shared.Contracts;
using Microsoft.EntityFrameworkCore;

namespace Clubhouse.Data.Repositories.Providers;

[Repository]
public class UserRepository : Repository<User>, IUserRepository
{
    private readonly ApplicationDbContext _dbContext;

    public UserRepository(ApplicationDbContext dbContext, 
        ILogger<UserRepository> logger) 
        : base(dbContext, logger)
    {
        _dbContext = dbContext;
    }

    public async Task<User?> Authenticate(LoginRequest request)
    {
        var user = await _dbContext.Users.FirstOrDefaultAsync(x => x.PhoneNumber == request.PhoneNumber);
        if (user is null) return null;

        var passwordIsInvalid = user?.PasswordKey == null ||
                                !MatchPasswordHash(request.Password, user.Password, user.PasswordKey);
        return passwordIsInvalid ? null : user;
    }

    public (byte[] PasswordHash, byte[] PasswordKey) CreatePassword(string password)
    {
        byte[] passwordHash, passwordKey;

        using (var hmac = new HMACSHA512())
        {
            passwordKey = hmac.Key;
            passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
        }
        return (passwordHash, passwordKey);
    }

    private static bool MatchPasswordHash(string passwordText, IReadOnlyList<byte> password, byte[] passwordKey)
    {
        using var hmac = new HMACSHA512(passwordKey);
        var passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(passwordText));
        return !passwordHash.Where((t, i) => t != password[i]).Any();
    }
}