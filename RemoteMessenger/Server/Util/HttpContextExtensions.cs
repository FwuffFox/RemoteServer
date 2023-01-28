using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace RemoteMessenger.Server.Util;

public static class Extensions
{
    public static string GetUniqueName(this ClaimsPrincipal user)
    {
        return user.FindFirst(JwtRegisteredClaimNames.UniqueName)?.Value!;
    }
    
    public static async Task<string> GetUniqueNameAsync(this ClaimsPrincipal user)
    {
        return await Task.Run(() =>
        {
            var res = user.FindFirstValue(JwtRegisteredClaimNames.UniqueName);
            res ??= user.FindFirstValue(ClaimTypes.Name);
            return res!;
        });
    }
}