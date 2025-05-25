using CatalogService.Api.Extensions;
using CatalogService.Api.Infrastructure;
using CatalogService.Api.Infrastructure.Context;

var builder = WebApplication.CreateBuilder(new WebApplicationOptions
{
    Args = args,
    ApplicationName = typeof(Program).Assembly.FullName,
    ContentRootPath = Directory.GetCurrentDirectory(),
    WebRootPath = "Pics",
    EnvironmentName = Environments.Development // -> Staging -> Production
});

// Add services to the container.

builder.Services.ConfigureDbContext(builder.Configuration);
builder.Services.Configure<CatalogSettings>(builder.Configuration.GetSection(nameof(CatalogSettings)));

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

try
{
    app.MigrateDbContext<CatalogContext>((context, services) =>
    {
        var env = services.GetService<IWebHostEnvironment>();
        var logger = services.GetService<ILogger<CatalogContextSeed>>();

        new CatalogContextSeed()
            .SeedAsync(context, env, logger)
            .Wait();
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

app.Run();
