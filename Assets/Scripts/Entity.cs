using Inventory.Model;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GridManager;

public abstract class Entity : MonoBehaviour
{
    #region Top Level Requirements
    // keep a ref to the gridmanager
    public GridManager gridManager;
    // data structures for pathfinding
    public List<AStarNodeInfo> path;
    public Dictionary<AStarNodeInfo, AStarNodeInfo> aStarSearchedList;
    public SortedSet<AStarNodeInfo> aStarToSearch;
    public GameManager gameManager;
    public GameLogManager gameLogManager;
    [HideInInspector] public Rigidbody rb;
    [HideInInspector] public Animator anim;
    [HideInInspector] public Vector2Int currentPos;
    [HideInInspector] public float gridSize;
    #endregion  

    #region Play Components
    public float readyTime;
    [SerializeField] public AttributeSet att;
    [HideInInspector] public bool visionBlock = false;
    [HideInInspector][SerializeField] public bool castSuccess = false;
    [HideInInspector][SerializeField] public Ability casting;
    [HideInInspector][SerializeField] public AbilitySlot castingSlot;
    [SerializeField] public Ability melee;
    [SerializeField] public List<Ability> activeAbilityList;
    [SerializeField] public List<Ability> totalKnownAbilities;
    [HideInInspector][SerializeField] public float movementSpeed;
    [HideInInspector] public float lastMoveTime = 0f;
    public AudioSource BGM;
    public AudioSource SoundEffect;
    public InventorySO inventory;
    [HideInInspector] public Vector2Int currentTile;
    [HideInInspector] public Vector2Int targetingTile;
    #endregion

    private void Awake()
    {
    }

    private void Start()
    {
        currentTile = gridManager.GetCellPosition(this.transform.position);
        this.transform.position = gridManager.GetTileCenter(gridManager.GetCellPosition(this.transform.position));
        gridManager.MapAddEntity(this, currentTile);
    }

    public void ReceiveEffect(AbilityEffect effect, Entity source, Ability ability)
    {
        
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
            attModValue = effect.finalEffect
        };
        this.att.ApplyInstantModifier(damage);
        Debug.Log($"{this.name} takes {effect.finalEffect} damage.");
        if (this.att.GetBaseAttributeValue(att.GetAttributeType("HP")) <= 0)
        {
            Debug.Log($"{this.name} has been defeated!");
            TryDestroy();
        }
    }
    public void ReceiveBuff(AbilityEffect effect, Entity source, Ability ability)
    {
            ApplyBuffOrDebuff(effect);
    }


    public void ReceiveDebuff(AbilityEffect effect, Entity source, Ability ability)
    {
            ApplyBuffOrDebuff(effect);
    }
    public void ReceiveCrowdControl(AbilityEffect effect, Entity source, Ability ability)
    {
        Debug.Log($"{this.name} is affected by crowd control.");
    }

    private void ApplyBuffOrDebuff(AbilityEffect effect)
    {
        this.att.ApplyModifier(effect.finalModifier);
        StartCoroutine(WaitForValueRoutine(effect, effect.duration));
    }

    private IEnumerator WaitForValueRoutine(AbilityEffect effect, float duration)
    {
        float timeAtStart = gameManager.globalTimer;
        float finishTime = timeAtStart + duration;
        yield return new WaitUntil(() => gameManager.globalTimer >= finishTime);
        this.att.RemoveModifier(effect.finalModifier);
    }
    public abstract void TryDestroy();
}
