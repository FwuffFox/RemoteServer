using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using RemoteMessenger.Server.Services;
using RemoteMessenger.Server.Util;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = @"JWT Authorization header using the Bearer scheme. </br> 
                      Enter 'Bearer' [space] and then your token in the text input below. </br>
                      Example: 'Bearer 12345abcdef'",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement()
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                },
                Scheme = "oauth2",
                Name = "Bearer",
                In = ParameterLocation.Header,

            },
            new List<string>()
        }
    });
});

// Initialize DataBase services
var connectionString = builder.Configuration["Database"] ?? "";
builder.Services.AddDbContext<MessengerContext>(
    optionsAction: op => op.UseNpgsql(connectionString),
    contextLifetime: ServiceLifetime.Singleton,
    optionsLifetime: ServiceLifetime.Singleton);
builder.Services.AddSingleton<UserService>();

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
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapGet("/public_key", () => RSAEncryption.ServerPublicRSAKeyBase64);
app.MapGet("/encrypt/{encryptedString}",
    RSAEncryption.Decrypt_Base64);
app.MapGet("/validate_jwt",
    async ([FromQuery] string jwt, HttpContext context) =>
    {
        var res = await JwtTokenManager.ValidateToken(jwt);
        return res.IsValid;
    });

if (app.Environment.IsDevelopment())
{
    app.MapGet("/check_auth", [Authorize] (HttpContext context) => "Authenticated");
    app.MapGet("/check_auth_admin", [Authorize(Roles = Roles.Admin)] (HttpContext context)
        => "Authenticated as Admin");
}

app.MapPost("/add_register_code", 
    [Authorize(Roles = Roles.Admin)] async (HttpContext context, UserService userService, RegistrationCodeDto code) =>
    {
        await userService.CreateRegistrationCodeAsync(new RegistrationCode {Code = code.Code, Role = code.Role});
    });
app.MapHub<GeneralChatHub>(GeneralChatHub.HubUrl);
app.Run();