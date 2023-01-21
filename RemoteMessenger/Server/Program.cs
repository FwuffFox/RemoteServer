using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using Microsoft.AspNetCore.SignalR;
using RemoteMessenger.Server.Hubs;
using RemoteMessenger.Server.Services;
using RemoteMessenger.Server.Services.SignalR;
using RemoteMessenger.Server.Startup;
using RemoteMessenger.Server.Util;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers(op =>
{
    op.ModelMetadataDetailsProviders.Add(new SystemTextJsonValidationMetadataProvider());
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwagger();

builder.UseDatabase();

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

builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.SuppressModelStateInvalidFilter = true;
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<MessengerContext>();
    scope.ServiceProvider.GetRequiredService<ILogger>()
        .LogInformation($"Database created {context.Database.EnsureCreated()}");
}

app.UseSwaggerUI();

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

