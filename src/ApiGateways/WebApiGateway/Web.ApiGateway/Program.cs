using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Ocelot.Provider.Consul;
using Ocelot.Values;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// ServiceProvider ayarlarý
builder.Host.UseDefaultServiceProvider(options =>
{
    options.ValidateOnBuild = false;
    options.ValidateScopes = false;
});

// Ocelot yapýlandýrmasýný ekliyoruz
builder.Configuration
    .SetBasePath(builder.Environment.ContentRootPath)
    .AddJsonFile("Configurations/ocelot.json", optional: false, reloadOnChange: true)
    .AddEnvironmentVariables();

// Ocelot servislerini ekliyoruz
builder.Services.AddOcelot(builder.Configuration).AddConsul(); 

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy",
        builder => builder.SetIsOriginAllowed((host) => true)
        .AllowAnyMethod()
        .AllowAnyHeader()
        .AllowCredentials());
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseRouting();
app.UseCors("CorsPolicy");

app.UseAuthorization();

app.MapControllers();

await app.UseOcelot();


app.Run();
