using Auth.API.Data;
using Auth.API.Data.Extensions;
using Auth.API.Models;
using AuthAPI.Services;
using AuthAPI.Services.IServices;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Integrations.Consul.Extension;
using Integrations.OpenTelemetry.Extension;

using SecShare.Servicer.Auth;
using Consul;

using Prometheus;

var builder = WebApplication.CreateBuilder(args);

// Database
var connectionString = builder.Configuration.GetConnectionString("Database")
    ?? builder.Configuration["ConnectionStrings:DefaultConnection"];

builder.Services.AddDbContext<IdentityApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

// Identity
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.User.RequireUniqueEmail = true;
})
.AddEntityFrameworkStores<IdentityApplicationDbContext>()
.AddDefaultTokenProviders();

// JWT + Services
builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection("ApiSettings:JwtOptions"));

builder.Services.AddScoped<IAuthAPIService, AuthAPIService>();
builder.Services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();

// Controllers + Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Health Check
builder.Services.AddHealthChecks();
builder.Services.AddCustomOpenTelemetry("Auth.API");

var app = builder.Build();

// Prometheus Middleware (bắt buộc)
app.UseHttpMetrics();

// Pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    await app.InitialiseDatabaseAsync();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

// Consul + Healthcheck
app.MapHealthChecks("/health");
app.RegisterWithConsul(builder.Configuration);

// Expose endpoint cho Prometheus
app.MapMetrics("/metrics");
app.Run();
