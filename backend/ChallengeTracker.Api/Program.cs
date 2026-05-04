using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.RateLimiting;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;

using ChallengeTracker.Api.Api.Endpoints;
using ChallengeTracker.Api.Application.Interfaces;
using ChallengeTracker.Api.Application.Services;
using ChallengeTracker.Api.Infrastructure;
using ChallengeTracker.Api.Infrastructure.Data;
using ChallengeTracker.Api.Models;

// Don't auto-map JWT 'sub' claim to ClaimTypes.NameIdentifier — keep claim names intact.
JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddIdentityCore<User>(options =>
    {
      // Demo-friendly password rules (relaxed from Identity defaults).
      options.Password.RequireDigit = false;
      options.Password.RequireLowercase = false;
      options.Password.RequireUppercase = false;
      options.Password.RequireNonAlphanumeric = false;
      options.Password.RequiredLength = 6;
    })
    .AddRoles<IdentityRole<Guid>>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

var jwtKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!));

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
      // Don't rename incoming `sub` to `ClaimTypes.NameIdentifier`.
      options.MapInboundClaims = false;
      options.TokenValidationParameters = new TokenValidationParameters
      {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = jwtKey,
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        ClockSkew = TimeSpan.Zero,
        NameClaimType = JwtRegisteredClaimNames.Sub
      };
    });
builder.Services.AddAuthorization();

// CORS — allow the local Vite dev server to call us.
builder.Services.AddCors(options =>
{
  options.AddDefaultPolicy(policy =>
  {
    policy.WithOrigins("http://localhost:5173")
        .AllowAnyMethod()
        .AllowAnyHeader();
  });
});

// Rate limiting — two named policies, partitioned by user id (sub claim).
// `progress-post` is generous because users will often log a few entries in a row.
// `general-post` is tighter because joining/creating/state-transitioning is rarer.
builder.Services.AddRateLimiter(options =>
{
  options.AddPolicy("progress-post", httpContext =>
      RateLimitPartition.GetFixedWindowLimiter(
          partitionKey: GetPartitionKey(httpContext),
          factory: _ => new FixedWindowRateLimiterOptions
          {
            PermitLimit = 10,
            Window = TimeSpan.FromMinutes(1),
            QueueLimit = 0,
            AutoReplenishment = true
          }));

  options.AddPolicy("general-post", httpContext =>
      RateLimitPartition.GetFixedWindowLimiter(
          partitionKey: GetPartitionKey(httpContext),
          factory: _ => new FixedWindowRateLimiterOptions
          {
            PermitLimit = 5,
            Window = TimeSpan.FromMinutes(1),
            QueueLimit = 0,
            AutoReplenishment = true
          }));

  options.OnRejected = async (context, ct) =>
  {
    context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;

    if (context.Lease.TryGetMetadata(MetadataName.RetryAfter, out var retry))
    {
      context.HttpContext.Response.Headers.RetryAfter =
          ((int)retry.TotalSeconds).ToString();
    }

    await context.HttpContext.Response.WriteAsJsonAsync(new
    {
      title = "Too many requests",
      detail = "You're sending requests faster than allowed. Try again in a moment.",
      status = 429
    }, cancellationToken: ct);
  };

  static string GetPartitionKey(HttpContext context)
  {
    return context.User.FindFirstValue(ClaimTypes.NameIdentifier)
        ?? context.User.FindFirstValue("sub")
        ?? "anon";
  }
});

builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IChallengeService, ChallengeService>();
builder.Services.AddScoped<IMembershipService, MembershipService>();
builder.Services.AddScoped<IProgressEntryService, ProgressEntryService>();
builder.Services.AddScoped<ILeaderboardService, LeaderboardService>();
builder.Services.AddScoped<DbSeeder>();

builder.Services.AddOpenApi();
builder.Services.AddProblemDetails();

// Accept enums as strings on the wire ("Public" instead of 0).
builder.Services.ConfigureHttpJsonOptions(options =>
{
  options.SerializerOptions.Converters.Add(
      new System.Text.Json.Serialization.JsonStringEnumConverter());
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
  app.MapOpenApi();
  app.MapScalarApiReference();

  using var scope = app.Services.CreateScope();
  var seeder = scope.ServiceProvider.GetRequiredService<DbSeeder>();
  await seeder.SeedAsync();
}

app.UseExceptionHandler();
app.UseStatusCodePages();

// CORS must come before auth so preflight requests are handled.
app.UseCors();

app.UseAuthentication();
app.UseAuthorization();

// Rate limiter sits after auth so the partition key resolves to the actual user.
app.UseRateLimiter();

app.MapHealthEndpoints();
app.MapAuthEndpoints();
app.MapChallengeEndpoints();
app.MapMembershipEndpoints();
app.MapProgressEntryEndpoints();
app.MapLeaderboardEndpoints();

app.Run();
