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
    [SerializeField]
    public PartLocation slot;

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
    public List<Modifier> effects { get; set; }
    //public List<Item> materials;
    //public List<Modifier> finalModifiers;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public enum PartLocation
    {
        PrimaryHand, //0
        OffHand, //1
        TorsoClothing, //2
        TorsoArmor, //3
        Belt, //4
        Glove, //5
        LegsClothing, //6
        LegsArmor, //7
        Foot, //8
        Head, //9
        Face, //10
        Amulet, //11
        Ring, //12
        Backpack, //13
        Quiver, //14
        NA, //15
        None
    }

}
