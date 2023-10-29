using Clubhouse.Business.Options;
using Clubhouse.Data.Entities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Clubhouse.Data.Extensions;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Clubhouse.Business.Authentication;

public class JwtService : IJwtService
{
    private readonly BearerTokenConfig _jwtConfig;

    public JwtService(IOptions<BearerTokenConfig> jwtConfig)
    {
        _jwtConfig = jwtConfig.Value;
    }

    public AuthToken GetToken(User user)
    {
        var identity = new ClaimsIdentity(JwtBearerDefaults.AuthenticationScheme);

        identity.AddClaims(new[]
        {
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(JwtRegisteredClaimNames.Sub, user.Username ?? ""),
            new Claim(JwtRegisteredClaimNames.Email, user.Email ?? ""),
            new Claim(ClaimTypes.MobilePhone, user.PhoneNumber ?? ""),
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

        var signingKey = _jwtConfig.SigningKey ?? throw new InvalidOperationException("Signing key is not specified");
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
}

public interface IJwtService
{
    AuthToken GetToken(User user);
}

public record AuthToken(string Token);