using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Ocelot.Provider.Polly;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.IdentityModel.JsonWebTokens;
using System.Text;
using Shares.SystemConfig.Authentication;
using Integrations.Consul.Extension;
using Ocelot.Provider.Consul;


var builder = WebApplication.CreateBuilder(args);

// Add Ocelot configuration
builder.Configuration.AddJsonFile("ocelot.json", optional: false, reloadOnChange: true);


// Add services to the container (optional for test endpoints)
builder.Services.AddControllers();

// Add Authentication with JWT
// Add Authentication with JWT
//builder.AddAppAuthentication();


builder.Services.AddCors(options => options.AddDefaultPolicy(p => p.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()));

// Add Ocelot
builder.Services.AddOcelot()
                //.AddPolly()
                //.AddConsul()
                ;

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
//builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerForOcelot(builder.Configuration);
//builder.Services.AddSwaggerGen();

builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();



var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    //app.UseSwagger();
    //app.UseSwaggerForOcelotUI(opt =>
    //{
    //    opt.PathToSwaggerGenerator = "/swagger/docs";
    //});
}
app.UseCors();
//app.UseHttpsRedirection();
//app.UseRouting();
//app.UseAuthentication();
//app.UseAuthorization();

//app.MapControllers();


// Important: use Ocelot after other middleware
app.UseOcelot().Wait();



app.Run();

