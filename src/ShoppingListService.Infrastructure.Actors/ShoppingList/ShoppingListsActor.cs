namespace ShoppingListService.Infrastructure.Actors.ShoppingList
{
    using System.Linq;
    using System.Threading.Tasks;

    using Proto;
    using Proto.Persistence;

    using ShoppingListService.Core.Application.ShoppingList.Actors.Messages;
    using ShoppingListService.Infrastructure.Actor.Monitoring;

    public class ShoppingListsActor : IActor
    {
        private readonly IProvider persistenceProvider;
        private readonly IMonitoringProvider monitoringProvider;

        public ShoppingListsActor(IProvider persistenceProvider, IMonitoringProvider monitoringProvider)
        {
            this.persistenceProvider = persistenceProvider;
            this.monitoringProvider = monitoringProvider;
        }

        public Task ReceiveAsync(IContext context)
        {
            if (context.Message is ShoppingListMessage)
            {
                var message = context.Message as ShoppingListMessage;

                var childId = $"{context.Self.Id}/{message.CustomerId}";

                var props = Actor.FromProducer(() => new ShoppingListActor(persistenceProvider, childId))
                    .WithReceiveMiddleware(Monitoring.ForReceiveMiddlewareUsing(monitoringProvider))
                    .WithSenderMiddleware(Monitoring.ForSenderMiddlewareUsing(monitoringProvider));

                var shoppingListActor = context.Children.All(c => c.Id != childId) ?
                    context.SpawnNamed(props, message.CustomerId) :
                    context.Children.FirstOrDefault(c => c.Id == childId);

                shoppingListActor.Request(context.Message, context.Sender);
            }

            return Actor.Done;
        }
    }
}
