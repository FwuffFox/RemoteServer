using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using RemoteMessenger.Server.Models;
using RemoteMessenger.Server.Util;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
var connectionString = builder.Configuration.GetSection("Database").Value;
builder.Services.AddDbContext<MessengerContext>(op => op.UseNpgsql(connectionString));
builder.Services.AddSignalR();

// Initialize JwtTokenManager
JwtTokenManager.Initialize(
    secret: builder.Configuration.GetSection("Jwt:Secret").Value,
    expireDays: builder.Configuration.GetValue<int>("Jwt:ExpireDays"),
    issuer: builder.Configuration.GetSection("Jwt:Issuer").Value,
    audience: builder.Configuration.GetSection("Jwt:Audience").Value
    );
RSAEncryption.Initialize();

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
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
var logger = app.Services.GetService<ILogger<Program>>();
app.MapGet("/public_key", () => RSAEncryption.ServerPublicRSAKeyBase64);
app.MapGet("/encrypt/{encryptedString}",
    RSAEncryption.Decrypt_Base64);
app.MapGet("/validate_jwt/{jwt}",
    async (string jwt, HttpContext context) =>
    {
        var res = await JwtTokenManager.ValidateToken(jwt, context.GetRequestBaseUrl());
        return res.IsValid;
    });
app.MapGet("/check_auth", [Authorize](HttpContext context) => "Authenticated");
app.MapHub<GeneralChatHub>(GeneralChatHub.HubUrl);
app.Run();