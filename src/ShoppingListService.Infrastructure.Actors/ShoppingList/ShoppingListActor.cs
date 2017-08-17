namespace ShoppingListService.Infrastructure.Actors.ShoppingList
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;

    using Proto;
    using Proto.Persistence;

    using ShoppingListService.Core.Application.ShoppingList.Actors.Messages;
    using ShoppingListService.Core.Application.ShoppingList.Dtos.Responses;
    using ShoppingListService.Core.Domain.ShoppingList.Events;
    using ShoppingListService.Core.Domain.ShoppingList.Models;

    public class ShoppingListActor : IActor
    {
        private readonly Persistence persistence;

        private ShoppingList State { get; set; } = new ShoppingList();

        public ShoppingListActor(IProvider provider, string actorId)
        {
            persistence = Persistence.WithEventSourcingAndSnapshotting(provider, provider, actorId, ApplyEvent, ApplySnapshot);
        }

        public async Task ReceiveAsync(IContext context)
        {
            switch (context.Message)
            {
                case Started _:
                    await persistence.RecoverStateAsync();
                    break;

                case GetItems msg:
                    context.Respond(new ShoppingListDto
                    {
                        Items = State.Items
                            .Skip((msg.PageNumber - 1) * msg.PageSize)
                            .Take(msg.PageSize)
                            .Select(i => new ShoppingListItemDto
                            {
                                Name = i.Name,
                                Quantity = i.Quantity
                            }),
                        Count = State.Items.Count
                    });
                    break;

                case GetItem msg:
                    context.Respond(State.GetItemByName(msg.Name));
                    break;

                case AddItem msg:
                    {
                        var @event = new ItemAdded(msg.Name, msg.Quantity);
                        await persistence.PersistEventAsync(@event)
                            .ContinueWith(t => persistence.PersistSnapshotAsync(State))
                            .ContinueWith(t => context.Respond(@event));
                    }
                    break;

                case UpdateQuantity msg:
                    {
                        var @event = new QuantityUpdated(msg.Name, msg.Quantity);
                        await persistence.PersistEventAsync(@event)
                            .ContinueWith(t => persistence.PersistSnapshotAsync(State))
                            .ContinueWith(t => context.Respond(@event));
                    }
                    break;

                case RemoveItem msg:
                    {
                        var @event = new ItemRemoved(msg.Name);
                        await persistence.PersistEventAsync(@event)
                            .ContinueWith(t => persistence.PersistSnapshotAsync(State))
                            .ContinueWith(t => context.Respond(@event));
                    }
                    break;
            }
        }

        private void ApplyEvent(Event @event)
        {
            switch (@event.Data)
            {
                case ItemAdded e:
                    {
                        try
                        {
                            e.Status = State.AddOrUpdateItem(e.Name, e.Quantity).Status;
                        }
                        catch (Exception)
                        {
                            e.Status = Status.UnexpectedError;
                            throw;
                        }
                    }
                    break;

                case QuantityUpdated e:
                    {
                        try
                        {
                            e.Status = State.UpdateQuantity(e.Name, e.Quantity).Status;
                        }
                        catch (Exception)
                        {
                            e.Status = Status.UnexpectedError;
                            throw;
                        }
                    }
                    break;

                case ItemRemoved e:
                    {
                        try
                        {
                            e.Status = State.RemoveItem(e.Name).Status;
                        }
                        catch (Exception)
                        {
                            e.Status = Status.UnexpectedError;
                            throw;
                        }
                    }
                    break;
            }
        }

        private void ApplySnapshot(Snapshot snapshot)
        {
            File.AppendAllLines(@"C:\Users\fkhodabu\log.txt", new List<string> { $"{DateTime.Now} - Applying snapshot" });

            if (snapshot.State is ShoppingList shoppingList)
            {
                State = shoppingList;
            }
        }
    }
}