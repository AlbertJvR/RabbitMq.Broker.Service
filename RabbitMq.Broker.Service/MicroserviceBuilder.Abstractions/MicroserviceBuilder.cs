using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Microservice.Abstractions
{
    //Trying to use similar architecture as https://github.com/aspnet/Docs/blob/master/aspnetcore/fundamentals/dependency-injection/samples/2.x/DependencyInjectionSample/Program.cs
    public class MicroserviceBuilder
    {
        private Action<IConfigurationRoot,IServiceCollection> _startup;
        protected readonly List<Action<IConfigurationBuilder>> _configurationActions = new List<Action<IConfigurationBuilder>>();
        public string ConfigurationPath => Path.Combine(AppContext.BaseDirectory, "Config");
        private string[] _commandLineArgs;
        public static MicroserviceBuilder CreateDefaultBuilder(string[] args)
        {
            return new MicroserviceBuilder(args);
        }

        public MicroserviceBuilder(string[] args)
        {
            _commandLineArgs = args;
        }

        private IConfigurationRoot BuildConfig(string[] args)
        {
            var configBuilder = new ConfigurationBuilder();
            if (Directory.Exists(ConfigurationPath))
            {
                configBuilder.SetBasePath(ConfigurationPath);
                var files = Directory.GetFiles(ConfigurationPath, "*.json");
                foreach (var file in files)
                    configBuilder.AddJsonFile(file);
            }

            foreach (var builderDelegate in _configurationActions)
                builderDelegate(configBuilder);

            configBuilder.AddEnvironmentVariables();

            if (args != null)
                configBuilder.AddCommandLine(args);

            return configBuilder.Build();
        }

        public MicroserviceBuilder UseStartup<T>() where T: IMicroserviceStartup, new ()
        {
            _startup = (config,i) =>
            {
                var startup = new T();
                startup.AddConfig(config);
                startup.ConfigureServices(i);
            };
            return this;
        }

        public MicroserviceHost Build()
        {
            var config = BuildConfig(_commandLineArgs);
            var sc = new ServiceCollection();
            sc.AddSingleton<IConfigurationRoot>(config);
            sc.AddSingleton<IConfiguration>(config);
            sc.AddLogging((logBuilder) =>
            {
                logBuilder.AddConsole();
                logBuilder.AddConfiguration(config.GetSection("Logging"));
            });
            sc.AddOptions();
            _startup?.Invoke(config,sc);

            return new MicroserviceHost(sc.BuildServiceProvider());
        }
    }
}
