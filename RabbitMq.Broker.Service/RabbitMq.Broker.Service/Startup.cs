﻿using System;
using System.Collections.Generic;
using System.Text;
using Microservice.Abstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RabbitMq.Adapter.Abstractions;
using RabbitMq.Adapter.Client;
using RabbitMq.Broker.Service.Options;

namespace RabbitMq.Broker.Service
{
    public class Startup: IMicroserviceStartup
    {
        private IConfiguration _configuration;

        public void AddConfig(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IRabbitMqAdapter, RabbitMqAdapter>();
            services.AddSingleton<IStartup, RabbitMqNameListenerService>();
            services.Configure<RabbitMqOptions>(_configuration.GetSection(nameof(RabbitMqOptions)));
            services.Configure<RabbitMqServiceOptions>(_configuration.GetSection(nameof(RabbitMqServiceOptions)));
        }
    }
}
