namespace ShoppingListService.Infrastructure.Actors.Tests
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    using Proto;

    using ShoppingListService.Core.Application.ShoppingList.Actors.Messages;
    using ShoppingListService.Core.Domain.ShoppingList.Events;
    using ShoppingListService.Core.Domain.ShoppingList.Models;
    using ShoppingListService.Infrastructure.Actor.Monitoring;
    using ShoppingListService.Infrastructure.Actor.Persistence.InMemory;
    using ShoppingListService.Infrastructure.Actors.ShoppingList;

    using Xunit;

    public class ShoppingListActorTests
    {
        [Fact]
        public async Task Given_NewItemIsAdded_ResponseStatusCode_ShouldBeItemAdded()
        {
            var inMemoryProviderState = new InMemoryProviderState();
            var inMemoryProvider = new InMemoryProvider(inMemoryProviderState);
            var noopMonitoringProvider = new NoopMonitoringProvider();

            var props = Actor.FromProducer(() => new ShoppingListsActor(inMemoryProvider, noopMonitoringProvider));

            var shoppingListsActor = Actor.SpawnNamed(props, "ShoppingLists");

            const string CustomerId = "Customer 1";
            const string ItemName = "Item 1";
            const int ItemQuantity = 5;

            var reply = await shoppingListsActor.RequestAsync<ShoppingListEvent>(new AddItem(CustomerId, ItemName, ItemQuantity));

            shoppingListsActor.Stop();

            Assert.IsType<ItemAdded>(reply);
            Assert.Equal(ItemName, ((ItemAdded)reply).Name);
            Assert.Equal(ItemQuantity, ((ItemAdded)reply).Quantity);
            Assert.Equal(Status.ItemAdded, reply.Status);
        }

        [Fact]
        public async Task Given_NewItemIsAdded_ItemAddedEvent_ShouldBeSavedToPersistence()
        {
            var inMemoryProviderState = new InMemoryProviderState();
            var inMemoryProvider = new InMemoryProvider(inMemoryProviderState);
            var noopMonitoringProvider = new NoopMonitoringProvider();

            var props = Actor.FromProducer(() => new ShoppingListsActor(inMemoryProvider, noopMonitoringProvider));

            var shoppingListsActor = Actor.SpawnNamed(props, "ShoppingLists");

            const string CustomerId = "Customer 1";
            const string ItemName = "Item 1";
            const int ItemQuantity = 5;

            await shoppingListsActor.RequestAsync<ShoppingListEvent>(new AddItem(CustomerId, ItemName, ItemQuantity));

            shoppingListsActor.Stop();

            await inMemoryProviderState.GetEventsAsync("ShoppingLists/Customer 1", 0,
                o =>
                    {
                        Assert.IsType(typeof(ItemAdded), o);
                        Assert.Equal(ItemName, ((ItemAdded)o).Name);
                        Assert.Equal(ItemQuantity, ((ItemAdded)o).Quantity);
                    });
        }

        [Fact]
        public async Task Given_NewItemIsAdded_Snapshot_ShouldBeSavedToPersistence()
        {
            var inMemoryProviderState = new InMemoryProviderState();
            var inMemoryProvider = new InMemoryProvider(inMemoryProviderState);
            var noopMonitoringProvider = new NoopMonitoringProvider();

            var props = Actor.FromProducer(() => new ShoppingListsActor(inMemoryProvider, noopMonitoringProvider));

            var shoppingListsActor = Actor.SpawnNamed(props, "ShoppingLists");

            const string CustomerId = "Customer 1";
            const string ItemName = "Item 1";
            const int ItemQuantity = 5;

            await shoppingListsActor.RequestAsync<ShoppingListEvent>(new AddItem(CustomerId, ItemName, ItemQuantity));

            shoppingListsActor.Stop();

            var (snapshot, _) = await inMemoryProviderState.GetSnapshotAsync("ShoppingLists/Customer 1");

            var snapshotState = snapshot as ShoppingList;

            Assert.Equal(1, snapshotState.Items.Count);
        }

        [Fact]
        public async Task Given_SameItemIsAddedTwice_Quantities_ShouldBeAddedTogether()
        {
            var inMemoryProviderState = new InMemoryProviderState();
            var inMemoryProvider = new InMemoryProvider(inMemoryProviderState);
            var noopMonitoringProvider = new NoopMonitoringProvider();

            var props = Actor.FromProducer(() => new ShoppingListsActor(inMemoryProvider, noopMonitoringProvider));

            var shoppingListsActor = Actor.SpawnNamed(props, "ShoppingLists");

            const string CustomerId = "Customer 1";
            const string ItemName = "Item 1";
            const int ItemQuantity = 5;

            await shoppingListsActor.RequestAsync<ShoppingListEvent>(new AddItem(CustomerId, ItemName, ItemQuantity));
            await shoppingListsActor.RequestAsync<ShoppingListEvent>(new AddItem(CustomerId, ItemName, ItemQuantity));

            var itemRetrieved = await shoppingListsActor.RequestAsync<ShoppingListEvent>(new GetItem(CustomerId, ItemName));

            shoppingListsActor.Stop();

            Assert.Equal(10, ((ItemRetrieved)itemRetrieved).Quantity);
        }

        [Fact]
        public async Task Given_ShoppingListsActorIsRestartedAfterSameItemIsAddedTwice_State_ShouldBeRestoredFromPersistence()
        {
            var inMemoryProviderState = new InMemoryProviderState();
            var inMemoryProvider = new InMemoryProvider(inMemoryProviderState);
            var noopMonitoringProvider = new NoopMonitoringProvider();

            var props = Actor.FromProducer(() => new ShoppingListsActor(inMemoryProvider, noopMonitoringProvider));

            var shoppingListsActor = Actor.SpawnNamed(props, "ShoppingLists");

            const string CustomerId = "Customer 1";
            const string ItemName = "Item 1";
            const int ItemQuantity = 5;

            await shoppingListsActor.RequestAsync<ShoppingListEvent>(new AddItem(CustomerId, ItemName, ItemQuantity));
            await shoppingListsActor.RequestAsync<ShoppingListEvent>(new AddItem(CustomerId, ItemName, ItemQuantity));

            shoppingListsActor.Stop();

            // Wait for actor termination
            Thread.Sleep(10);

            // Respawn dead actor
            shoppingListsActor = Actor.SpawnNamed(props, "ShoppingLists");

            var itemRetrieved = await shoppingListsActor.RequestAsync<ShoppingListEvent>(new GetItem(CustomerId, ItemName));

            shoppingListsActor.Stop();

            Assert.Equal(10, ((ItemRetrieved)itemRetrieved).Quantity);
        }

        [Fact]
        public async Task Given_ItemQuantityIsUpdatedWithAValue_ItemQuantityReturned_ShouldBeReplacedWithThatValue()
        {
            var inMemoryProviderState = new InMemoryProviderState();
            var inMemoryProvider = new InMemoryProvider(inMemoryProviderState);
            var noopMonitoringProvider = new NoopMonitoringProvider();

            var props = Actor.FromProducer(() => new ShoppingListsActor(inMemoryProvider, noopMonitoringProvider));

            var shoppingListsActor = Actor.SpawnNamed(props, "ShoppingLists");

            const string CustomerId = "Customer 1";
            const string ItemName = "Item 1";
            const int ItemQuantity = 5;

            await shoppingListsActor.RequestAsync<ShoppingListEvent>(new AddItem(CustomerId, ItemName, ItemQuantity));

            await shoppingListsActor.RequestAsync<ShoppingListEvent>(new UpdateQuantity(CustomerId, ItemName, 15));

            var itemRetrieved = await shoppingListsActor.RequestAsync<ShoppingListEvent>(new GetItem(CustomerId, ItemName));

            shoppingListsActor.Stop();

            Assert.Equal(15, ((ItemRetrieved)itemRetrieved).Quantity);
        }

        [Fact]
        public async Task Given_NonExistingItemIsRemoved_ResponseStatusCode_ShouldBeItemNotFound()
        {
            var inMemoryProviderState = new InMemoryProviderState();
            var inMemoryProvider = new InMemoryProvider(inMemoryProviderState);
            var noopMonitoringProvider = new NoopMonitoringProvider();

            var props = Actor.FromProducer(() => new ShoppingListsActor(inMemoryProvider, noopMonitoringProvider));

            var shoppingListsActor = Actor.SpawnNamed(props, "ShoppingLists");

            const string CustomerId = "Customer 1";
            const string ItemName = "Item 1";

            var reply = await shoppingListsActor.RequestAsync<ShoppingListEvent>(new RemoveItem(CustomerId, ItemName));

            shoppingListsActor.Stop();

            Assert.Equal(Status.ItemNotFound, reply.Status);
        }

        [Fact]
        public async Task Given_ExistingItemIsRemoved_ResponseStatusCode_ShouldBeItemRemoved()
        {
            var inMemoryProviderState = new InMemoryProviderState();
            var inMemoryProvider = new InMemoryProvider(inMemoryProviderState);
            var noopMonitoringProvider = new NoopMonitoringProvider();

            var props = Actor.FromProducer(() => new ShoppingListsActor(inMemoryProvider, noopMonitoringProvider));

            var spawnNamed = Actor.SpawnNamed(props, "ShoppingLists");

            const string CustomerId = "Customer 1";
            const string ItemName = "Item 1";
            const int ItemQuantity = 5;

            await spawnNamed.RequestAsync<ShoppingListEvent>(new AddItem(CustomerId, ItemName, ItemQuantity));
            var reply = await spawnNamed.RequestAsync<ShoppingListEvent>(new RemoveItem(CustomerId, ItemName));

            spawnNamed.Stop();

            Assert.IsType<ItemRemoved>(reply);
            Assert.Equal(ItemName, ((ItemRemoved)reply).Name);
            Assert.Equal(Status.ItemRemoved, reply.Status);
        }

        [Fact]
        public async Task Given_ExistingItemIsRemoved_Item_ShouldBeRemovedFromStore()
        {
            var inMemoryProviderState = new InMemoryProviderState();
            var inMemoryProvider = new InMemoryProvider(inMemoryProviderState);
            var noopMonitoringProvider = new NoopMonitoringProvider();

            var props = Actor.FromProducer(() => new ShoppingListsActor(inMemoryProvider, noopMonitoringProvider));

            var shoppingListsActor = Actor.SpawnNamed(props, "ShoppingLists");

            const string CustomerId = "Customer 1";
            const string ItemName = "Item 1";
            const int ItemQuantity = 5;

            await shoppingListsActor.RequestAsync<ShoppingListEvent>(new AddItem(CustomerId, ItemName, ItemQuantity));

            await shoppingListsActor.RequestAsync<ShoppingListEvent>(new RemoveItem(CustomerId, ItemName));

            var itemRetrieved = await shoppingListsActor.RequestAsync<ShoppingListEvent>(new GetItem(CustomerId, ItemName));

            shoppingListsActor.Stop();

            Assert.Equal(Status.ItemNotFound, itemRetrieved.Status);
        }
    }
}
