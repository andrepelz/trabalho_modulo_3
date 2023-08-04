﻿using MassTransit;
using Microsoft.Extensions.Hosting;
using System.Reflection;
using System.Security.Authentication;

var builder = Host.CreateDefaultBuilder(args);

builder.ConfigureServices((hostContext, services) =>
{
    services.AddMassTransit(busConfigurator =>
    {
        var entryAssembly = Assembly.GetExecutingAssembly();
        busConfigurator.AddConsumers(entryAssembly);
        busConfigurator.UsingRabbitMq((context, busFactoryConfigurator) =>
        {
            busFactoryConfigurator.Host("jackal-01.rmq.cloudamqp.com", 5671, "zrdaqmiq", h =>
            {
                h.Username("zrdaqmiq");
                h.Password("P_-0fNC98Kiz5kOBrL5Tkz9fod4O6AY6");

                h.UseSsl(s =>
                {
                    s.Protocol = SslProtocols.Tls12;
                });
            });
            busFactoryConfigurator.ConfigureEndpoints(context);
        });
    });
});

var app = builder.Build();

app.Run();