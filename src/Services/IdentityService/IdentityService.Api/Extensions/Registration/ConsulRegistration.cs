﻿using Consul;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Http.Features;

namespace IdentityService.Api.Extensions.Registration
{
    public static class ConsulRegistrationAdd
    {
        public static IServiceCollection ConfigureConsul(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<IConsulClient, ConsulClient>(p => new ConsulClient(consulConfig =>
            {
                var address = configuration["ConsulConfig:Address"];
                consulConfig.Address = new Uri(address);
            }));

            return services;
        }

        public static IApplicationBuilder RegisterWithConsul(this IApplicationBuilder app, IHostApplicationLifetime lifetime)
        {
            var consulClient = app.ApplicationServices.GetRequiredService<IConsulClient>();
            var logger = app.ApplicationServices.GetRequiredService<ILoggerFactory>().CreateLogger<IApplicationBuilder>();

            lifetime.ApplicationStarted.Register(() =>
            {
                var features = app.ServerFeatures.Get<IServerAddressesFeature>();
                var address = features?.Addresses?.FirstOrDefault();

                if (string.IsNullOrEmpty(address))
                {
                    logger.LogError("No address found for registration with Consul.");
                    return;
                }

                var uri = new Uri(address);
                var registration = new AgentServiceRegistration()
                {
                    ID = "IdentityService",
                    Name = "IdentityService",
                    Address = uri.Host,
                    Port = uri.Port,
                    Tags = new[] { "IdentityService", "Identity", "JWT" }
                };

                logger.LogInformation("Registering with Consul at {0}:{1}", uri.Host, uri.Port);

                consulClient.Agent.ServiceDeregister(registration.ID).Wait();
                consulClient.Agent.ServiceRegister(registration).Wait();

                lifetime.ApplicationStopping.Register(() =>
                {
                    logger.LogInformation("Deregistering from Consul");
                    consulClient.Agent.ServiceDeregister(registration.ID).Wait();
                });
            });

            return app;
        }



    }
}
