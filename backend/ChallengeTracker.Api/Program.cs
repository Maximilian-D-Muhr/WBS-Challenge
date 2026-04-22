using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// OpenAPI + ProblemDetails
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

// Placeholder root endpoint — will be replaced by real endpoint groups in later phases.
app.MapGet("/", () => "ChallengeTracker API");

app.Run();
