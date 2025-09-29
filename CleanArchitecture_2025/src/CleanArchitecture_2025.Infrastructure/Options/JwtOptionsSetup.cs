using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace CleanArchitecture_2025.Infrastructure.Options;
public sealed class JwtOptionsSetup : IPostConfigureOptions<JwtBearerOptions>
{
    private readonly IOptions<JwtOptions> _jwtOptions;

    public JwtOptionsSetup(IOptions<JwtOptions> options)
    {
        _jwtOptions = options;
    }

    public void PostConfigure(string? name, JwtBearerOptions options)
    {
        options.TokenValidationParameters.ValidateIssuer = true;
        options.TokenValidationParameters.ValidateAudience = true;
        options.TokenValidationParameters.ValidateLifetime = true;
        options.TokenValidationParameters.ValidateIssuerSigningKey = true;
        options.TokenValidationParameters.ValidIssuer = _jwtOptions.Value.Issuer;
        options.TokenValidationParameters.ValidAudience = _jwtOptions.Value.Audience;
        options.TokenValidationParameters.IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.Value.SecretKey));
    }
}
