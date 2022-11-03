namespace RemoteMessenger.Server.Util;

public static class HttpContextExtensions
{
    public static string GetRequestBaseUrl(this HttpContext context)
    {
        return $"{context.Request.Scheme}://{context.Request.Host}";
    }
}