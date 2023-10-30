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
    private readonly ILogger<UserRepository> _logger;

    public UserRepository(ApplicationDbContext dbContext, 
        ILogger<UserRepository> logger) 
        : base(dbContext, logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task<User?> Authenticate(LoginRequest request)
    {
        try
        {
            var user = await _dbContext.Users.FirstOrDefaultAsync(x => x.PhoneNumber == request.PhoneNumber);
            if (user is null) return null;

            var passwordIsInvalid = user?.PasswordKey == null ||
                                    !MatchPasswordHash(request.Password, user.Password, user.PasswordKey);
            return passwordIsInvalid ? null : user;
        }
        catch (Exception a)
        {
            _logger.LogError(a, "Error while checking login request. {LoginRequest}", request);
            return null;
        }
    }

    public (byte[] PasswordHash, byte[] PasswordKey)? CreatePassword(string password)
    {
        try
        {
            byte[] passwordHash, passwordKey;

            using (var hmac = new HMACSHA512())
            {
                passwordKey = hmac.Key;
                passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            }

            return (passwordHash, passwordKey);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error while creating password hash and key");
            return null;
        }
    }

    private static bool MatchPasswordHash(string passwordText, IReadOnlyList<byte> password, byte[] passwordKey)
    {
        using var hmac = new HMACSHA512(passwordKey);
        var passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(passwordText));
        return !passwordHash.Where((t, i) => t != password[i]).Any();
    }
}