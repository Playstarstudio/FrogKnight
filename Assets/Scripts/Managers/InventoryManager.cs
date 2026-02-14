using Inventory.Model;
using Inventory.UI;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using static ItemSO;

namespace Inventory
{
    public class InventoryManager : MonoBehaviour
    {
        private P_StateManager playerStateManager;
        private bool isInventoryOpen = false;
        [SerializeField] private InventoryItem itemPrefab;
        [SerializeField] private RectTransform contentPanel;
        [SerializeField] private GameObject inventoryPanel;
        [SerializeField] public InventorySO inventoryData;
        [SerializeField] private InventoryDescription inventoryDescription;
        [SerializeField] private FloatingItem floatingItem;
        [SerializeField] List<InventoryItem> listofUIItems = new List<InventoryItem>();
        [SerializeField] public EquipmentSlot[] equipmentSlots;
        private enum DragSource { None, Inventory, Equipment }
        private DragSource dragSource = DragSource.None;
        private EquipmentSlot draggedEquipmentSlot;
        private int currentlyDraggedItemIndex = -1;
        public event Action<int> onDescriptionRequested, OnItemActionRequested, OnStartDragging;
        public event Action<int, int> OnSwapItems;
        public List<InventoryItem> initialItems = new List<InventoryItem>();
        [SerializeField]
        private ItemActionPanel actionPanel;
        [SerializeField]
        private AudioSource audioSource;
        [SerializeField]
        private AudioClip clip;
        [SerializeField]
        public GameObject genericItem;

        private void Awake()
        {
            Hide();
            floatingItem.Toggle(false);
            inventoryDescription.ResetDescription();
        }

        private void Start()
        {
            isInventoryOpen = false;
            PrepareUI();
            PrepareInventoryData();
            Hide();
        }

        private void PrepareInventoryData()
        {
            inventoryData.Initialize();
            inventoryData.InventoryChanged += UpdateInventoryUI;
            //current video state is 
            //initializing inventory starts with a blank inventory - needs resolving.
            /*
            foreach (InventoryItem item in initialItems)
            {
                if (item.empty)
                    continue;
                else
                    inventoryData.AddInventoryItem(item);
            }
             */
        }

        private void UpdateInventoryUI(Dictionary<int, InventorySO.InventoryItem> InventoryState)
        {
            ResetAllItems();
            foreach (var item in InventoryState)
            {
                UpdateData(item.Key, item.Value.item.image, item.Value.quantity);
            }
        }

        private void ResetAllItems()
        {
            foreach (var item in listofUIItems)
            {
                item.ResetData();
                item.Deselect();
            }
        }

        public void UpdateData(int itemIndex, Sprite itemImage, int itemQuantity)
        {
            listofUIItems[itemIndex].SetData(inventoryData.GetItemAt(itemIndex), itemImage, itemQuantity);
        }
        public int TryPickupItem(ItemOnGround item)
        {
            int reminder = inventoryData.AddItem(item.inventoryItem, item.quantity);
            if (reminder == 0)
            {
                return reminder;
            }
            else
            {
                item.quantity = reminder;
                return reminder;
            }
        }
        public bool InventoryCheck()
        {
            if (playerStateManager == null)
            {
                playerStateManager = FindFirstObjectByType<P_StateManager>();
            }
            if (isInventoryOpen)
            {
                Hide();
                return false;
            }
            else
            {
                Show();
                foreach (var uiItem in inventoryData.GetCurrentInventoryState())
                {
                    UpdateData(uiItem.Key, uiItem.Value.item.image, uiItem.Value.quantity);
                }
                return true;
            }
        }

        public void Show()
        {
            inventoryPanel.SetActive(true);
            isInventoryOpen = true;
            ResetSelection();
            /*
            for (int i = -1; i < playerStateManager.inventory.Length; i++)
            {
                listofUIItems[i].SetData(playerStateManager.inventory[i].image, playerStateManager.inventory[i].count);
            }
             */
        }
        public void Hide()
        {
            inventoryPanel.SetActive(false);
            isInventoryOpen = false;
        }
        public void ResetSelection()
        {
            inventoryDescription.ResetDescription();
            DeselectAllItems();
            DeselectAllEquipmentSlots();
        }
        public void AddAction(string actionName, Action performAction)
        {
            actionPanel.AddButton(actionName, performAction);
        }
        public void ShowItemAction(int itemIndex)
        {
            actionPanel.Toggle(true);
            actionPanel.transform.position = listofUIItems[itemIndex].transform.position;
        }

