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

var builder = WebApplication.CreateBuilder(args);


var connectionString = builder.Configuration.GetConnectionString("Database") ?? builder.Configuration["ConnectionStrings:DefaultConnection"];

builder.Services.AddDbContext<IdentityApplicationDbContext>(options =>
   options.UseSqlServer(connectionString)
   );

builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection("ApiSettings:JwtOptions"));

builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.User.RequireUniqueEmail = true;
}).AddEntityFrameworkStores<IdentityApplicationDbContext>().AddDefaultTokenProviders();


// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IAuthAPIService, AuthAPIService>();

builder.Services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();

builder.Services.AddHealthChecks();
builder.Services.AddCustomOpenTelemetry("Auth.API");


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    await app.InitialiseDatabaseAsync();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

//Đăng ký consul
app.MapHealthChecks("/health");
app.RegisterWithConsul(builder.Configuration);

app.Run();
