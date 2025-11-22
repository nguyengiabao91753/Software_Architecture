using Integrations.Messaging.Events;
using MassTransit;
using Orders.Orchestrator;
using Quartz;

var builder = WebApplication.CreateBuilder(args);


var configuration = builder.Configuration;
builder.Services.AddMassTransit(x =>
{
    x.SetKebabCaseEndpointNameFormatter();

    x.AddSagaStateMachine<OrderOrchestrationSaga, OrderStateData>()
     .MongoDbRepository(r =>
     {
         r.Connection = "mongodb://root:example123@mongodb:27017";
         r.DatabaseName = "order_saga_db";
         r.CollectionName = "order_sagas";
     });

    //x.AddDelayedMessageScheduler();           

    x.UsingRabbitMq((context, configurator) =>
    {
        configurator.Host(configuration["MessageBroker:HostAddress"], host =>
        {
            host.Username(configuration["MessageBroker:UserName"]);
            host.Password(configuration["MessageBroker:Password"]);
        });


        //configurator.UseDelayedMessageScheduler();
        configurator.ConfigureEndpoints(context);
    });
});

//builder.Services.AddQuartz(q =>
//{
//    q.SchedulerId = "Scheduler-Core";
//    q.UseMicrosoftDependencyInjectionJobFactory();
//});

//builder.Services.AddQuartzHostedService(options =>
//{
//    options.WaitForJobsToComplete = true;
//});
// Add services to the container.
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



app.Run();


