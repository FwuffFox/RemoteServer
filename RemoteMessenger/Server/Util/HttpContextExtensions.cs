using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace RemoteMessenger.Server.Util;

public static partial class Extensions
{
    public static string GetRequestBaseUrl(this HttpContext context)
    {
        return $"{context.Request.Scheme}://{context.Request.Host}";
    }

    public static string GetUniqueName(this ClaimsPrincipal user)
    {
        return user.FindFirst(JwtRegisteredClaimNames.UniqueName)?.Value!;
    }
    
    public static async Task<string> GetUniqueNameAsync(this ClaimsPrincipal user)
    {
        return await Task.Run(() => user.FindFirst(JwtRegisteredClaimNames.UniqueName)?.Value!);
    }
}