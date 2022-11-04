using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using RemoteMessenger.Server.Models;

namespace RemoteMessenger.Server.Util;

// TODO: REMAKE FOR DEPENDENCY INJECTION!!!!!
public static class JwtTokenManager
{
    private static SymmetricSecurityKey? _key;
    private static string Secret { get; set; } = string.Empty;

    private static int ExpireDays { get; set; }

    private static string Issuer { get; set; } = string.Empty;

    private static string Audience { get; set; } = string.Empty;

    public static TokenValidationParameters? TokenValidationParameters;

    public static void Initialize(string secret, int expireDays, string issuer, string audience)
    {
        Secret = secret;
        _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Secret));
        ExpireDays = expireDays;
        Issuer = issuer;
        Audience = audience;
        TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = _key,
            ValidateAudience = true,
            ValidAudience = Audience,
            ValidateIssuer = true,
            ValidIssuer = Issuer,
            ValidateLifetime = true,
            ValidAlgorithms = new [] {SecurityAlgorithms.HmacSha512Signature}
        };
    }

    public static async Task<TokenValidationResult> ValidateToken(string token, string issuer)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var result = await tokenHandler.ValidateTokenAsync(token, TokenValidationParameters);
        return result;
    }

    public static async Task<string> IssueToken(User user)
    {
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.Ticks.ToString()),
            new(JwtRegisteredClaimNames.UniqueName, user.Username),
            new(JwtRegisteredClaimNames.Name, user.FullName),
            new(JwtRegisteredClaimNames.Gender, user.Gender),
            new(JwtRegisteredClaimNames.Birthdate, user.DateOfBirth),
            new(ClaimsIdentity.DefaultRoleClaimType, user.Role)
        };
        var signingCredentials = new SigningCredentials(_key, SecurityAlgorithms.HmacSha512Signature);
        var token = new JwtSecurityToken(
            issuer: Issuer,
            audience: Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddDays(ExpireDays),
            signingCredentials: signingCredentials);
        var jwt = new JwtSecurityTokenHandler().WriteToken(token);
        return jwt;
    }
}