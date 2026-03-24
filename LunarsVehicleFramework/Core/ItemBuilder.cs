using System;
using System.Collections.Generic;

namespace LunarsVehicleFramework.Core
{
    /// <summary>
    /// Builder class for easily creating new items in Subnautica
    /// </summary>
    public class ItemBuilder
    {
        private string _itemId;
        private string _itemName;
        private string _itemDescription;
        private string _iconPath;
        private int _stackSize = 1;
        private Dictionary<string, object> _properties = new Dictionary<string, object>();

        public ItemBuilder(string itemId, string itemName)
        {
            _itemId = itemId ?? throw new ArgumentNullException(nameof(itemId));
            _itemName = itemName ?? throw new ArgumentNullException(nameof(itemName));
        }

        /// <summary>
        /// Set the item description
        /// </summary>
        public ItemBuilder WithDescription(string description)
        {
            _itemDescription = description;
            return this;
        }

        /// <summary>
        /// Set the item icon file path
        /// </summary>
        public ItemBuilder WithIcon(string iconPath)
        {
            _iconPath = iconPath ?? throw new ArgumentNullException(nameof(iconPath));
            return this;
        }

        /// <summary>
        /// Set the stack size for the item
        /// </summary>
        public ItemBuilder WithStackSize(int stackSize)
        {
            if (stackSize < 1)
                throw new ArgumentException("Stack size must be at least 1", nameof(stackSize));

            _stackSize = stackSize;
            return this;
        }

        /// <summary>
        /// Add a custom property to the item
        /// </summary>
        public ItemBuilder WithProperty(string key, object value)
        {
            _properties[key] = value;
            return this;
        }

        /// <summary>
        /// Build and register the item
        /// </summary>
        public void Build()
        {
            var item = new ItemData
            {
                Id = _itemId,
                Name = _itemName,
                Description = _itemDescription ?? string.Empty,
                IconPath = _iconPath,
                StackSize = _stackSize,
                Properties = _properties
            };

            ItemRegistry.Register(item);
            LunarsVehicleFrameworkPlugin.Logger.LogInfo($"Item '{_itemName}' registered successfully");
        }
    }

    internal class ItemData
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string IconPath { get; set; }
        public int StackSize { get; set; }
        public Dictionary<string, object> Properties { get; set; }
    }

    internal static class ItemRegistry
    {
        private static Dictionary<string, ItemData> _items = new Dictionary<string, ItemData>();

        internal static void Register(ItemData item)
        {
            if (_items.ContainsKey(item.Id))
                throw new InvalidOperationException($"Item with ID '{item.Id}' is already registered");

            _items[item.Id] = item;
        }

        internal static ItemData Get(string itemId)
        {
            if (_items.TryGetValue(itemId, out var item))
                return item;

            throw new KeyNotFoundException($"Item with ID '{itemId}' not found");
        }

        internal static IReadOnlyDictionary<string, ItemData> GetAll()
        {
            return _items;
        }
    }
}
