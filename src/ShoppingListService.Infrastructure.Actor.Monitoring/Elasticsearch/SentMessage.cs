namespace ShoppingListService.Infrastructure.Actor.Monitoring.Elasticsearch
{
    public class SentMessage
    {
        public object Message { get; set; }

        public string TypeName { get; set; }
    }
}
