namespace ShoppingListService.Infrastructure.Actor.Monitoring.Elasticsearch
{
    using System;

    using Nest;

    public class ReceivedMessage
    {
        public object Message { get; set; }

        [Keyword]
        public string SelfId { get; set; }

        public string SelfAddress { get; set; }

        [Keyword]
        public string SenderId { get; set; }

        public string SenderAddress { get; set; }

        public DateTime Timestamp { get; set; }

        public string TypeName { get; set; }
    }
}
