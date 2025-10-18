using Integrations.Messaging.Masstransit;
using Payment.Messaging;
using Payment.Messaging.Consumer;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddHttpClient("OrdersApi", client =>
{
    client.BaseAddress = new Uri(builder.Configuration["Services:OrdersApi"]);
});


//Add Masstransit Message Broker
builder.Services.AddPaymentMessaging(builder.Configuration);
//builder.Services.AddMessageBroker(builder.Configuration, typeof(OrderPlacedConsumer).Assembly);











// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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