        private void DeselectAllItems()
        {
            foreach (InventoryItem item in listofUIItems)
            {
                item.Deselect();
            }
            actionPanel.Toggle(false);
        }


        public void InitializeInventoryUI(int inventorySize)
        {
            for (int i = 0; i < inventorySize; i++)
            {
                InventoryItem newItem = Instantiate(itemPrefab, contentPanel);
                RectTransform rt = newItem.GetComponent<RectTransform>();
                //Debug.Log($"Item {i} - LocalScale: {rt.localScale}, Position: {rt.anchoredPosition}, Active: {newItem.gameObject.activeSelf}");
                listofUIItems.Add(newItem);
                newItem.OnItemClicked += HandleItemSelection;
                newItem.OnItemBeginDrag += HandleItemBeginDrag;
                newItem.OnItemEndDrag += HandleItemEndDrag;
                newItem.OnItemDroppedOn += HandleItemSwap;
                newItem.OnRightMouseBtnClick += HandleShowItemActions;
                newItem.OnPointerEnter += HandlePointerEnter;
                newItem.OnPointerExit += HandlePointerExit;

            }
        }
        private void InitializeEquipmentSlots()
        {
            foreach (var slot in equipmentSlots)
            {
                slot.OnItemDroppedOnSlot += HandleDropOnEquipmentSlot;
                slot.OnSlotBeginDrag += HandleEquipmentSlotBeginDrag;
                slot.OnSlotEndDrag += HandleEquipmentSlotEndDrag;
                slot.OnSlotClicked += HandleEquipmentSlotClicked;
                // slot.OnRightMouseBtnClick
            }
        }
        private void HandleDropOnEquipmentSlot(EquipmentSlot slot)
        {
            if (dragSource == DragSource.Inventory)
            {
                InventorySO.InventoryItem invItem = inventoryData.GetInventoryItemAt(currentlyDraggedItemIndex);
                if (invItem.IsEmpty) return;

                EquippableItemSO equippable = invItem.item as EquippableItemSO;
                if (equippable == null || equippable.slot != slot.GetSlotType())
                    return;
                if (!slot.IsEmpty)
                {
                    InventoryItem replacedItem = slot.Unequip(slot.item);
                    if (replacedItem != null)
                    {
                        inventoryData.AddItemToFirstFreeSlot(replacedItem.item, 1);
                    }
                    slot.SetData(invItem.item);
                    inventoryData.RemoveItem(currentlyDraggedItemIndex, 1);
                }
                else
                {
                    slot.SetData(invItem.item);
                    inventoryData.RemoveItem(currentlyDraggedItemIndex, invItem.quantity);
                }
            }
            else if (dragSource == DragSource.Equipment && draggedEquipmentSlot != null)
            {
                if (draggedEquipmentSlot == slot) return;
                if (draggedEquipmentSlot.GetSlotType() != slot.GetSlotType()) return;
                ItemSO sourceItem = draggedEquipmentSlot.item;
                EquipmentSlot sourceSlot = draggedEquipmentSlot;
                ItemSO targetData = slot.item;
                draggedEquipmentSlot.SetData(targetData);
                slot.SetData(sourceItem);
            }
        }
        public bool HandleTryActionEquip(EquipmentSlot slot, int index)
        {
            InventorySO.InventoryItem invItem = inventoryData.GetInventoryItemAt(index);
            if (invItem.IsEmpty)
            {
                return false;
            }
            EquippableItemSO equippable = invItem.item as EquippableItemSO;
            if (equippable == null || equippable.slot != slot.GetSlotType())
            {
                return false;
            }
            if (!slot.IsEmpty)
            {
                InventoryItem replacedItem = slot.Unequip(slot.item);
                inventoryData.RemoveItem(index, 1);
                if (replacedItem != null)
                {
                    inventoryData.AddItemToFirstFreeSlot(replacedItem.item, 1);
                }
                slot.SetData(invItem.item);
                return true;
            }
            else
            {
                slot.SetData(invItem.item);
                inventoryData.RemoveItem(index, invItem.quantity);
                return true;
            }
        }
        private void HandleDropFromEquipmentToInventory(InventoryItem targetUIItem)
        {
            if (dragSource != DragSource.Equipment || draggedEquipmentSlot == null) return;
            if (draggedEquipmentSlot.IsEmpty) return;

            int targetIndex = listofUIItems.IndexOf(targetUIItem);
            if (targetIndex == -1) return;

            InventorySO.InventoryItem targetItem = inventoryData.GetInventoryItemAt(targetIndex);
            ItemSO unequippedItemSO = draggedEquipmentSlot.item;
            draggedEquipmentSlot.Unequip(targetUIItem.item);
            if (!targetItem.IsEmpty)
            {
                EquippableItemSO equippable = targetItem.item as EquippableItemSO;
                if (equippable != null && equippable.slot == draggedEquipmentSlot.GetSlotType())
                {
                    draggedEquipmentSlot.SetData(targetItem.item);
                    inventoryData.RemoveItem(targetIndex, targetItem.quantity);
                    inventoryData.AddItem(targetItem.item, 1);
                }
                else
                {
                    int remainder = inventoryData.AddItemToFirstFreeSlot(unequippedItemSO, 1);
                    if (remainder > 0)
                    {
                        draggedEquipmentSlot.SetData(unequippedItemSO);
                    }
                }
            }
            else
            {
                inventoryData.AddItem(unequippedItemSO, 1);
            }
        }
        public void HandleTryActionEquipSwap(EquipmentSlot slot)
        {
            inventoryData.AddItemToFirstFreeSlot(slot.item, 1);
        }

