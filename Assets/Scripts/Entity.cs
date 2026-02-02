using Inventory.Model;
using System.Collections;
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
    [HideInInspector] public Vector2Int currentPos;
    public float gridSize;
    #endregion  

    #region Play Components
    public float readyTime;
    [SerializeField] public AttributeSet att;
    public bool visionBlock = false;
    [SerializeField] public bool castSuccess = false;
    [SerializeField] public Ability casting;
    [SerializeField] public AbilitySlot castingSlot;
    [SerializeField] public Ability melee;
    [SerializeField] public List<Ability> activeAbilityList;
    [SerializeField] public List<Ability> totalKnownAbilities;
    [SerializeField] public float movementSpeed;
    public AttributeModifier hurt;
    public bool isRepeatedMovement = false;
    public bool huurt = false;
    public bool moving = false;
    public float lastMoveTime = 0f;
    public AudioSource BGM;
    public AudioSource SoundEffect;
    public InventorySO inventory;
    public Vector2Int currentTile;
    public Vector2Int targetingTile;
    #endregion

    private void Awake()
    {
    }

    private void Start()
    {
        currentTile = gridManager.GetCellPosition(this.transform.position);
        this.transform.position = gridManager.GetTileCenter(gridManager.GetCellPosition(this.transform.position));

    }

    public void ReceiveEffect(AbilityEffect effect, Entity source, Ability ability)
    {
        // Implement effect reception logic here
        Debug.Log($"{this.name} received {effect.effectName} from {source.name} via {ability.abilityName}");
        switch (effect.effectType)
        {
            case (AbilityEffect.EffectType.Damage):
                ReceiveDamage(effect, source, ability);
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
            attModValue = effect.mod.attModValue
        };
        this.att.ApplyInstantModifier(damage);
        Debug.Log($"{this.name} takes {effect.mod.attModValue} damage.");
        if (this.att.GetBaseAttributeValue(att.GetAttributeType("HP")) <= 0)
        {
            Debug.Log($"{this.name} has been defeated!");
            Destroy(this.gameObject);
        }
    }
    public void ReceiveBuff(AbilityEffect effect, Entity source, Ability ability)
    {
            SmartApplyEffect(effect);
    }


    public void ReceiveDebuff(AbilityEffect effect, Entity source, Ability ability)
    {
            SmartApplyEffect(effect);
    }
    public void ReceiveCrowdControl(AbilityEffect effect, Entity source, Ability ability)
    {
        Debug.Log($"{this.name} is affected by crowd control.");
    }

    private void SmartApplyEffect(AbilityEffect effect)
    {
        AttributeModifier mod = new AttributeModifier()
        {
            attribute = effect.mod.attribute,
            operation = (AttributeModifier.Operator)effect.mod.operation,
            attModValue = effect.mod.attModValue
        };
        Debug.Log(mod);
        this.att.ApplyInstantModifier(mod);
        //StartCoroutine(WaitForValueRoutine(effect, mod));
    }

    private IEnumerator WaitForValueRoutine(AbilityEffect effect, AttributeModifier mod)
    {
        float timeAtStart = gameManager.globalTimer;
        yield return new WaitUntil(() => gameManager.globalTimer >= timeAtStart + effect.duration);

    }


}
