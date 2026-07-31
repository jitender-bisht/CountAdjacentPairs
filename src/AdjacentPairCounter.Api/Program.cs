using AdjacentPairCounter.Application.Interfaces;
using AdjacentPairCounter.Application.Services;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddScoped<IAdjacentPairService, AdjacentPairService>();
builder.Services.AddOpenApi();
builder.Services.AddHealthChecks();

var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();

    // Redirect the root URL to Scalar
    app.MapGet("/", () => Results.Redirect("/scalar"));
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapHealthChecks("/healthz");
app.MapControllers();

app.Run();
