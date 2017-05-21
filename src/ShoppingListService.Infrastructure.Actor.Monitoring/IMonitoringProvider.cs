namespace ShoppingListService.Infrastructure.Actor.Monitoring
{
    using System.Threading.Tasks;

    using Proto;

    public interface IMonitoringProvider
    {
        Task IndexReceiveAsync(IContext context);

        Task IndexSendAsync(ISenderContext senderContext);
    }
}
