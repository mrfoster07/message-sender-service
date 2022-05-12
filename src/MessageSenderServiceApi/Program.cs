using MessageSenderServiceApi.Domain.Modules.Notification.Extensions;
using MessageSenderServiceApi.Infrastructure.Extensions.DI;
using MessageSenderServiceApi.Middleware;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

//Config
builder.Configuration.AddEnvironmentVariables();

//Packages
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddHealthChecks();
builder.Services.AddSwaggerGen();

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger();

builder.Host.UseSerilog();

//Domain
builder.Services.AddNotificationModule();

//Infrastructure
builder.Services.AddDataProviders(builder.Configuration);
builder.Services.AddRepositories();


//App
var app = builder.Build();

app.RunMigrationsIfNeeds(app.Configuration);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<ErrorHandlerMiddleware>();

app.MapHealthChecks("/health");

app.MapControllers();

app.Run();