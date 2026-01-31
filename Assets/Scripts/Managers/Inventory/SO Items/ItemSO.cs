using Inventory;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "ItemSO", menuName = "Scriptable Objects/ItemSO")]

public abstract class ItemSO : ScriptableObject
{
    public int ID => GetInstanceID();
    private InventoryManager inventoryManager;
    [field: SerializeField]
    public string itemName { get; set; }
    [field: SerializeField]
    public Sprite image { get; set; }
    [field: SerializeField]
    [field: TextArea]
    public string description { get; set; }
    [field: SerializeField]
    public float weight { get; set; }
    [field: SerializeField]
    public bool isStackable { get; set; }
    [field: SerializeField]
    public int MaxStackSize { get; set; } = 1;
    [field: SerializeField]
    public int count { get; set; } = 1;
    [field: SerializeField]
    public List<Modifier> effects;
    //public List<Item> materials;
    //public List<Modifier> finalModifiers;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public void Awake()
    {
    }
}
