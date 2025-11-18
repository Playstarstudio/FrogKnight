using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;

public class UIInventoryPage :MonoBehaviour
{
    [SerializeField] private InventoryItem itemPrefab;
    [SerializeField] private RectTransform contentPanel;
    List<InventoryItem> listofUIItems = new List<InventoryItem>();
/*
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
        gameObject.SetActive(false);
    }
 */

}
