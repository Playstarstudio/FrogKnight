using Inventory.Model;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using static UnityEngine.Rendering.HDROutputUtils;

namespace Inventory
{
    public class InventoryManager : MonoBehaviour
    {
        private P_StateManager playerStateManager;
        private bool isInventoryOpen = false;
        [SerializeField] private InventoryItem itemPrefab;
        [SerializeField] private RectTransform contentPanel;
        [SerializeField] private GameObject inventoryPanel;
        [SerializeField] private InventorySO inventoryData;
        [SerializeField] private InventoryDescription inventoryDescription;
        [SerializeField] private FloatingItem floatingItem;
        List<InventoryItem> listofUIItems = new List<InventoryItem>();
        private int currentlyDraggedItemIndex = -1;
        public event Action<int> onDescriptionRequested, OnItemActionRequested, OnStartDragging;
        public event Action<int, int> OnSwapItems;
        public List<InventoryItem> initialItems = new List<InventoryItem>();
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
            foreach (InventoryItem item in initialItems)
            {
                if (item.empty)
                    continue;
                else
                    inventoryData.AddInventoryItem(item);
            }
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
            if (listofUIItems.Count > itemIndex)
            {
                listofUIItems[itemIndex].SetData(itemImage, itemQuantity);
            }
        }
        public int TryPickupItem(ItemOnGround item)
        {
            int reminder = inventoryData.AddItem(item.inventoryItem, item.quantity);
            if (reminder == 0)
            {
                item.DestroyItem();
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

        private void ResetSelection()
        {
            inventoryDescription.ResetDescription();
            DeselectAllItems();
        }

        private void DeselectAllItems()
        {
            foreach (InventoryItem item in listofUIItems)
            {
                item.Deselect();
            }
        }


        public void InitializeInventoryUI(int inventorySize)
        {
            for (int i = 0; i < inventorySize; i++)
            {
                InventoryItem newItem = Instantiate(itemPrefab, Vector3.zero, Quaternion.identity);
                newItem.transform.SetParent(contentPanel);
                listofUIItems.Add(newItem);
                newItem.OnItemClicked += HandleItemSelection;
                newItem.OnItemBeginDrag += HandleItemBeginDrag;
                newItem.OnItemEndDrag += HandleItemEndDrag;
                newItem.OnItemDroppedOn += HandleItemSwap;
                newItem.OnRightMouseBtnClick += HandleShowItemActions;
            }
        }

        private void PrepareUI()
        {
            InitializeInventoryUI(inventoryData.Capacity);
            onDescriptionRequested += HandleDescriptionRequest;
            OnSwapItems += HandleSwapItems;
            OnStartDragging += HandleStartDragging;
            OnItemActionRequested += HandleItemActionRequest;
        }
        private void UpdateDescription(int itemIndex, Sprite image, string name, string description)
        {
            inventoryDescription.SetDescription(image, name, description);
            DeselectAllItems();
            listofUIItems[itemIndex].Select();
        }

        #region Event Handlers
        private void HandleItemActionRequest(int itemIndex)
        {
            InventorySO.InventoryItem inventoryItem = inventoryData.GetItemAt(itemIndex);
            if (inventoryItem.IsEmpty)
                return;
            IItemAction itemAction = inventoryItem.item as IItemAction;
            if(itemAction != null)
            {
                itemAction.PerformAction(playerStateManager);
            }
            IDestroyableItem destroyableItem = inventoryItem.item as IDestroyableItem;
            if (destroyableItem != null)
            {
                inventoryData.RemoveItem(itemIndex,1);
            }
        }

        private void HandleStartDragging(int itemIndex)
        {
            InventorySO.InventoryItem invItem = inventoryData.GetItemAt(itemIndex);
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
            InventorySO.InventoryItem inventoryItem = inventoryData.GetItemAt(itemIndex);
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