using System;
using System.Buffers;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "Scriptable Objects/Item")]

public class Item : ScriptableObject
{
    public Sprite icon;
    public string itemName;
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