        private void HandleEquipmentSlotClicked(EquipmentSlot slot)
        {
            DeselectAllItems();
            DeselectAllEquipmentSlots();
            if (slot.IsEmpty)
            {
                ResetSelection();
                return;
            }
            slot.Select();
            inventoryDescription.SetDescription(slot.item.image, slot.item.name, slot.item.description);
        }

        private void DeselectAllEquipmentSlots()
        {
            foreach (EquipmentSlot item in equipmentSlots)
            {
                item.DeselectSlot();
            }
            actionPanel.Toggle(false);
        }

        private void HandleEquipmentSlotBeginDrag(EquipmentSlot slot)
        {
            dragSource = DragSource.Equipment;
            draggedEquipmentSlot = slot;
            CreateDraggedItem(slot.item.image, 1);
        }

        private void HandleEquipmentSlotEndDrag(EquipmentSlot slot)
        {
            ResetDraggedItem();
            dragSource = DragSource.None;
            draggedEquipmentSlot = null;
        }

        private void HandlePointerExit(InventoryItem item)
        {
            if (item.hoverPanel != null)
            {
                item.hoverPanel.Toggle(false);

            }
        }

        private void HandlePointerEnter(InventoryItem item)
        {
            int index = listofUIItems.IndexOf(item);
            InventorySO.InventoryItem inventoryItem = inventoryData.GetInventoryItemAt(index);
            if (inventoryItem.IsEmpty)
                return;
            else
                if (item.hoverPanel.panel != null)
            {
                item.hoverPanel.Toggle(true);

            }

        }

        private void PrepareUI()
        {
            InitializeInventoryUI(inventoryData.inventoryCapacity);
            InitializeEquipmentSlots();
            onDescriptionRequested += HandleDescriptionRequest;
            OnSwapItems += HandleSwapItems;
            OnStartDragging += HandleStartDragging;
            OnItemActionRequested += HandleItemActionRequest;
        }
        private void UpdateDescription(int itemIndex, Sprite image, string name, string description)
        {
            inventoryDescription.SetDescription(image, name, description);
            inventoryData.GetItemAt(itemIndex);
            inventoryData.GetInventoryItemAt(itemIndex);

            DeselectAllItems();
            DeselectAllEquipmentSlots();
            listofUIItems[itemIndex].Select();
        }

        #region Event Handlers
        private void HandleItemActionRequest(int itemIndex)
        {
            InventorySO.InventoryItem inventoryItem = inventoryData.GetInventoryItemAt(itemIndex);
            if (inventoryItem.IsEmpty)
                return;
            IItemAction itemAction = inventoryItem.item as IItemAction;
            if (itemAction != null)
            {
                ShowItemAction(itemIndex);
                AddAction(itemAction.ActionName, () => PerformAction(itemIndex));
            }
            IDestroyableItem destroyableItem = inventoryItem.item as IDestroyableItem;
            if (destroyableItem != null)
            {
                AddAction("Drop", () => DropItem(itemIndex, inventoryItem.quantity));
            }
        }

