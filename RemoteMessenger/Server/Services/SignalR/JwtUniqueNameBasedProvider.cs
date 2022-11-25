using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.SignalR;

namespace RemoteMessenger.Server.Services.SignalR;

public class JwtUniqueNameBasedProvider : IUserIdProvider
{
    public string GetUserId(HubConnectionContext connection)
    {
        return connection.User?.FindFirst(JwtRegisteredClaimNames.UniqueName)?.Value!;
    }
}