using Auth.API.Data;
using Auth.API.Models;
using AuthAPI.Services;
using AuthAPI.Services.IServices;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SecShare.Servicer.Auth;

var builder = WebApplication.CreateBuilder(args);


var connectionString = builder.Configuration.GetConnectionString("DefaultConnection").ToString();

builder.Services.AddDbContext<IdentityApplicationDbContext>(options =>
   options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")
       
    ));

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

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
