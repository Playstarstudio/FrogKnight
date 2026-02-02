using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AbilityEffect", menuName = "Scriptable Objects/AbilityEffect")]
public class AbilityEffect : ScriptableObject
{
    public string effectName;
    public string description;
    public AttributeModifier mod;
    public float duration;
    public float tickRate;
    public bool isDebuff;
    //affected attritbutes[] attibutesAffected;
    /// <summary>
    /// List of attribute modifiers this effect applies
    /// </summary>
    ///public List<AttributeModifier> attributeModifiers = new List<AttributeModifier>();
    public EffectType effectType;
    public enum EffectType
    {
        Damage,
        Buff,
        Debuff,
        CrowdControl
    }

    public virtual void ApplyEffect(Entity target, Entity source, Ability ability)
    {
        // Base effect logic
        Debug.Log($"{effectName} applied to {target.name} by {source.name}");
        // Implement effect application logic here. 

        //TODO: Make a whole effect receipt system on entities
        target.ReceiveEffect(this, source, ability);
    }
}
