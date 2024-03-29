using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace RemoteServer.Util;

public static class Extensions
{
    public static string GetUniqueName(this ClaimsPrincipal user)
    {
        var res = user.FindFirstValue(JwtRegisteredClaimNames.UniqueName);
        res ??= user.FindFirstValue(ClaimTypes.Name);
        return res!;
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