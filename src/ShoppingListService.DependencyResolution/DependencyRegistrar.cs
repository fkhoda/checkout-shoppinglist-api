namespace ShoppingListService.DependencyResolution
{
    using System;

    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;

    using Proto.Persistence;

    using ShoppingListService.Core.Application.ShoppingList.Actors;
    using ShoppingListService.Infrastructure.Actor.Monitoring;
    using ShoppingListService.Infrastructure.Actor.Monitoring.Elasticsearch;
    using ShoppingListService.Infrastructure.Actor.Persistence.InMemory;
    using ShoppingListService.Infrastructure.Actors.ShoppingList;

    public sealed class DependencyRegistrar
    {
        public void Register(IServiceProvider serviceProvider, IServiceCollection services, IConfigurationRoot configurationRoot)
        {
            var logger = serviceProvider.GetService<ILogger<DependencyRegistrar>>();

            services.AddSingleton<IShoppingListsActorProvider, ShoppingListsActorProvider>();
            services.AddSingleton<IProvider, InMemoryProvider>();
            services.AddSingleton<IMonitoringProvider>(
                p =>
                    {
                        var host = GetConfig<string>(configurationRoot, "ES_URL", "Monitoring:Elasticsearch:Host");
                        var index = GetConfig<string>(configurationRoot, "ES_INDEX", "Monitoring:Elasticsearch:Index");
                        var username = GetConfig<string>(configurationRoot, "ES_USERNAME", "Monitoring:Elasticsearch:Username");
                        var password = GetConfig<string>(configurationRoot, "ES_PASSWORD", "Monitoring:Elasticsearch:Password");
                        var recreateIndexOnStartup = GetConfig<bool>(configurationRoot, "ES_RECREATEINDEXONSTARTUP", "Monitoring:Elasticsearch:RecreateIndexOnStartup");

                        if (string.IsNullOrEmpty(host) || string.IsNullOrEmpty(index))
                        {
                            logger.LogInformation("Using Noop monitoring.");
                            return new NoOpMonitoringProvider();
                        }

                        logger.LogInformation($"Using Elasticsearch monitoring at {host}.");
                        return new ElasticsearchMonitoringProvider(host, index, username, password, recreateIndexOnStartup, logger);
                    });
        }

        private static T GetConfig<T>(IConfiguration configuration, string envVariable, string configKey)
        {
            var valueFromEnvVariable = Environment.GetEnvironmentVariable(envVariable);
            if (string.IsNullOrEmpty(valueFromEnvVariable))
            {
                return configuration.GetValue<T>(configKey);
            }
            return (T)Convert.ChangeType(valueFromEnvVariable, typeof(T));
        }
    }
}
