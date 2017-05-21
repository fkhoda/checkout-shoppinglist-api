namespace ShoppingListService.Infrastructure.Actor.Monitoring.Elasticsearch
{
    using System;

    public class ReceivedMessage
    {
        public object Message { get; set; }

        public string SelfId { get; set; }

        public string SelfAddress { get; set; }

        public string SenderId { get; set; }

        public string SenderAddress { get; set; }

        public DateTime Timestamp { get; set; }

        public string TypeName { get; set; }
    }
}
