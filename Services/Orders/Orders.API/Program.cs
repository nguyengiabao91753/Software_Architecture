using Orders.Application;
using Orders.Infrastructure;
using Orders.Infrastructure.Extensions;
using Orders.Messaging;
using Integrations.Messaging.Masstransit;

var builder = WebApplication.CreateBuilder(args);





// Add services to the container.
builder.Services.AddInfrastructureServices(builder.Configuration)
                .AddOrderMessaging()
                .AddOrderServices();

//Add Masstransit Message Broker
builder.Services.AddMessageBroker(builder.Configuration, typeof(Program).Assembly);


builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();





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

app.Run();
