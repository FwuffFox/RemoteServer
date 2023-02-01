using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using RemoteServer.Util;

namespace RemoteServer.Services;

public class JwtTokenManager
{
    private static SymmetricSecurityKey? _key;

    private readonly string _secret;

    public string Secret => _secret;

    private int _expireDays;

    private string _issuer;

    private string _audience;

    private TokenValidationParameters? _tokenValidationParameters;

    public JwtTokenManager(IConfiguration configuration)
    {
        var secret = configuration.GetValue<string>("Secret");
        _secret = secret ?? throw new Exception("Server Secret is not set.");
        _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secret));

        _expireDays = configuration.GetValue<int>("Jwt:ExpireDays");
        _audience = configuration.GetValue<string>("Jwt:Audience") ?? "";
        _issuer = configuration.GetValue<string>("Jwt:Issuer") ?? "";

        _tokenValidationParameters = configuration.CreateTokenValidationParameters();
    }
    
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
            issuer: _issuer,
            audience: _audience,
            claims: claims,
            expires: DateTime.UtcNow.AddDays(_expireDays),
            signingCredentials: signingCredentials);
        var jwt = new JwtSecurityTokenHandler().WriteToken(token);
        return jwt;
    }
}