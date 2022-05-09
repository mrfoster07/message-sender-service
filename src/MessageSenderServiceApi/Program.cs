using MessageSenderServiceApi.Domain.Modules.Notification.Extensions;
using MessageSenderServiceApi.Infrastructure.Extensions;
using MessageSenderServiceApi.Infrastructure.Extensions.DI;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

//Config
builder.Configuration.AddEnvironmentVariables();

//Packages
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddHealthChecks();
builder.Services.AddSwaggerGen();

builder.Host.UseSerilog(
    (ctx, lc) => lc
        .WriteTo.Console());

//Domain
builder.Services.AddNotificationModule();

//Infrastructure
builder.Services.AddProviders(builder.Configuration);
builder.Services.AddRepositories();


//App
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapHealthChecks("/health");

app.MapControllers();

app.Run();