namespace RemoteMessenger.Server.Startup;

public static class ConfigureDatabase
{
    public static WebApplicationBuilder UseDatabase(this WebApplicationBuilder builder)
    {
        var useSqlite = builder.Configuration.GetValue<bool>("UseSqlite");
        if (useSqlite) builder.UseSQLiteDatabase();
        else builder.UseRemoteDatabase();
        return builder;
    }

    private static WebApplicationBuilder UseRemoteDatabase(this WebApplicationBuilder builder)
    {
        var connectionString = builder.Configuration.GetConnectionString("MessengerContext");
        
        builder.Services.AddDbContext<MessengerContext>(
            optionsAction: op => op.UseNpgsql(connectionString),
            contextLifetime: ServiceLifetime.Singleton,
            optionsLifetime: ServiceLifetime.Singleton
        );

        return builder;
    }

    private static WebApplicationBuilder UseSQLiteDatabase(this WebApplicationBuilder builder)
    {
        builder.Services.AddDbContext<MessengerContext>(
            optionsAction: op => op.UseSqlite("Data Source=database.db"),
            contextLifetime: ServiceLifetime.Singleton,
            optionsLifetime: ServiceLifetime.Singleton
        );
        
        return builder;
    }
}