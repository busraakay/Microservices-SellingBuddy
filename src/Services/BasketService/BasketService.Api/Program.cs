using BasketService.Api.Core.Application.Repository;
using BasketService.Api.Core.Application.Services;
using BasketService.Api.Extensions;
using BasketService.Api.Infrastructure.Repository;
using BasketService.Api.IntegrationEvents.EventHandlers;
using BasketService.Api.IntegrationEvents.Events;
using EventBus.Base;
using EventBus.Base.Abstraction;
using EventBus.Factory;

//var builder = WebApplication.CreateBuilder(args);

//// Add services to the container.

//builder.WebHost.UseUrls("http://localhost:5003");

//builder.Services.ConfigureConsul(builder.Configuration);
//builder.Services.ConfigureAuth(builder.Configuration);
//builder.Services.AddSingleton(sp => sp.ConfigureRedis(builder.Configuration));

//builder.Services.AddHttpContextAccessor();
//builder.Services.AddScoped<IBasketRepository, RedisBasketRepository>();
//builder.Services.AddTransient<IIdentityService, IdentityService>();


//builder.Services.AddSingleton<IEventBus>(sp =>
//{
//    EventBusConfig config = new()
//    {
//        ConnectionRetryCount = 5,
//        EventNameSuffix = "IntegrationEvent",
//        SubscriberClientAppName = "BasketService",
//        EventBusType = EventBusType.RabbitMQ
//    };

//    return EventBusFactory.Create(config, sp);
//});

//builder.Services.AddTransient<OrderCreatedIntegrationEventHandler>();



//builder.Services.AddControllers();
//// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
//builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen();

//builder.Services.ConfigureConsul(builder.Configuration);

//var app = builder.Build();

//// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
//    app.UseSwagger();
//    app.UseSwaggerUI();
//}

//app.UseHttpsRedirection();

//app.UseAuthentication();
//app.UseAuthorization();

//app.MapControllers();

//app.RegisterWithConsul(app.Lifetime);

//IEventBus eventBus = app.Services.GetRequiredService<IEventBus>();
//eventBus.Subscribe<OrderCreatedIntegrationEvent, OrderCreatedIntegrationEventHandler>();

//app.Run();


var builder = WebApplication.CreateBuilder(args);

builder.WebHost.UseUrls("http://localhost:5003");

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.ConfigureConsul(builder.Configuration);
builder.Services.ConfigureAuth(builder.Configuration);
builder.Services.AddSingleton(sp => sp.ConfigureRedis(builder.Configuration));

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IBasketRepository, RedisBasketRepository>();
builder.Services.AddTransient<IIdentityService, IdentityService>();

builder.Services.AddSingleton<IEventBus>(sp =>
{
    EventBusConfig config = new()
    {
        ConnectionRetryCount = 5,
        EventNameSuffix = "IntegrationEvent",
        SubscriberClientAppName = "BasketService",
        EventBusType = EventBusType.RabbitMQ
    };

    return EventBusFactory.Create(config, sp);
});

builder.Services.AddTransient<OrderCreatedIntegrationEventHandler>();

builder.Services.AddControllers();
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

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.RegisterWithConsul(app.Lifetime);

IEventBus eventBus = app.Services.GetRequiredService<IEventBus>();
eventBus.Subscribe<OrderCreatedIntegrationEvent, OrderCreatedIntegrationEventHandler>();

app.Run();
