using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class UIInventoryPage :MonoBehaviour
{
    [SerializeField] private ItemPrefabScript itemPrefab;
    [SerializeField] private RectTransform contentPanel;
    List<ItemPrefabScript> listofUIItems = new List<ItemPrefabScript>();

    public void InitializeInventoryUI(int inventorySize)
    {
        for (int i = 0; i < inventorySize; i++)
        {
            ItemPrefabScript newItem = Instantiate(itemPrefab, Vector3.zero, Quaternion.identity);
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
        gameObject.SetActive(false);
    }

}
