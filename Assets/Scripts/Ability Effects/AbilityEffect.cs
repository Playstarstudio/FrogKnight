using UnityEngine;

[CreateAssetMenu(fileName = "AbilityEffect", menuName = "Scriptable Objects/AbilityEffect")]
public class AbilityEffect : ScriptableObject
{
    public string effectName;
    public string description;
    public float duration;
    public float tickRate;
    public bool isDebuff;
    public AttributeModifier[] mods;
    public float effectValue;
    public float finalEffect;
    public AttributeModifier finalModifier;

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
        if (mods.Length > 0)
        {
            CalculateEffect(source, ability);
            finalModifier.attModValue = finalEffect;
        }

        // Base effect logic
        Debug.Log($"{effectName} applied to {target.name} by {source.name}");
        // Implement effect application logic here. 

        //TODO: Make a whole effect receipt system on entities
        target.ReceiveEffect(this, source, ability);
        return;
    }

    private void CalculateEffect(Entity source, Ability ability)
    {
        float previousValue = effectValue;
        float sumToAdd = 0f;
        float multiplier = 1f;
        foreach (AttributeModifier mod in mods)
        {
            float playerAtt = source.att.GetCurrentAttributeValue(mod.attribute);
            switch (mod.operation)
            {
                case (AttributeModifier.Operator.Add):
                    {
                        sumToAdd += playerAtt;
                        continue;
                    }
                case (AttributeModifier.Operator.Subtract):
                    {
                        sumToAdd -= playerAtt;
                        continue;
                    }
                case (AttributeModifier.Operator.Multiply):
                    {
                        sumToAdd += playerAtt * mod.attModValue;
                        continue;
                    }
                case (AttributeModifier.Operator.Divide):
                    {
                        sumToAdd += playerAtt * mod.attModValue;
                        continue;
                    }
                case (AttributeModifier.Operator.Set):
                    {
                        sumToAdd = playerAtt;
                        continue;
                    }
            }
        }
        finalEffect = (effectValue + sumToAdd) * multiplier;
    }
}
