namespace ShoppingListService.DependencyResolution
{
    using Microsoft.Extensions.DependencyInjection;

    using Proto.Persistence;

    using ShoppingListService.Core.Application.ShoppingList.Actors;
    using ShoppingListService.Infrastructure.Actor.Persistence.InMemory;
    using ShoppingListService.Infrastructure.Actors.ShoppingList;

    public sealed class DependencyRegistrar
    {
        public void Register(IServiceCollection services)
        {
            services.AddSingleton<IShoppingListsActorProvider, ShoppingListsActorProvider>();
            services.AddSingleton<IProvider, InMemoryProvider>();
            services.AddSingleton<IInMemoryProviderState, InMemoryProviderState>();
        }
    }
}
