using System.Text;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;

using ChallengeTracker.Api.Api.Endpoints;
using ChallengeTracker.Api.Application.Interfaces;
using ChallengeTracker.Api.Application.Services;
using ChallengeTracker.Api.Infrastructure;
using ChallengeTracker.Api.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddIdentityCore<User>()
    .AddRoles<IdentityRole<Guid>>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

var jwtKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!));

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
      options.TokenValidationParameters = new TokenValidationParameters
      {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = jwtKey,
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        ClockSkew = TimeSpan.Zero
      };
    });
builder.Services.AddAuthorization();

builder.Services.AddScoped<IAuthService, AuthService>();

builder.Services.AddOpenApi();
builder.Services.AddProblemDetails();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
  app.MapOpenApi();
  app.MapScalarApiReference();
}

app.UseExceptionHandler();
app.UseStatusCodePages();

app.UseAuthentication();
app.UseAuthorization();

app.MapAuthEndpoints();

app.Run();
