using Clubhouse.Business.Options;
using Clubhouse.Data.Entities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.Extensions.Logging;

namespace Clubhouse.Business.Authentication;

public class JwtService : IJwtService
{
    private readonly ILogger<JwtService> _logger;
    private readonly BearerTokenConfig _jwtConfig;

    public JwtService(IOptions<BearerTokenConfig> jwtConfig,
        ILogger<JwtService> logger)
    {
        _logger = logger;
        _jwtConfig = jwtConfig.Value;
    }

    public AuthToken? GetToken(User user)
    {
        try
        {
            var identity = new ClaimsIdentity(JwtBearerDefaults.AuthenticationScheme);

            identity.AddClaims(new[]
            {
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.NameId, user.Id ?? "")
            });

            var roles = user.Roles.Select(x => x.Name);
            foreach (var role in roles)
            {
                identity.AddClaim(new Claim(ClaimTypes.Role, role));
            }

            var id = Guid.NewGuid().ToString().GetHashCode().ToString("x", CultureInfo.InvariantCulture);
            identity.AddClaim(new Claim(JwtRegisteredClaimNames.Jti, id));

            identity.AddClaim(new Claim(JwtRegisteredClaimNames.Aud, _jwtConfig.Audience));

            var handler = new JwtSecurityTokenHandler();

            var signingKey = _jwtConfig.SigningKey ??
                             throw new InvalidOperationException("Signing key is not specified");
            var signingKeyBytes = Encoding.UTF8.GetBytes(signingKey);
            var jwtSigningCredentials = new SigningCredentials(new SymmetricSecurityKey(signingKeyBytes),
                SecurityAlgorithms.HmacSha256Signature);

            var jwtToken = handler.CreateJwtSecurityToken(
                _jwtConfig.Issuer,
                audience: _jwtConfig.Audience,
                identity,
                notBefore: DateTime.UtcNow,
                expires: DateTime.UtcNow.AddMinutes(30),
                issuedAt: DateTime.UtcNow,
                jwtSigningCredentials);

            return new AuthToken(handler.WriteToken(jwtToken));
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error while generating jwt");
            return null;
        }
    }
}

public interface IJwtService
{
    AuthToken? GetToken(User user);
}

public record AuthToken(string Token);