namespace ShoppingListService.Core.Domain.ShoppingList.Models
{
    using System;
    using System.Collections.Generic;

    using ShoppingListService.Core.Domain.ShoppingList.Events;

    public sealed class ShoppingList
    {
        public List<ShoppingListItem> Items { get; } = new List<ShoppingListItem>();

        public ShoppingListEvent GetItemByName(string name)
        {
            var item = GetItem(name);

            if (item != null)
            {
                return new ItemRetrieved(item.Name, item.Quantity);
            }

            return new ShoppingListEvent(Status.ItemNotFound);
        }

        public ItemAdded AddOrUpdateItem(string name, int quantity)
        {
            var item = GetItem(name);

            if (item != null)
            {
                item.Quantity += quantity;
                return new ItemAdded(name, quantity, Status.QuantityUpdated);
            }

            Items.Add(new ShoppingListItem { Name = name, Quantity = quantity });

            return new ItemAdded(name, quantity);
        }

        public ShoppingListEvent UpdateQuantity(string name, int quantity)
        {
            var item = GetItem(name);

            if (item != null)
            {
                item.Quantity = quantity;
                return new QuantityUpdated(name, quantity);
            }

            return new ShoppingListEvent(Status.ItemNotFound);
        }

        public ShoppingListEvent RemoveItem(string name)
        {
            var item = GetItem(name);

            if (item != null)
            {
                Items.Remove(item);
                return new ItemRemoved(name);
            }

            return new ShoppingListEvent(Status.ItemNotFound);
        }

        private ShoppingListItem GetItem(string name)
        {
            return Items.Find(item => item.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        }
    }
}