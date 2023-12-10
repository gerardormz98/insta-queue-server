using System.Text;
using LiveWaitlistServer.Configuration;
using LiveWaitlistServer.Data;
using LiveWaitlistServer.Data.Interfaces;
using LiveWaitlistServer.Hubs;
using LiveWaitlistServer.Services;
using LiveWaitlistServer.Services.Interfaces;
using LiveWaitlistServer.Shared.Services;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Application

builder.Services.AddControllers();

builder.Services.AddSignalR(options => 
{
    options.ClientTimeoutInterval = TimeSpan.FromSeconds(20);
    options.KeepAliveInterval = TimeSpan.FromSeconds(10);
}).AddAzureSignalR();

builder.Services.AddSwaggerGen(c => 
{
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] { }
        }
    });
});

// Security

var jwtOptions = builder.Configuration.GetSection(JwtOptions.KeyName).Get<JwtOptions>() ?? new();

builder.Services.AddAuthentication().AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateIssuerSigningKey = true,
        ValidateLifetime = true,
        ValidIssuer = jwtOptions.Issuer,
        ValidAudience = jwtOptions.Audience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtOptions.Key))
    };
});

builder.Services.AddAuthorization();

if (builder.Environment.IsDevelopment())
{
    builder.Services.AddCors(options =>
    {
        options.AddDefaultPolicy(builder =>
        {
            builder.SetIsOriginAllowed(origin => new string[] { "localhost", "192.168.100.", "127.0.0.1" }.Any(h => new Uri(origin).Host.StartsWith(h)));
            builder.AllowCredentials();
            builder.AllowAnyHeader();
            builder.AllowAnyMethod();
        });
    });
}

// Configuration

builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection(JwtOptions.KeyName));
builder.Services.Configure<AesOptions>(builder.Configuration.GetSection(AesOptions.KeyName));
builder.Services.Configure<WaitlistConfigOptions>(builder.Configuration.GetSection(WaitlistConfigOptions.KeyName));

// Dependency Injection

builder.Services.AddSingleton<ILiveWaitlistManager, LiveWaitlistManagerInMemory>();
builder.Services.AddSingleton<IUserRepository, UserRepositoryInMemory>();
builder.Services.AddSingleton<IWaitlistHostRepository, WaitlistHostRepositoryInMemory>();
builder.Services.AddSingleton<ITokenService, TokenService>();
builder.Services.AddSingleton<IWaitlistCodeService, WaitlistCodeService>();
builder.Services.AddSingleton<IEncryptionService, EncryptionService>();

var app = builder.Build();

// Middleware

app.UseHttpsRedirection();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.MapHub<LiveWaitlistHub>("/hubs/live-waitlist");

app.Run();

// TODO: Azure function to clean the waitlists after 1 hour.