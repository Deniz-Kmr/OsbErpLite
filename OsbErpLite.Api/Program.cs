using Microsoft.EntityFrameworkCore;
using OsbErpLite.Infrastructure.Persistence.Context;

var builder = WebApplication.CreateBuilder(args);

var connectionString = Environment.GetEnvironmentVariable("CONNECTION_STRING")
    ?? throw new InvalidOperationException("CONNECTION_STRING environment variable is not set.");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString));

var app = builder.Build();

app.UseHttpsRedirection();

app.Run();
