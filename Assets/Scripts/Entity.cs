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
    [SerializeField] public AttributeSet p_Att;
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
    #endregion


    public void ReceiveEffect(AbilityEffect effect, Entity source, Ability ability)
    {
        // Implement effect reception logic here
        Debug.Log($"{this.name} received {effect.effectName} from {source.name} via {ability.abilityName}");
        Destroy(this.gameObject);
        switch(effect.effectType)
        {
            case(AbilityEffect.EffectType.Damage):
                // Apply damage logic
                Debug.Log($"{this.name} takes {effect.effectValue} damage.");
                break;
            case (AbilityEffect.EffectType.Heal):
                // Apply healing logic
                Debug.Log($"{this.name} heals {effect.effectValue} health.");
                break;
            case (AbilityEffect.EffectType.Buff):
                // Apply buff logic
                Debug.Log($"{this.name} receives a buff of {effect.effectValue}.");
                break;
            case (AbilityEffect.EffectType.Debuff):
                // Apply debuff logic
                Debug.Log($"{this.name} receives a debuff of {effect.effectValue}.");
                break;
            case (AbilityEffect.EffectType.CrowdControl):
                // Apply crowd control logic
                Debug.Log($"{this.name} is affected by crowd control.");
                break;
            default:
                Debug.Log("Unknown effect type.");
                break;
        }
    }
}
