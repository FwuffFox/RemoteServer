using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.OpenApi.Models;
using RemoteMessenger.Server.Hubs;
using RemoteMessenger.Server.Services;
using RemoteMessenger.Server.Services.SignalR;
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
var inMemory = builder.Configuration.GetValue<bool>("InMemory");
if (inMemory) 
{
    builder.Services.AddDbContext<MessengerContext>(
        optionsAction: op => op.UseInMemoryDatabase(databaseName: "MessengerDb")
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
var database = app.Services.GetService<MessengerContext>();

if (database.RegistrationCodes.Count == 0) 
{
    var code = new RegistrationCode(Code = "123", Role = "Admin");
    database.RegistrationCodes.Add(code);
    database.SaveChanges();
}

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

app.MapPost("/add_register_code", 
    [Authorize(Roles = Roles.Admin)] async (HttpContext context, UserService userService, RegistrationCodeDto code) =>
    {
        await userService.CreateRegistrationCodeAsync(new RegistrationCode {Code = code.Code, Role = code.Role});
    });
app.MapHub<GeneralChatHub>(GeneralChatHub.HubUrl);
app.MapHub<DirectMessagesHub>(DirectMessagesHub.HubUrl);
app.Run();