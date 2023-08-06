using Core.DTOs;
using Core.Interfaces;
using Core.Repositories;
using Core.Models;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Security.Claims;
using Core.Policy;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<AppSettings>(builder.Configuration.GetSection("Settings"));
builder.Services.AddScoped<ILoginRepository, LoginRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();

var appSettings = new AppSettings();
builder.Configuration.GetSection("Settings").Bind(appSettings);

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy(PermissionsPolicy.Read, policy => policy.RequireClaim("permissions", PermissionsPolicy.Read));
    options.AddPolicy(PermissionsPolicy.Write, policy => policy.RequireClaim("permissions", PermissionsPolicy.Write));
    options.AddPolicy(PermissionsPolicy.Delete, policy => policy.RequireClaim("permissions", PermissionsPolicy.Delete));
    options.AddPolicy(PermissionsPolicy.Update, policy => policy.RequireClaim("permissions", PermissionsPolicy.Update));
});

builder.Services.AddAuthentication("Bearer").AddJwtBearer(options =>
{
    SymmetricSecurityKey signedKey = new(Encoding.UTF8.GetBytes(appSettings.JwtSecret));
    SigningCredentials signingCredentials = new(signedKey, SecurityAlgorithms.HmacSha256Signature);

    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidIssuer = appSettings.Issuer,
        ValidateAudience = true,
        ValidAudience = appSettings.Audience,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = signedKey,
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero
    };
});


var app = builder.Build();

app.UseHttpsRedirection();

app.UseExceptionHandler("/error");

app.MapFallback("/error", () => Results.StatusCode(StatusCodes.Status500InternalServerError));

app.MapPost("/Login", (LoginRequest login, ILoginRepository loginRepository, IUserRepository userRepository) =>
{
    User user = userRepository.GetUser(login.User);

    bool isValidLogin = loginRepository.IsValidLogin(login, user);

    if (!isValidLogin)
    {
        return Results.Unauthorized();
    }

    return Results.Ok(loginRepository.GetJwt(user));
});

app.MapGet("/", (ClaimsPrincipal claims) => $"Hello {claims.Identity?.Name}").RequireAuthorization();

app.MapPost("/write", (ClaimsPrincipal claims) => $"You have permissions to write").RequireAuthorization(AuthorizationPolicies.WritePermission());

app.MapGet("/read", (ClaimsPrincipal claims) => $"You have permissions to read").RequireAuthorization(AuthorizationPolicies.ReadPermission());

app.MapPost("/delete", (ClaimsPrincipal claims) => $"You have permissions to delete").RequireAuthorization(AuthorizationPolicies.DeletePermission());

app.MapPut("/update", (ClaimsPrincipal claims) => $"You have permissions to update").RequireAuthorization(AuthorizationPolicies.UpdatePermission());

app.Run();
