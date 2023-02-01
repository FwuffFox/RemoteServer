using Microsoft.AspNetCore.SignalR;
using RemoteServer.Util;

namespace RemoteServer.Services.SignalR;

public class JwtUniqueNameBasedProvider : IUserIdProvider
{
    public string GetUserId(HubConnectionContext connection)
    {
        return connection.User.GetUniqueName();
    }
}