namespace RemoteMessenger.Server.Startup;

public static class ConfigureDatabase
{
    public static WebApplicationBuilder UseDatabase(this WebApplicationBuilder builder)
    {
        var inMemory = builder.Configuration.GetValue<bool>("InMemory");
        if (inMemory) builder.UseInMemoryDatabase();
        else builder.UseRemoteDatabase();
        return builder;
    }

    private static WebApplicationBuilder UseInMemoryDatabase(this WebApplicationBuilder builder)
    {
        builder.Services.AddDbContext<MessengerContext>(
            optionsAction: op => op.UseInMemoryDatabase(databaseName: "MessengerDb"),
            contextLifetime: ServiceLifetime.Singleton,
            optionsLifetime: ServiceLifetime.Singleton
        );
        
        return builder;
    }

    private static WebApplicationBuilder UseRemoteDatabase(this WebApplicationBuilder builder)
    {
        var connectionString = builder.Configuration["Database"] ?? "";
        
        builder.Services.AddDbContext<MessengerContext>(
            optionsAction: op => op.UseNpgsql(connectionString),
            contextLifetime: ServiceLifetime.Singleton,
            optionsLifetime: ServiceLifetime.Singleton
        );

        return builder;
    }
}