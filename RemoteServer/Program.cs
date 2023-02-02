using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using Microsoft.AspNetCore.SignalR;
using RemoteServer.Hubs;
using RemoteServer.Models.DbContexts;
using RemoteServer.Models.Shared;
using RemoteServer.Repositories;
using RemoteServer.Services;
using RemoteServer.Services.SignalR;
using RemoteServer.Startup;
using RemoteServer.Util;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers(op =>
{
    op.ModelMetadataDetailsProviders.Add(new SystemTextJsonValidationMetadataProvider());
});
builder.Services.AddSingleton<JwtTokenManager>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwagger();


builder.UseDatabase();
builder.Services.AddSingleton<UserRepository>();

builder.Services.AddSingleton<IUserIdProvider, JwtUniqueNameBasedProvider>();
builder.Services.AddSignalR();

builder.UseJwtAuthentication();

builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.SuppressModelStateInvalidFilter = true;
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<MessengerContext>();
    app.Logger
        .LogInformation($"Database created {context.Database.EnsureCreated()}");
    var userService = scope.ServiceProvider.GetRequiredService<UserRepository>();
    if (!await userService.IsUsernameTaken("@admin"))
    {
        var user = new User()
        {
            Username = "@admin",
            FullName = "Admin Adminovich",
            JobTitle = "Admin"
        };
        var password = builder.Configuration.GetValue<string>("AdminPassword") ?? "";
        await user.SetPassword(password);
        await userService.CreateUserAsync(user);
        app.Logger
            .LogInformation($"Admin created {user.Username}: {password}");
    }
    
}

app.UseSwaggerUI();

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
    
app.MapGet("/validate_jwt",
    async ([FromQuery] string jwt, JwtTokenManager jwtTokenManager) =>
    {
        var res = await jwtTokenManager.ValidateToken(jwt);
        return res.IsValid;
    });

if (app.Environment.IsDevelopment())
{
    app.MapGet("/auth/check_auth", [Authorize] () => "Authenticated");
    app.MapGet("/auth/admin/check_auth", [Authorize(Roles = Roles.Admin)] () => "Authenticated as Admin");
}

app.MapHub<GeneralChatHub>(GeneralChatHub.HubUrl);
app.MapHub<ChatHub>(ChatHub.HubUrl);

app.Run();

