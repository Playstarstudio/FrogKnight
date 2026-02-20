using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Attribute
{
    public Attribute() { }

    public Attribute(float value)
    {
        baseValue = value;
    }
    /// <summary>
    /// Action Invoked whenever the CurrentValue of this Attribute changes
    /// </summary>
    public event Action<Attribute, float> OnValueChanged;

    /// <summary>
    /// Dictionary of Attribute modifiers
    /// </summary>
    private List<AttributeModifier> _modifiers = new List<AttributeModifier>();

    /// <summary>
    /// base value of this attribute before modifiers
    /// </summary>
    [SerializeField] private float baseValue;

    //Attribute representing the max base value of this attribute
    private Attribute _maxBaseAttribute;

    /// <summary>
    /// Base Value of the attribute before modifiers
    /// </summary>
    public float BaseValue
    {
        get
        {
            return baseValue;
        }
        set
        {
            float previous = CurrentValue;
            if (_maxBaseAttribute != null)
            {
                baseValue = Mathf.Min(value, _maxBaseAttribute.CurrentValue);
            }
            else
            {
                baseValue = value;
            }

            UpdateCurrentValue();
        }
    }

    /// <summary>
    /// Current Value of the attribute after all modifiers
    /// </summary>
    public float CurrentValue { get; private set; }

    public Attribute GetMaxAttribute()
    {
        return _maxBaseAttribute;
    }

    public void SetMaxAttribute(Attribute modifiableAttributeValue)
    {
        _maxBaseAttribute = modifiableAttributeValue;
    }

    public void SetUpActions()
    {
        OnValueChanged += HandleValueChanged;
    }

    private void HandleValueChanged(Attribute attribute, float previousValue)
    {

    }

    /// <summary>
    /// Updates the current value of this attribute including all the modifiers affecting it
    /// </summary>
    public void UpdateCurrentValue()
    {
        float previousValue = CurrentValue;
        float sumToAdd = 0f;
        float multiplier = 1f;
        foreach (AttributeModifier modifier in _modifiers)
        {
            switch (modifier.operation)
            {
                case AttributeModifier.Operator.Add:
                    sumToAdd += modifier.attModValue;
                    continue;
                case AttributeModifier.Operator.Subtract:
                    sumToAdd -= modifier.attModValue;
                    continue;
                case AttributeModifier.Operator.Multiply:
                    multiplier *= modifier.attModValue;
                    continue;
                case AttributeModifier.Operator.Divide:
                    multiplier /= modifier.attModValue;
                    continue;
                default:
                    Debug.LogError("Unexpected Operator in attributeMod");
                    break;
            }
        }
        CurrentValue = (baseValue + sumToAdd) * multiplier;
        if (!Mathf.Approximately(previousValue, CurrentValue))
        {
            OnValueChanged?.Invoke(this, previousValue);
            Debug.Log("stat changed: " + this.GetType() + " Current Val = " + CurrentValue);
        }
    }

    /// <summary>
    /// Add a modifier that will change the value of the attribute
    /// </summary>
    /// <param name="modifier"></param>
    public void AddModifier(AttributeModifier modifier)
    {
        _modifiers.Add(modifier);
        UpdateCurrentValue();
    }
    /// <summary>
    /// Remove a modifier that was changing the value of the attribute
    /// </summary>
    /// <param name="modifier"></param>
    public void RemoveModifier(AttributeModifier modifier)
    {
        _modifiers.Remove(modifier);
        UpdateCurrentValue();
    }

    public void InstantlyApply(AttributeModifier modifier)
    {
        float previous = CurrentValue;
        switch (modifier.operation)
        {
            case AttributeModifier.Operator.Add:
                BaseValue += modifier.attModValue;
                break;
            case AttributeModifier.Operator.Subtract:
                BaseValue -= modifier.attModValue;
                break;
            case AttributeModifier.Operator.Multiply:
                BaseValue *= modifier.attModValue;
                break;
            case AttributeModifier.Operator.Divide:
                BaseValue /= modifier.attModValue;
                break;
            case AttributeModifier.Operator.Set:
                BaseValue = modifier.attModValue;
                return;
            default:
                Debug.LogError("Unexpected Operator in attributeMod");
                break;

        }
        UpdateCurrentValue();
    }
}
