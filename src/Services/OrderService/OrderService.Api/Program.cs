using EventBus.Base.Abstraction;
using EventBus.Base;
using EventBus.Factory;
using OrderService.Api.IntegrationEvents.EventHandlers;
using OrderService.Api.IntegrationEvents.Events;
using OrderService.Api.Extensions;
using OrderService.Infrastructure;
using OrderService.Application;
using OrderService.Infrastructure.Context;
using OrderService.Api.Extensions.Registration.EventHandlerRegistration;
using OrderService.Api.Extensions.Registration.ServiceDiscovery;
using RabbitMQ.Client;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services
    .AddLogging(configure => configure.AddConsole())
    .AddApplicationRegistration(typeof(Program))
    .AddPersistenceRegistration(builder.Configuration)
    .ConfigureEventHandlers()
    .AddServiceDiscoveryRegistration(builder.Configuration);

builder.Services.AddSingleton<IEventBus>(sp =>
{
    EventBusConfig config = new()
    {
        ConnectionRetryCount = 5,
        EventNameSuffix = "IntegrationEvent",
        SubscriberClientAppName = "OrderService",
        //Connection = new ConnectionFactory()
        //{
        //    HostName = "localhost",
        //    Port = 15672,
        //    UserName = "guest",
        //    Password = "guest"
        //},
        EventBusType = EventBusType.RabbitMQ
    };

    return EventBusFactory.Create(config, sp);
});

builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddEnvironmentVariables();

var app = builder.Build();

try
{
    app.MigrateDbContext<OrderDbContext>((context, services) =>
    {
        var logger = services.GetService<ILogger<OrderDbContext>>();

        var dbContextSeeder = new OrderDbContextSeed();
        dbContextSeeder.SeedAsync(context, logger).Wait();
    });
}
catch (Exception ex)
{
    // Burada loglama yapabilirsin veya konsola yazdýrabilirsin
    Console.WriteLine("Migration veya Seed sýrasýnda hata oluþtu: " + ex.Message);
    Console.WriteLine(ex.StackTrace);
    throw;  // Ýstersen burada uygulamayý durdurabilirsin
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();


IEventBus eventBus = app.Services.GetRequiredService<IEventBus>();
eventBus.Subscribe<OrderCreatedIntegrationEvent, OrderCreatedIntegrationEventHandler>();

app.Run();
