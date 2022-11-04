using RemoteMessenger.Server;
using RemoteMessenger.Server.Models;
using RemoteMessenger.Server.Util;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
var connectionString = builder.Configuration.GetConnectionString("Database") ?? "Data Source=Database.db";
builder.Services.AddDbContext<MessengerContext>(op => op.UseSqlite(connectionString));
builder.Services.AddSignalR();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapControllers();
app.MapGet("/public_key", () => RSAEncryption.ServerPublicRSAKeyBase64);
app.MapGet("/encrypt/{encryptedString}",
    RSAEncryption.Decrypt_Base64);
app.MapGet("/validate_jwt/{jwt}",
    async (string jwt, HttpContext context) =>
    {
        var res = await JwtTokenManager.ValidateToken(jwt, context.GetRequestBaseUrl());
        return res.IsValid;
    });
app.MapHub<GeneralChatHub>(GeneralChatHub.HubUrl);

RSAEncryption.Initialize();
JwtTokenManager.Initialize(builder.Configuration.GetSection("AppSettings:Token").Value);
app.Run();