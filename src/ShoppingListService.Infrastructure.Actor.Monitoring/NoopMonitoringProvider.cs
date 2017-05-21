namespace ShoppingListService.Infrastructure.Actor.Monitoring
{
    using System.Threading.Tasks;

    using Proto;

    public class NoopMonitoringProvider : IMonitoringProvider
    {
        public Task IndexReceiveAsync(IContext context)
        {
            return Task.FromResult(0);
        }

        public Task IndexSendAsync(ISenderContext senderContext)
        {
            return Task.FromResult(0);
        }
    }
}
