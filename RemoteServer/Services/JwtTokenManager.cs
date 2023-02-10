using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using RemoteServer.Util;

namespace RemoteServer.Services;

public class JwtTokenManager
{
    private static SymmetricSecurityKey? _key;

    private readonly string _audience;

    private readonly int _expireDays;

    private readonly string _issuer;

    private readonly TokenValidationParameters? _tokenValidationParameters;

    public JwtTokenManager(IConfiguration configuration)
    {
        var secret = configuration.GetValue<string>("Secret");
        Secret = secret ?? throw new Exception("Server Secret is not set.");
        _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Secret));

        _expireDays = configuration.GetValue<int>("Jwt:ExpireDays");
        _audience = configuration.GetValue<string>("Jwt:Audience") ?? "";
        _issuer = configuration.GetValue<string>("Jwt:Issuer") ?? "";

        _tokenValidationParameters = configuration.CreateTokenValidationParameters();
    }

    public string Secret { get; }

    public async Task<TokenValidationResult> ValidateToken(string token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var result = await tokenHandler.ValidateTokenAsync(token, _tokenValidationParameters);
        return result;
    }

    public string IssueToken(User user)
    {
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(JwtRegisteredClaimNames.Iat, EpochTime.GetIntDate(DateTime.Now).ToString(),
                ClaimValueTypes.Integer64),
            new(JwtRegisteredClaimNames.UniqueName, user.Username),
            new(JwtRegisteredClaimNames.Name, user.FullName),
            new("roles", user.Role)
        };
        var signingCredentials = new SigningCredentials(_key, SecurityAlgorithms.HmacSha512Signature);
        var token = new JwtSecurityToken(
            _issuer,
            _audience,
            claims,
            expires: DateTime.UtcNow.AddDays(_expireDays),
            signingCredentials: signingCredentials);
        var jwt = new JwtSecurityTokenHandler().WriteToken(token);
        return jwt;
    }
}