using Serilog;
using OsbErpLite.Application;
using OsbErpLite.Infrastructure;
using OsbErpLite.Api.Extensions;
using OsbErpLite.Api.Middleware;
using OsbErpLite.Api.Services;
using OsbErpLite.Application.Common.Interfaces;
using OsbErpLite.Application.Dashboard.Dtos;

var builder = WebApplication.CreateBuilder(args);

builder.AddSerilogLogging();

// Connection string
var connectionString = Environment.GetEnvironmentVariable("CONNECTION_STRING")
    ?? throw new InvalidOperationException("CONNECTION_STRING environment variable is not set.");

var jwtSecretKey = Environment.GetEnvironmentVariable("JWT_SECRET_KEY")
    ?? throw new InvalidOperationException("JWT_SECRET_KEY environment variable is not set.");

// DI
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
builder.Services.Configure<DashboardSettings>(builder.Configuration.GetSection("Dashboard"));
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(connectionString, builder.Configuration, jwtSecretKey);
builder.Services.AddJwtAuthentication(builder.Configuration, jwtSecretKey);
builder.Services.AddApiRateLimiting(builder.Configuration);
builder.Services.AddApiCors(builder.Configuration);
builder.Services.AddHealthCheckServices(connectionString);

builder.Services.AddControllers();
builder.Services.AddSwaggerServices();

var app = builder.Build();

// Middleware
app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseSerilogRequestLogging();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors(CorsServiceExtensions.FrontendPolicyName);
app.UseApiRateLimiting(app.Configuration);
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapHealthCheckEndpoints();

app.Run();
