namespace ShoppingListService.Infrastructure.Actor.Monitoring.Elasticsearch
{
    using System;
    using System.Threading.Tasks;

    using Microsoft.Extensions.Logging;

    using Nest;

    using Proto;

    public class ElasticsearchMonitoringProvider : IMonitoringProvider
    {
        private readonly ElasticClient elasticClient;

        public ElasticsearchMonitoringProvider(string uri, string indexName, string username,
            string password, bool recreateIndex, ILogger logger)
        {
            var connectionSettings = new ConnectionSettings(new Uri(uri)).DefaultIndex(indexName);

            if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password))
            {
                connectionSettings = connectionSettings.BasicAuthentication(username, password);
            }

            elasticClient = new ElasticClient(connectionSettings);

            if (recreateIndex)
            {
                logger.LogInformation($"Recreating index {indexName}");
                if (elasticClient.IndexExists(indexName).Exists)
                {
                    elasticClient.DeleteIndex(indexName);
                }
            }

            if (!elasticClient.IndexExists(indexName).Exists)
            {
                var descriptor = new CreateIndexDescriptor(indexName)
                    .Mappings(ms => ms
                        .Map<ReceivedMessage>(m => m.AutoMap())
                        .Map<SentMessage>(m => m.AutoMap())
                    );

                elasticClient.CreateIndex(indexName, d => descriptor);
            }
        }

        public async Task IndexReceiveAsync(IContext context)
        {
            await elasticClient.IndexAsync(new ReceivedMessage
            {
                Message = context.Message,
                TypeName = context.Message.GetType().FullName,
                SelfId = context.Self != null ? context.Self.Id : string.Empty,
                SelfAddress = context.Self != null ? context.Self.Address : string.Empty,
                SenderId = context.Sender != null ? context.Sender.Id : string.Empty,
                SenderAddress = context.Sender != null ? context.Sender.Address : string.Empty,
                Timestamp = DateTime.Now
            }, i => i.Type(typeof(ReceivedMessage)));
        }

        public async Task IndexSendAsync(ISenderContext senderContext)
        {
            await elasticClient.IndexAsync(new SentMessage
            {
                Message = senderContext.Message,
                TypeName = senderContext.Message.GetType().FullName
            }, i => i.Type(typeof(SentMessage)));
        }
    }
}
