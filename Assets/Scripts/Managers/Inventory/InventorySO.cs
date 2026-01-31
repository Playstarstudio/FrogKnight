using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


namespace Inventory.Model
{
    [CreateAssetMenu(fileName = "InventorySO", menuName = "Scriptable Objects/InventorySO")]
    public class InventorySO : ScriptableObject
    {
        [SerializeField]
        private List<InventoryItem> inventoryItems;

        [field: SerializeField]
        public int Capacity { get; private set; } = 20;

        public event Action<Dictionary<int, InventoryItem>> InventoryChanged;
        public void Initialize()
        {
            inventoryItems = new List<InventoryItem>();
            for (int i = 0; i < Capacity; i++)
            {
                inventoryItems.Add(InventoryItem.GetEmptyItem());
            }
        }
        public int AddItem(ItemSO item, int count)
        {
            if (item.isStackable == false)
            {
                for (int i = 0; i < inventoryItems.Count; i++)
                {
                    while (count > 0 && IsInventoryFull() == false)
                    {
                        count -= AddItemToFirstFreeSlot(item, 1);
                    }
                    ItemChange();
                    return count;
                }
            }
            count = AddStackableItem(item, count);
            ItemChange();
            return count;
        }

        private int AddItemToFirstFreeSlot(ItemSO item, int v)
        {
            InventoryItem newItem = new InventoryItem
            {
                item = item,
                quantity = v
            };
            for (int i = 0; i < inventoryItems.Count; i++)
            {

                if (inventoryItems[i].IsEmpty)
                {
                    inventoryItems[i] = newItem;
                    return v;
                }
            }
            return 0;
        }

        private bool IsInventoryFull()
            => inventoryItems.Where(item => item.IsEmpty).Any() == false;

        private int AddStackableItem(ItemSO item, int count)
        {
            for (int i = 0; i < inventoryItems.Count; ++i)
            {
                if (!inventoryItems[i].IsEmpty)
                    continue;
                if (inventoryItems[i].item.ID == item.ID)
                {
                    int amountPossibleToTake =
                        inventoryItems[i].item.MaxStackSize - inventoryItems[i].quantity;
                    if (count > amountPossibleToTake)
                    {
                        inventoryItems[i] = inventoryItems[i]
                            .ChangeQuantity(inventoryItems[i].item.MaxStackSize);
                        count -= amountPossibleToTake;
                    }
                    else
                    {
                        inventoryItems[i] = inventoryItems[i]
                            .ChangeQuantity(inventoryItems[i].quantity + count);
                        ItemChange();
                        return 0;
                    }
                }
            }
            while (count > 0 && IsInventoryFull() == false)
            {
                int newCount = Mathf.Clamp(count, 0, item.MaxStackSize);
                count -= newCount;
                AddItemToFirstFreeSlot(item, newCount);
            }
            return count;
        }

        public Dictionary<int, InventoryItem> GetCurrentInventoryState()
        {
            Dictionary<int, InventoryItem> returnValue = new Dictionary<int, InventoryItem>();
            for (int i = 0; i < inventoryItems.Count; i++)
            {
                if (inventoryItems[i].IsEmpty)
                    continue;
                returnValue[i] = inventoryItems[i];
            }
            return returnValue;
        }

        public InventoryItem GetItemAt(int itemIndex)
        {
            return inventoryItems[itemIndex];
        }

        public void AddInventoryItem(global::InventoryItem item)
        {
            AddItem(item.item, item.quantity);
            return;
        }

        internal void SwapItems(int itemIndex1, int itemIndex2)
        {
            InventoryItem item1 = inventoryItems[itemIndex1];
            inventoryItems[itemIndex1] = inventoryItems[itemIndex2];
            inventoryItems[itemIndex2] = item1;
            ItemChange();
        }

        private void ItemChange()
        {
            InventoryChanged?.Invoke(GetCurrentInventoryState());
        }

        [Serializable]
        public struct InventoryItem
        {
            public ItemSO item;
            public int quantity;
            public bool IsEmpty => item == null;

            public InventoryItem ChangeQuantity(int newQuantity)
            {
                return new InventoryItem
                {
                    item = this.item,
                    quantity = newQuantity
                };
            }

            public static InventoryItem GetEmptyItem()
            => new InventoryItem
            {
                item = null,
                quantity = 0
            };
        }
    }

}