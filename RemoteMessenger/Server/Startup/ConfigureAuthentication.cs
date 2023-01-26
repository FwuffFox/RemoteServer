using Microsoft.AspNetCore.Authentication.JwtBearer;
using RemoteMessenger.Server.Util;

namespace RemoteMessenger.Server.Startup;

public static class ConfigureAuthentication
{
    public static WebApplicationBuilder UseJwtAuthentication(this WebApplicationBuilder builder)
    {
        builder.Services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(options =>
        {
            options.TokenValidationParameters =
                builder.Configuration.CreateTokenValidationParameters();
        
            options.Events = new JwtBearerEvents
            {
                OnMessageReceived = (context) =>
                {
                    var accessToken = context.Request.Query["access_token"];
        
                    var path = context.HttpContext.Request.Path;
                    if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/hubs"))
                    {
                        context.Token = accessToken;
                    }
                    return Task.CompletedTask;
                }
            };
        });
        builder.Services.AddAuthorization();
        
        return builder;
    }
}