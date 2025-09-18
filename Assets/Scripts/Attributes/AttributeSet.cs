using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]

public class AttributeEntry
{
public AttributeType type;
public Attribute value;
}


public class AttributeSet : MonoBehaviour
{
    [SerializeField] 
    private List<AttributeEntry> attributes = new List<AttributeEntry>();


    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
