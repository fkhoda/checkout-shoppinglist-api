namespace ShoppingListService.Infrastructure.Actors.Tests
{
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
            var inMemoryProvider = new InMemoryProvider();
            var noOpMonitoringProvider = new NoOpMonitoringProvider();

            var props = Actor.FromProducer(() => new ShoppingListsActor(inMemoryProvider, noOpMonitoringProvider));

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
            var inMemoryProvider = new InMemoryProvider();
            var noOpMonitoringProvider = new NoOpMonitoringProvider();

            var props = Actor.FromProducer(() => new ShoppingListsActor(inMemoryProvider, noOpMonitoringProvider));

            var shoppingListsActor = Actor.SpawnNamed(props, "ShoppingLists");

            const string CustomerId = "Customer 1";
            const string ItemName = "Item 1";
            const int ItemQuantity = 5;

            await shoppingListsActor.RequestAsync<ShoppingListEvent>(new AddItem(CustomerId, ItemName, ItemQuantity));

            shoppingListsActor.Stop();

            await inMemoryProvider.GetEventsAsync("ShoppingLists/Customer 1", 0, long.MaxValue,
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
            var inMemoryProvider = new InMemoryProvider();
            var noOpMonitoringProvider = new NoOpMonitoringProvider();

            var props = Actor.FromProducer(() => new ShoppingListsActor(inMemoryProvider, noOpMonitoringProvider));

            var shoppingListsActor = Actor.SpawnNamed(props, "ShoppingLists");

            const string CustomerId = "Customer 1";
            const string ItemName = "Item 1";
            const int ItemQuantity = 5;

            await shoppingListsActor.RequestAsync<ShoppingListEvent>(new AddItem(CustomerId, ItemName, ItemQuantity));

            shoppingListsActor.Stop();

            var (snapshot, _) = await inMemoryProvider.GetSnapshotAsync("ShoppingLists/Customer 1");

            var snapshotState = snapshot as ShoppingList;

            Assert.Equal(1, snapshotState.Items.Count);
        }

        [Fact]
        public async Task Given_SameItemIsAddedTwice_Quantities_ShouldBeAddedTogether()
        {
            var inMemoryProvider = new InMemoryProvider();
            var noOpMonitoringProvider = new NoOpMonitoringProvider();

            var props = Actor.FromProducer(() => new ShoppingListsActor(inMemoryProvider, noOpMonitoringProvider));

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
            var inMemoryProvider = new InMemoryProvider();
            var noOpMonitoringProvider = new NoOpMonitoringProvider();

            var props = Actor.FromProducer(() => new ShoppingListsActor(inMemoryProvider, noOpMonitoringProvider));

            var shoppingListsActor = Actor.SpawnNamed(props, "ShoppingLists");

            const string CustomerId = "Customer 1";
            const string ItemName = "Item 1";
            const int ItemQuantity = 5;

            await shoppingListsActor.RequestAsync<ShoppingListEvent>(new AddItem(CustomerId, ItemName, ItemQuantity));
            await shoppingListsActor.RequestAsync<ShoppingListEvent>(new AddItem(CustomerId, ItemName, ItemQuantity));

            shoppingListsActor.Stop();

            // Wait for actor termination
            Thread.Sleep(50);

            // Respawn dead actor
            shoppingListsActor = Actor.SpawnNamed(props, "ShoppingLists");
            
            var itemRetrieved = await shoppingListsActor.RequestAsync<ShoppingListEvent>(new GetItem(CustomerId, ItemName));
          
            shoppingListsActor.Stop();

            Assert.IsType<ItemRetrieved>(itemRetrieved);
            Assert.Equal(10, ((ItemRetrieved)itemRetrieved).Quantity);
        }

        [Fact]
        public async Task Given_ItemQuantityIsUpdatedWithAValue_ItemQuantityReturned_ShouldBeReplacedWithThatValue()
        {
            var inMemoryProvider = new InMemoryProvider();
            var noOpMonitoringProvider = new NoOpMonitoringProvider();

            var props = Actor.FromProducer(() => new ShoppingListsActor(inMemoryProvider, noOpMonitoringProvider));

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
            var inMemoryProvider = new InMemoryProvider();
            var noOpMonitoringProvider = new NoOpMonitoringProvider();

            var props = Actor.FromProducer(() => new ShoppingListsActor(inMemoryProvider, noOpMonitoringProvider));

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
            var inMemoryProvider = new InMemoryProvider();
            var noOpMonitoringProvider = new NoOpMonitoringProvider();

            var props = Actor.FromProducer(() => new ShoppingListsActor(inMemoryProvider, noOpMonitoringProvider));

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
            var inMemoryProvider = new InMemoryProvider();
            var noOpMonitoringProvider = new NoOpMonitoringProvider();

            var props = Actor.FromProducer(() => new ShoppingListsActor(inMemoryProvider, noOpMonitoringProvider));

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
