using Microsoft.AspNetCore.SignalR;
using RemoteMessenger.Server.Util;

namespace RemoteMessenger.Server.Services.SignalR;

public class JwtUniqueNameBasedProvider : IUserIdProvider
{
    public string GetUserId(HubConnectionContext connection)
    {
        return connection.User.GetUniqueName();
    }
}