        private void DropItem(int itemIndex, int quantity)
        {
            GameObject newItem = Instantiate(genericItem);
            newItem.GetComponent<ItemOnGround>().inventoryItem = inventoryData.GetInventoryItemAt(itemIndex).item;
            newItem.GetComponent<ItemOnGround>().quantity = inventoryData.GetInventoryItemAt(itemIndex).quantity;
            newItem.transform.position = playerStateManager.gridManager.GetTileCenter(playerStateManager.currentTile);
            playerStateManager.gridManager.MapAddItem(newItem.GetComponent<ItemOnGround>(), playerStateManager.currentTile);
            inventoryData.RemoveItem(itemIndex, quantity);
            ResetSelection();
            if (clip != null)
            {
                audioSource.PlayOneShot(clip);
            }
        }

        public void PerformAction(int itemIndex)
        {
            InventorySO.InventoryItem inventoryItem = inventoryData.GetInventoryItemAt(itemIndex);
            if (inventoryItem.IsEmpty)
                return;
            IItemAction itemAction = inventoryItem.item as IItemAction;
            if (itemAction != null)
            {
                itemAction.PerformAction(playerStateManager, itemIndex);
                if (inventoryData.GetInventoryItemAt(itemIndex).IsEmpty)
                {
                    ResetSelection();
                }
            }
            if (inventoryItem.item.GetType() == typeof(ConsumableItemSO))
            {
                IDestroyableItem destroyableItem = inventoryItem.item as IDestroyableItem;
                if (destroyableItem != null)
                {
                    inventoryData.RemoveItem(itemIndex, 1);
                    ResetSelection();
                }
            }
        }

        private void HandleStartDragging(int itemIndex)
        {
            InventorySO.InventoryItem invItem = inventoryData.GetInventoryItemAt(itemIndex);

            if (invItem.IsEmpty)
            {
                return;
            }
            CreateDraggedItem(invItem.item.image, invItem.quantity);
        }

        private void HandleSwapItems(int itemIndex1, int itemIndex2)
        {
            inventoryData.SwapItems(itemIndex1, itemIndex2);
        }

        private void HandleDescriptionRequest(int itemIndex)
        {
            InventorySO.InventoryItem inventoryItem = inventoryData.GetInventoryItemAt(itemIndex);
            if (inventoryItem.IsEmpty)
            {
                ResetSelection();
                return;
            }
            string description = PrepareDescription(inventoryItem);
            ItemSO item = inventoryItem.item;
            UpdateDescription(itemIndex, item.image, item.name, description);
        }
        private string PrepareDescription(InventorySO.InventoryItem inventoryItem)
        {
            StringBuilder sb = new StringBuilder();
            //sb.Append(inventoryItem.item.description);
            sb.AppendLine();
            /* FOR IF I ADD MORE INFO PAST DESC
             */
            for (int i = 0; i < inventoryItem.item.effects.Count; i++)
            {
                sb.Append(inventoryItem.item.effects[i].operation);
                sb.Append("s ");
                sb.Append(inventoryItem.item.effects[i].modifierValue);
                sb.Append(" ");
                sb.Append(inventoryItem.item.effects[i].attName.name);
                sb.AppendLine();
            }
            return sb.ToString();
        }

        private void HandleShowItemActions(InventoryItem invItem)
        {
            int index = listofUIItems.IndexOf(invItem);
            if (index == -1)
            {
                return;
            }
            OnItemActionRequested?.Invoke(index);
        }

        private void HandleItemSwap(InventoryItem invItem)
        {
            if (dragSource == DragSource.Equipment)
            {
                HandleDropFromEquipmentToInventory(invItem);
                return;
            }
            int index = listofUIItems.IndexOf(invItem);
            if (index == -1)
            {
                return;
            }
            OnSwapItems?.Invoke(currentlyDraggedItemIndex, index);
        }

        private void HandleItemEndDrag(InventoryItem invItem)
        {
            ResetDraggedItem();
        }

        private void HandleItemBeginDrag(InventoryItem invItem)
        {
            dragSource = DragSource.Inventory;
            int index = listofUIItems.IndexOf(invItem);
            if (index == -1)
                return;
            currentlyDraggedItemIndex = index;
            HandleItemSelection(invItem);
            OnStartDragging?.Invoke(index);
        }
        public void CreateDraggedItem(Sprite sprite, int quantity)
        {
            floatingItem.Toggle(true);
            floatingItem.SetData(sprite, quantity);
        }
        private void HandleItemSelection(InventoryItem invItem)
        {
            int index = listofUIItems.IndexOf(invItem);
            if (index == -1)
                return;
            onDescriptionRequested?.Invoke(index);
        }
        private void ResetDraggedItem()
        {
            floatingItem.Toggle(false);
            currentlyDraggedItemIndex = -1;
        }


        #endregion
    }
}