using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

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
    public List<Entity> targets;

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
        targets.Clear();
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
        Vector2Int castCenter = source.gridManager.GetCellPosition(castLoc);
        switch (targetType)
        {
            case TargetType.Direct:
                if (targetSubType == TargetSubType.AOE)
                {
                    List<Vector2Int> tilesTargeted = GetAllCellsInArea(source, castCenter);
                    foreach (Vector2Int tile in tilesTargeted)
                    {
                        targets.Add(source.gridManager.GetEnemyOnTile(tile));
                    }
                }
                else if (targetSubType == TargetSubType.Target)
                {
                    targets.Add(source.gridManager.GetEnemyOnTile(castCenter));  
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

    private List<Vector2Int> GetAllCellsInArea(Entity source, Vector2Int castCenter)
    {
        List<Vector2Int> tilesInRadius = new List<Vector2Int>();
        float radius = areaMod;
        float radiusSquared = areaMod * areaMod;

        int minX = castCenter.x - Mathf.CeilToInt(radius);
        int maxX = castCenter.x + Mathf.CeilToInt(radius);
        int minY = castCenter.y - Mathf.CeilToInt(radius);
        int maxY = castCenter.y + Mathf.CeilToInt(radius);

        for (int x = minX; x <= maxX; x++)
        {
            for (int y = minY; y <= maxY; y++)
            {
                Vector2Int tilePos = new Vector2Int(x, y);
                //float distSq = source.gridManager.ManhattanDistanceToTile(castCenter, tilePos);
                float distSq = (tilePos - castCenter).sqrMagnitude;
                if (distSq <= radiusSquared)
                {
                    tilesInRadius.Add(tilePos);
                }
            }
        }
        return tilesInRadius;
    }

    private void ApplyAbilityEffects(Entity source)
    {   //apply each effect to each target entity
        foreach (Entity target in targets)
        {
            foreach (AbilityEffect effect in abilityEffects)
            {
                // Apply each effect to the target entity
                if (target != null)
                {
                    effect.ApplyEffect(target, source, this);
                }
            }
        }
        if (targets.Count == 0)
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
