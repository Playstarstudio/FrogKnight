using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "Ability", menuName = "Scriptable Objects/Ability")]
public class Ability : ScriptableObject
{
    public string abilityName;
    public string description;
    public float manaCost;
    public int range;
    public int areaMod;
    public int numberMod;
    public float speed;
    public float coolDown;
    public float critMod;
    public bool enemyOnly;
    public bool crit;
    public TargetType targetType;
    public TargetSubType targetSubType;
    public AbilityEffect[] abilityEffects;
    public Collider2D[] colliders;

    public enum TargetType
    {
        Direct,
        Beam,
        Projectile,
        TogglePassive,
        FreePassive
    }
    public enum TargetSubType
    {
        Target,
        AOE,
        Chain,
        Bounce,
        MultiTarget
    }
    public virtual bool TryCastAbility(Entity source, Vector2 targetPosition)
    {
        // Base spell logic (e.g., reduce mana)
        // if(source = source)
        // GameObject source = source.gameObject;
        source.castSuccess = false;
        float mana = source.att.GetBaseAttributeValue(source.att.GetAttributeType("MP"));
        if (mana >= manaCost)
        {
            AttributeModifier manaCostModifier = new AttributeModifier()
            {
                attribute = source.att.GetAttributeType("MP"),
                operation = AttributeModifier.Operator.Subtract,
                attributeModifierValue = manaCost
            };
            source.att.ApplyInstantModifier(manaCostModifier);
            Debug.Log($"{abilityName} cast towards {targetPosition}");
            source.gameManager.PlayerAction(source, speed);
            crit = TryCrit(source);
            getTargets(source, targetPosition);
            ApplyAbilityEffects(source);
            source.castSuccess = true;
            return true;
        }
        /*
        else if (!validTarget)
        {
            Debug.Log("Invalid target for ability.");
            source.castSuccess = false;
            return false;
        }
         */
        else if (mana <= manaCost)
        {
            Debug.Log("Not enough MP to cast the ability.");
            source.castSuccess = false;
            return false;
        }
        else
            return false;
    }
    void getTargets(Entity source, Vector2 castLoc)
    {
        switch (targetType)
        {
            case TargetType.Direct:
                if (targetSubType == TargetSubType.AOE)
                {
                    colliders = Physics2D.OverlapAreaAll(castLoc - new Vector2(areaMod, areaMod), castLoc + new Vector2(areaMod, areaMod));
                }
                else if (targetSubType == TargetSubType.Target)
                {
                    colliders = Physics2D.OverlapPointAll(castLoc);
                }
                else
                {
                    Debug.Log("Direct target type with this subtype is not implemented.");
                }
                break;
            case TargetType.Beam:
                Debug.Log("Beam target type is not implemented.");
                break;
            case TargetType.Projectile:
                Debug.Log("Projectile target type is not implemented.");
                break;
            case TargetType.TogglePassive:
                Debug.Log("TogglePassive target type is not implemented.");
                break;
            case TargetType.FreePassive:
                Debug.Log("FreePassive target type is not implemented.");
                break;
            default:
                Debug.Log("Unknown target type.");
                break;
        }
    }

    private void ApplyAbilityEffects(Entity source)
    {   //apply each effect to each target entity
        foreach (Collider2D collider in colliders)
        {
            Entity target = collider.gameObject.GetComponent<Entity>();
            foreach (AbilityEffect effect in abilityEffects)
            {
                // Apply each effect to the target entity
                if (target != null)
                {
                    effect.ApplyEffect(target, source, this);
                }
            }
        }
        if (colliders.Length == 0)
        {
            Debug.Log("No targets hit.");
        }
    }

    bool TryCrit(Entity source)
    {
        float roll = Random.Range(0f, 1f);
        if (roll <= critMod)
        {
            crit = true;
            Debug.Log("Critical Hit!");
            return true;
        }
        else
        {
            crit = false;
            return false;
        }
    }
}
