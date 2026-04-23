using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;

using ChallengeTracker.Api.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

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

// Placeholder root endpoint — replaced by real endpoint groups in later phases.
app.MapGet("/", () => "ChallengeTracker API");

app.Run();
