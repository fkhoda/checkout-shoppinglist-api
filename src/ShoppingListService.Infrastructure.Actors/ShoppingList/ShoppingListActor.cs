namespace ShoppingListService.Infrastructure.Actors.ShoppingList
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    using Proto;
    using Proto.Persistence;

    using ShoppingListService.Core.Application.ShoppingList.Actors.Messages;
    using ShoppingListService.Core.Application.ShoppingList.Dtos.Responses;
    using ShoppingListService.Core.Domain.ShoppingList.Events;
    using ShoppingListService.Core.Domain.ShoppingList.Models;

    public class ShoppingListActor : IPersistentActor
    {
        public Persistence Persistence { get; set; }

        private ShoppingList State { get; set; } = new ShoppingList();

        public async Task ReceiveAsync(IContext context)
        {
            switch (context.Message)
            {
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
                        await Persistence.PersistEventAsync(@event)
                            .ContinueWith(t => Persistence.PersistSnapshotAsync(State))
                            .ContinueWith(t => context.Respond(@event));
                    }
                    break;

                case UpdateQuantity msg:
                    {
                        var @event = new QuantityUpdated(msg.Name, msg.Quantity);
                        await Persistence.PersistEventAsync(@event)
                            .ContinueWith(t => Persistence.PersistSnapshotAsync(State))
                            .ContinueWith(t => context.Respond(@event));
                    }
                    break;

                case RemoveItem msg:
                    {
                        var @event = new ItemRemoved(msg.Name);
                        await Persistence.PersistEventAsync(@event)
                            .ContinueWith(t => Persistence.PersistSnapshotAsync(State))
                            .ContinueWith(t => context.Respond(@event));
                    }
                    break;
            }
        }

        public void UpdateState(object message)
        {
            switch (message)
            {
                case Event e:
                    ApplyEvent(e);
                    break;

                case Snapshot s:
                    ApplySnapshot(s);
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
            if (snapshot.State is ShoppingList shoppingList)
            {
                State = shoppingList;
            }
        }
    }
}