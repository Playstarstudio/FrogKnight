using System.Collections.Generic;
using UnityEngine;

namespace Inventory.UI
{
    public class UIInventoryPage : MonoBehaviour
    {
        [SerializeField] private InventoryItem itemPrefab;
        [SerializeField] private RectTransform contentPanel;
        [SerializeField] private InventoryDescription inventoryDescription;
        [SerializeField] private FloatingItem floatingItem;
        List<InventoryItem> listofUIItems = new List<InventoryItem>();
        [SerializeField] 
        private ItemActionPanel itemActionPanel;
        public void InitializeInventoryUI(int inventorySize)
        {
            for (int i = 0; i < inventorySize; i++)
            {
                InventoryItem newItem = Instantiate(itemPrefab, Vector3.zero, Quaternion.identity);
                newItem.transform.SetParent(contentPanel);
                listofUIItems.Add(newItem);


            }
        }

        public void Show()
        {
            gameObject.SetActive(true);
        }
        public void Hide()
        {
            itemActionPanel.Toggle(false);
            gameObject.SetActive(false);
        }
    }
}