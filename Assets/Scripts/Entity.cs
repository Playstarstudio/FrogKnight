using System.Collections.Generic;
using UnityEngine;
using static GridManager;

public class Entity : MonoBehaviour
{
    #region Top Level Requirements
    // keep a ref to the gridmanager
    public GridManager gridManager;
    // data structures for pathfinding
    public List<AStarNodeInfo> path;
    public Dictionary<AStarNodeInfo, AStarNodeInfo> aStarSearchedList;
    public SortedSet<AStarNodeInfo> aStarToSearch;
    public GameManager gameManager;
    [HideInInspector] public Rigidbody rb;
    [HideInInspector] public Animator anim;
    public float gridSize;
    #endregion  

    #region Play Components
    public float readyTime;
    [SerializeField] public AttributeSet att;
    public bool visionBlock = false;
    [SerializeField] public bool castSuccess = false;
    [SerializeField] public AbilitySlot casting;
    [SerializeField] public AbilitySlot spell0;
    [SerializeField] public AbilitySlot spell1;
    [SerializeField] public AbilitySlot spell2;
    [SerializeField] public AbilitySlot spell3;
    [SerializeField] public float movementSpeed;
    public AttributeModifier hurt;
    public bool isRepeatedMovement = false;
    public bool huurt = false;
    public bool moving = false;
    public float lastMoveTime = 0f;
    public AudioSource BGM;
    public AudioSource SoundEffect;
    public InventorySO inventory;
    #endregion


    public void ReceiveEffect(AbilityEffect effect, Entity source, Ability ability)
    {
        // Implement effect reception logic here
        Debug.Log($"{this.name} received {effect.effectName} from {source.name} via {ability.abilityName}");
        switch(effect.effectType)
        {
            case(AbilityEffect.EffectType.Damage):
                ReceiveDamage(effect, source, ability);
                break;
            case (AbilityEffect.EffectType.Heal):
                ReceiveHeal(effect, source, ability);
                break;
            case (AbilityEffect.EffectType.Buff):
                ReceiveBuff(effect, source, ability);
                break;
            case (AbilityEffect.EffectType.Debuff):
                ReceiveDebuff(effect, source, ability);
                break;
            case (AbilityEffect.EffectType.CrowdControl):
                ReceiveCrowdControl(effect, source, ability);
                break;
            default:
                Debug.Log("Unknown effect type.");
                break;
        }
    }

    public void ReceiveDamage(AbilityEffect effect, Entity source, Ability ability)
    {
        this.att.GetBaseAttributeValue(att.GetAttributeType("HP"));
        AttributeModifier damage = new AttributeModifier()
        {
            attribute = source.att.GetAttributeType("HP"),
            operation = AttributeModifier.Operator.Subtract,
            attributeModifierValue = effect.effectValue
        };
        this.att.ApplyInstantModifier(damage);
        Debug.Log($"{this.name} takes {effect.effectValue} damage.");
        if(this.att.GetBaseAttributeValue(att.GetAttributeType("HP")) <= 0)
        {
            Debug.Log($"{this.name} has been defeated!");
            Destroy(this.gameObject);
        }
    }
    public void ReceiveHeal(AbilityEffect effect, Entity source, Ability ability)
    {
        Debug.Log($"{this.name} heals {effect.effectValue} health.");
    }
    public void ReceiveBuff(AbilityEffect effect, Entity source, Ability ability)
    {
        Debug.Log($"{this.name} receives a buff of {effect.effectValue}.");
    }
    public void ReceiveDebuff(AbilityEffect effect, Entity source, Ability ability)
    {
        Debug.Log($"{this.name} receives a debuff of {effect.effectValue}.");
    }
    public void ReceiveCrowdControl(AbilityEffect effect, Entity source, Ability ability)
    {
        Debug.Log($"{this.name} is affected by crowd control.");
    }
}
