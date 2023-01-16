using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using Microsoft.AspNetCore.SignalR;
using RemoteMessenger.Server.Hubs;
using RemoteMessenger.Server.Services;
using RemoteMessenger.Server.Services.SignalR;
using RemoteMessenger.Server.Setup;
using RemoteMessenger.Server.Util;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers(op =>
{
    op.ModelMetadataDetailsProviders.Add(new SystemTextJsonValidationMetadataProvider());
});
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwagger();

// Initialize DataBase services
var inMemory = builder.Configuration.GetValue<bool>("InMemory");
if (inMemory) 
{
    builder.Services.AddDbContext<MessengerContext>(
        optionsAction: op => op.UseInMemoryDatabase(databaseName: "MessengerDb"),
        contextLifetime: ServiceLifetime.Singleton,
        optionsLifetime: ServiceLifetime.Singleton
    );
}
else
{
    var connectionString = builder.Configuration["Database"] ?? "";
    builder.Services.AddDbContext<MessengerContext>(
        optionsAction: op => op.UseNpgsql(connectionString),
        contextLifetime: ServiceLifetime.Singleton,
        optionsLifetime: ServiceLifetime.Singleton
    );
}

builder.Services.AddSingleton<UserService>();

builder.Services.AddSingleton<IUserIdProvider, JwtUniqueNameBasedProvider>();
builder.Services.AddSignalR();

// Initialize JwtTokenManager
JwtTokenManager.Initialize(
    secret: builder.Configuration["Jwt:Secret"],
    expireDays: builder.Configuration.GetValue<int>("Jwt:ExpireDays"),
    issuer: builder.Configuration["Jwt:Issuer"],
    audience: builder.Configuration["Jwt:Audience"]
    );

// RSAEncryption.Initialize();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    options.TokenValidationParameters = JwtTokenManager.TokenValidationParameters!;
});
builder.Services.AddAuthorization();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(op =>
    {
        op.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
        op.RoutePrefix = string.Empty;
    });
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
    
app.MapGet("/validate_jwt",
    async ([FromQuery] string jwt) =>
    {
        var res = await JwtTokenManager.ValidateToken(jwt);
        return res.IsValid;
    });

if (app.Environment.IsDevelopment())
{
    app.MapGet("/auth/check_auth", [Authorize] () => "Authenticated");
    app.MapGet("/auth/admin/check_auth", [Authorize(Roles = Roles.Admin)] () => "Authenticated as Admin");
}

app.MapHub<GeneralChatHub>(GeneralChatHub.HubUrl);
app.MapHub<DirectMessagesHub>(DirectMessagesHub.HubUrl);
app.Run();

// WarmUp database
var _ = app.Services.GetService<MessengerContext>()!.Users.FirstOrDefault();