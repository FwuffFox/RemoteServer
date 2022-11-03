using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace RemoteMessenger.Server.Util;

public static class JwtTokenManager
{
    private static string Secret { get; set; } = string.Empty;
    public static SymmetricSecurityKey? Key = null!;

    public static void Initialize(string secret)
    {
        Secret = secret;
        Key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Secret));
    }

    public static async Task<bool> ValidateToken(string token, string issuer)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var result = await tokenHandler.ValidateTokenAsync(token, new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            ValidateIssuer = true,
            ValidIssuer = issuer,
            IssuerSigningKey = Key,
            ValidateLifetime = true,
        });
        return result.IsValid;
    }
}