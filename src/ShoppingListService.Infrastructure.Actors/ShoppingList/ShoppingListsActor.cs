namespace ShoppingListService.Infrastructure.Actors.ShoppingList
{
    using System.Linq;
    using System.Threading.Tasks;

    using Proto;
    using Proto.Persistence;

    using ShoppingListService.Core.Application.ShoppingList.Actors.Messages;

    public class ShoppingListsActor : IActor
    {
        private readonly IProvider persistenceProvider;

        public ShoppingListsActor(IProvider persistenceProvider)
        {
            this.persistenceProvider = persistenceProvider;
        }

        public Task ReceiveAsync(IContext context)
        {
            if (context.Message is ShoppingListMessage)
            {
                var message = context.Message as ShoppingListMessage;

                var props = Actor.FromProducer(() => new ShoppingListActor()).WithReceiveMiddleware(Persistence.Using(persistenceProvider));

                var childId = $"{context.Self.Id}/{message.CustomerId}";

                var shoppingListActor = context.Children.All(c => c.Id != childId) ?
                    context.SpawnNamed(props, message.CustomerId) :
                    context.Children.FirstOrDefault(c => c.Id == childId);

                shoppingListActor.Request(context.Message, context.Sender);
            }

            return Actor.Done;
        }
    }
}
