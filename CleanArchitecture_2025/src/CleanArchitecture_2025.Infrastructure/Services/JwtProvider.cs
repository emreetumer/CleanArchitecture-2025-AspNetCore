using CleanArchitecture_2025.Application.Services;
using CleanArchitecture_2025.Domain.Users;
using CleanArchitecture_2025.Infrastructure.Options;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace CleanArchitecture_2025.Infrastructure.Services;
internal sealed class JwtProvider : IJwtProvider
{
    private readonly IOptions<JwtOptions> _options;
    public JwtProvider(IOptions<JwtOptions> options)
    {
        _options = options;
    }
    public Task<string> CreateTokenAsync(AppUser user, CancellationToken cancellationToken = default)
    {
        List<Claim> claims = new()
        {
            new Claim("user-id",user.Id.ToString())
        };

        var expires = DateTime.Now.AddDays(1);

        SymmetricSecurityKey securityKey = new(Encoding.UTF8.GetBytes(_options.Value.SecretKey));
        SigningCredentials signingCredentials = new(securityKey, SecurityAlgorithms.HmacSha512);

        JwtSecurityToken securityToken = new(
            issuer: _options.Value.Issuer,
            audience: _options.Value.Audience,
            claims: claims,
            notBefore: DateTime.Now,
            expires: expires,
            signingCredentials: signingCredentials);
        JwtSecurityTokenHandler handler = new();
        string token = handler.WriteToken(securityToken);
        return Task.FromResult(token);
    }
}
