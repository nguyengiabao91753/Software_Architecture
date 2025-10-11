using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Services.OrderAPI.Application.IServices;
using Services.OrderAPI.Application.Mapper;
using Services.OrderAPI.Application.Services;
using Services.OrderAPI.Infrastructure.DataBase;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

// Đọc connection string
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// Thêm DbContext vào DI container
builder.Services.AddDbContext<OrderDbContext>(options =>
    options.UseSqlServer(connectionString));

// Cấu hình AutoMapper
IMapper mapper = MappingConfig.RegisterMaps().CreateMapper();
builder.Services.AddSingleton(mapper);
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());


// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


#region Add Dependencies Injection
builder.Services.AddScoped<IOrderService, OrderService>();
#endregion

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
