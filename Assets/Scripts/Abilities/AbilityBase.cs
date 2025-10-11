using UnityEngine;

[CreateAssetMenu(fileName = "Ability", menuName = "Scriptable Objects/Ability")]
public class Ability : ScriptableObject
{
    public string abilityName;
    public string description;
    public float manaCost;
    public int range;
    public int areaMod;
    public float speed;
    public float coolDown;
    public float critMod;
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
        Bounce
    }
    public virtual bool TryCastAbility(P_StateManager player, Vector2 targetPosition)
    {
        // Base spell logic (e.g., reduce mana)
        // if(source = player)
        // GameObject source = player.gameObject;
        player.castSuccess = false;
        float mana = player.p_Att.GetBaseAttributeValue(player.p_Att.GetAttributeType("MP"));
        if (mana >= manaCost)
        {
            AttributeModifier manaCostModifier = new AttributeModifier()
            {
                attribute = player.p_Att.GetAttributeType("MP"),
                operation = AttributeModifier.Operator.Subtract,
                attributeModifierValue = manaCost
            };
            player.p_Att.ApplyInstantModifier(manaCostModifier);
            Debug.Log($"{abilityName} cast towards {targetPosition}");
            player.gameManager.PlayerAction(player, speed);
            crit = TryCrit(player.gameObject);
            getTargets(player.gameObject, targetPosition);
            ApplyAbilityEffects(player.gameObject);
            player.castSuccess = true;
            return true;
        }
        /*
        else if (!validTarget)
        {
            Debug.Log("Invalid target for ability.");
            player.castSuccess = false;
            return false;
        }
         */
        else if (mana <= manaCost)
        {
            Debug.Log("Not enough MP to cast the ability.");
            player.castSuccess = false;
            return false;
        }
        else
            return false;
    }
    void getTargets(GameObject source, Vector2 castLoc)
    {
        switch (targetType)
        {
            case TargetType.Direct:
                if (targetSubType == TargetSubType.AOE)
                {
                    colliders = Physics2D.OverlapAreaAll(castLoc - new Vector2(areaMod, areaMod), castLoc + new Vector2(areaMod, areaMod));
                }
                else
                {
                    colliders = Physics2D.OverlapPointAll(castLoc);
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

    private void ApplyAbilityEffects(GameObject source)
    {   //apply each effect to each target entity
        foreach (Collider2D collider in colliders)
        {
            Enemy enemy = collider.gameObject.GetComponent<Enemy>();
            foreach (AbilityEffect effect in abilityEffects)
            {
                // Apply each effect to the target entity
                if (enemy != null)
                {
                    effect.ApplyEffect(enemy, source, this);
                }
            }
        }
        if (colliders.Length == 0)
        {
            Debug.Log("No targets hit.");
        }
    }

    bool TryCrit(GameObject source)
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
