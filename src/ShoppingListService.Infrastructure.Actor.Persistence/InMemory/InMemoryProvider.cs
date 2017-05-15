namespace ShoppingListService.Infrastructure.Actor.Persistence.InMemory
{
    using Proto.Persistence;

    public sealed class InMemoryProvider : IInMemoryProvider
    {
        private readonly IInMemoryProviderState state;

        public InMemoryProvider(IInMemoryProviderState state)
        {
            this.state = state;
        }

        public IProviderState GetState()
        {
            return state;
        }
    }
}
