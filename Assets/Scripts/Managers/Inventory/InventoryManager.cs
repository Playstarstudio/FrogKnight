using System;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    private P_StateManager playerStateManager;
    [SerializeField] private GameObject inventoryPanel;
    private bool isInventoryOpen = false;
    public int inventorySize = 10;
    [SerializeField] private InventoryItem itemPrefab;
    [SerializeField] private RectTransform contentPanel;
    List<InventoryItem> listofUIItems = new List<InventoryItem>();


    private void Start()
    {
        isInventoryOpen = false;
        InitializeInventoryUI(inventorySize);
        Hide();
    }
    public void Show()
    {
        inventoryPanel.SetActive(true);
    }
    public void Hide()
    {
        inventoryPanel.SetActive(false);
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
            isInventoryOpen = false;
            return false;
        }
        else
        {
            Show();
            isInventoryOpen = true;
            return true;
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
    #region Event Handlers
    private void HandleShowItemActions(InventoryItem item)
    {
  
    }

    private void HandleItemSwap(InventoryItem item)
    {
   
    }

    private void HandleItemEndDrag(InventoryItem item)
    {
   
    }

    private void HandleItemBeginDrag(InventoryItem item)
    {
  
    }

    private void HandleItemSelection(InventoryItem item)
    {
        Debug.Log(item.item.itemName + " selected.");
    }

    #endregion


}
