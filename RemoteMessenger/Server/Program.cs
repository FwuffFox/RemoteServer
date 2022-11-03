using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using RemoteMessenger.Server;
using RemoteMessenger.Server.Models;

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
app.MapHub<GeneralChatHub>(GeneralChatHub.HubUrl);

RSAEncryption.Initialize();

app.Run();
