using System;
using System.Buffers;
using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "Scriptable Objects/Item")]

public class Item : ScriptableObject
{
    public Entity owner;
    public string itemName;
    public string description;
    public float weight;
    public bool stackable;
    public int count;
    public Modifier[] effects;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    }
