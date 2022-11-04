using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;

namespace RemoteMessenger.Server.Util;

public static class JwtTokenManager
{
    public static SymmetricSecurityKey? Key;
    private static string Secret { get; set; } = string.Empty;

    public static void Initialize(string secret)
    {
        Secret = secret;
        Key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Secret));
    }

    public static async Task<TokenValidationResult> ValidateToken(string token, string issuer)
    {
        var tokenHandler = new JwtSecurityTokenHandler();

        var result = await tokenHandler.ValidateTokenAsync(token, new TokenValidationParameters
        {
            IssuerSigningKey = Key,
            ValidateAudience = false,
            ValidateIssuer = true,
            ValidIssuer = issuer,
            ValidateLifetime = true
        });

        return result;
    }
}