namespace ShoppingListService.DependencyResolution
{
    using System;

    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;

    using Proto.Persistence;

    using ShoppingListService.Core.Application.ShoppingList.Actors;
    using ShoppingListService.Infrastructure.Actor.Monitoring;
    using ShoppingListService.Infrastructure.Actor.Monitoring.Elasticsearch;
    using ShoppingListService.Infrastructure.Actor.Persistence.InMemory;
    using ShoppingListService.Infrastructure.Actors.ShoppingList;

    public sealed class DependencyRegistrar
    {
        public void Register(IServiceCollection services, IConfigurationRoot configurationRoot)
        {
            services.AddSingleton<IShoppingListsActorProvider, ShoppingListsActorProvider>();
            services.AddSingleton<IProvider, InMemoryProvider>();
            services.AddSingleton<IInMemoryProviderState, InMemoryProviderState>();
            services.AddSingleton<IMonitoringProvider>(
                p =>
                    {
                        var host = GetConfig(configurationRoot, "ELASTICSEARCH_HOST", "Monitoring:Elasticsearch:Host");
                        var index = GetConfig(configurationRoot, "ELASTICSEARCH_INDEX", "Monitoring:Elasticsearch:Index");
                        var username = GetConfig(configurationRoot, "ELASTICSEARCH_USERNAME", "Monitoring:Elasticsearch:Username");
                        var password = GetConfig(configurationRoot, "ELASTICSEARCH_PASSWORD", "Monitoring:Elasticsearch:Password");
                        if (host == null || index == null || username == null || password == null)
                        {
                            return new NoopMonitoringProvider();
                        }
                        return new ElasticsearchMonitoringProvider(host, index, username, password);
                    });
        }

        private static string GetConfig(IConfigurationRoot configurationRoot, string envVariable, string configKey)
        {
            var valueFromEnvVariable = Environment.GetEnvironmentVariable(envVariable);
            if (string.IsNullOrEmpty(valueFromEnvVariable))
            {
                var valueFromConfiguration = configurationRoot[configKey];
                if (string.IsNullOrEmpty(valueFromConfiguration))
                {
                    return null;
                }
                return valueFromConfiguration;
            }
            return valueFromEnvVariable;
        }
    }
}
