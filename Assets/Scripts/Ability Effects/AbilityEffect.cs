using UnityEngine;

[CreateAssetMenu(fileName = "AbilityEffect", menuName = "Scriptable Objects/AbilityEffect")]
public class AbilityEffect : ScriptableObject
{
    public string effectName;
    public string description;
    public float effectValue;
    public float duration;
    public float tickRate;
    public bool isDebuff;
    public EffectType effectType;
    public enum EffectType
    {
        Damage,
        Heal,
        Buff,
        Debuff,
        CrowdControl
    }

    public virtual void ApplyEffect(Enemy target, GameObject source, Ability ability)
    {
        // Base effect logic
        Debug.Log($"{effectName} applied to {target.name} by {source.name}");
        // Implement effect application logic here
        target.ReceiveEffect(this, source, ability);
    }
}
