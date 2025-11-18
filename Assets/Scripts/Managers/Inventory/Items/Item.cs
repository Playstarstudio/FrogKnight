using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "Item", menuName = "Scriptable Objects/Item")]

public class Item : ScriptableObject
{
    private InventoryManager inventoryManager;
    [SerializeField]
    public string itemName;
    [SerializeField]
    public Image image;
    public string description;
    public float weight;
    public bool stackable;
    public int count;
    public List<Modifier> effects;
    //public List<Item> materials;
    //public List<Modifier> finalModifiers;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public void Awake()
    {
    }
    void Start()
    {
        inventoryManager = GameObject.Find("InventoryCanvas").GetComponent<InventoryManager>();
        /*
        finalModifiers.Clear();
        finalModifiers.AddRange(effects);
        foreach(Item material in materials)
        {
            finalModifiers.AddRange(material.effects);
        }
         */
    }


}
