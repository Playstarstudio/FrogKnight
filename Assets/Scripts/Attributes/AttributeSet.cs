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
    // this list shows up in the Inspector and will be populated with default attributes (by Reset function)
    [SerializeField]
    private List<AttributeEntry> attributes = new List<AttributeEntry>();

    private Dictionary<AttributeType, Attribute> _attributeDictionary = new Dictionary<AttributeType, Attribute>();
    public Dictionary<AttributeType, Attribute> attributeDictionary { get => _attributeDictionary; set => _attributeDictionary = value; }
    public void Reset()
    {
        // clear any existing values
        attributes.Clear();

        // Iterate through every AttributeType and create a new Attribute instance for each
        foreach (AttributeType type in Resources.LoadAll<AttributeType>(""))
        {
            attributes.Add(new AttributeEntry { type = type, value = new Attribute(type.DefaultValue)});
        }
    }
    private void Awake()
    {
        // ensure the dictionary is populated before gameplay.
        InitializeAttributeDictionary();
        InitializeMaxAttributes();
    }

    /// <summary>
    /// Initializes the dictionary from the serialized list.
    /// </summary>
    public void InitializeAttributeDictionary()
    {
        _attributeDictionary = new Dictionary<AttributeType, Attribute>();
        foreach (var entry in attributes)
        {
            // This check prevents duplicate keys in case of user error.
            if (!attributeDictionary.ContainsKey(entry.type))
            {
                attributeDictionary.Add(entry.type, entry.value);
            }
        }
    }

    private void InitializeMaxAttributes()
    {
        foreach (var entry in attributeDictionary)
        {
            attributeDictionary.TryGetValue(entry.Key, out var attribute);
            if (entry.Key.MaxAttribute != null && attribute != null)
            {
                attributeDictionary.TryGetValue(entry.Key.MaxAttribute, out var maxAttribute);
                attribute.SetMaxAttribute(maxAttribute);
            }
        }
    }

    public void UpdateCurrentValues()
    {
        foreach (var pair in attributeDictionary)
        {
            pair.Value.UpdateCurrentValue();
        }
    }

    public void ApplyModifier(AttributeModifier modifier)
    {
        if (attributeDictionary.TryGetValue(modifier.attribute, out var attribute))
        {
            attribute.AddModifier(modifier);
        }
        else
        {
            Debug.LogWarning("Attribute not found: " + modifier.attribute);
        }
    }

    public void RemoveModifier(AttributeModifier modifier)
    {
        if (attributeDictionary.TryGetValue(modifier.attribute, out var attribute))
        {
            attribute.RemoveModifier(modifier);
        }
        else
        {
            Debug.LogWarning("Attribute not found: " + modifier.attribute);
        }
    }

    public void ApplyInstantModifier(AttributeModifier modifier)
    {
        if (attributeDictionary.TryGetValue(modifier.attribute, out var attribute))
        {
            attribute.InstantlyApply(modifier);
        }
        else
        {
            Debug.LogWarning("Attribute not found: " + modifier.attribute);
        }
    }

    public bool HasAttribute(AttributeType type)
    {
        return attributeDictionary.ContainsKey(type);
    }

    public Attribute GetAttribute(AttributeType type)
    {
        if (attributeDictionary.TryGetValue(type, out var attribute))
        {
            return attribute;
        }
        Debug.LogWarning("Attribute not found: " + type);
        return null;
    }
    public float GetCurrentAttributeValue(AttributeType type)
    {
        if (attributeDictionary.TryGetValue(type, out var attribute))
        {
            return attribute.CurrentValue;
        }
        else
        {

            Debug.LogError("Attribute not found: " + type);
            return 1;
        }
    }

    public float GetBaseAttributeValue(AttributeType type)
    {
        if (attributeDictionary.TryGetValue(type, out var attribute))
        {
            return attribute.BaseValue;
        }
        Debug.LogError("Attribute not found: " + type);
        return 0f;
    }

    public AttributeType GetAttributeType(string typeName)
    {
        foreach (var entry in attributeDictionary)
        {
            if (entry.Key.name == typeName)
            {
                return entry.Key;
            }
        }
        Debug.LogWarning("AttributeType not found: " + typeName);
        return null;
    }
}
