using Microsoft.IdentityModel.Tokens;

namespace RemoteServer.Util;

public static class JwtConfigurationCreator
{
    public static TokenValidationParameters CreateTokenValidationParameters
        (this IConfiguration configuration)
    {
        var secret = configuration.GetValue<string>("Secret")
                     ?? throw new Exception("Server Secret is not set.");
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));

        var audience = configuration.GetValue<string>("Jwt:Audience") ?? "";
        var issuer = configuration.GetValue<string>("Jwt:Issuer") ?? "";

        return new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = key,
            ValidateAudience = !audience.IsNullOrEmpty(),
            ValidAudience = audience,
            ValidateIssuer = !issuer.IsNullOrEmpty(),
            ValidIssuer = issuer,
            ValidateLifetime = true,
            ValidAlgorithms = new[] { SecurityAlgorithms.HmacSha512Signature }
        };
    }
}