using Inventory;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class P_StateManager : Entity
{
    #region SpellCasting
    #endregion
    #region States
    public string stateName;
    public P_State currentState;
    public P_State previousState;
    public P_BaseState baseState = new P_BaseState();
    public P_AbilityState abilityState = new P_AbilityState();
    public P_InventoryState inventoryState = new P_InventoryState();
    public P_DialogueState dialogueState = new P_DialogueState();
    public List<AbilitySlot> abilitySlots = new List<AbilitySlot>();
    /*
    public P_CharacterState characterstate = new P_CharacterState();
    
    public P_PausedState pausedState = new P_PausedState();
    public P_OverworldState p_OverworldState = new P_OverworldState();
    public P_CraftingState p_CraftingState = new P_CraftingState();
    public P_ShopState p_ShopState = new P_ShopState();
    */

    #endregion
    public List<Vector2Int> rangeTile;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public InventoryManager inventoryManager;
    public DialogueManager dialogueManager;
    public Transform mainCamera;
    /*
    public AudioSource SoundEffect1;
    public AudioSource SoundEffect2;
    public AudioSource SoundEffect3;
    public AudioSource otherSoundsAudioSource;
     */
    /*
    public AudioClip[] breathingClips;
    public AudioClip[] backgroundMusicClips;
    public AudioClip idCardClip, laserCutterClip, sealantSprayClip, oxygenRefillClip,
            landingClip, crawlingClip, oxygenBoostingClip, grabbingClip, suffocationClip, jumpClip;
    */

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        anim = GetComponentInChildren<Animator>();
        currentState = baseState;
        currentState.EnterState(this);
        gameManager = FindFirstObjectByType<GameManager>();
        gridManager = FindFirstObjectByType<GridManager>();
        dialogueManager = FindFirstObjectByType<DialogueManager>();
        this.transform.position = gridManager.GetTileCenter(gridManager.GetCellPosition(this.transform.position));
        for (int i = 0; i < activeAbilityList.Count; i++)
        {
            abilitySlots[i].ability = activeAbilityList[i];
            abilitySlots[i].image.sprite = activeAbilityList[i].abilityImage;
        }
        StartCoroutine(WaitForTime(.1f));

    }

    // Update is called once per frame
    void Update()
    {
        currentState.UpdateState(this);
    }
    public void SwitchState(P_State state)
    {
        currentState.ExitState(this);
        previousState = currentState;
        currentState = state;
        state.EnterState(this);
    }
    public void SwitchToPreviousState()
    {
        if (previousState != null)
        {
            SwitchState(previousState);
        }
    }
    internal void CalculateAllStats()
    {
        foreach (EquipmentSlot equipmentSlot in inventoryManager.equipmentSlots)
        {
            if (equipmentSlot == null || equipmentSlot.IsEmpty)
            {
                continue;
            }
            {
                foreach (Modifier effect in equipmentSlot.item.effects)
                {
                    AttributeModifier mod = new AttributeModifier()
                    {
                        attribute = effect.attName,
                        operation = (AttributeModifier.Operator)effect.operation,
                        attModValue = effect.modifierValue
                    };
                    att.ApplyModifier(mod);
                }
            }
        }
        att.UpdateCurrentValues();
    }

    /*
    IEnumerator<Vector2> Move(Vector2 direction)
    {
        moving = true;
        Vector2 startPosition = transform.position;
        Vector2 target = startPosition + (direction * gridSize);
        Vector2Int endPosition = gridManager.GetCellPosition(target);
        if (!gridManager.TraversableCheck(endPosition))
        {
            moving = false;
            //bump
            yield break;
        }
        float elapsedTime = 0;
        while (elapsedTime < movementSpeed)
        {
            elapsedTime += Time.deltaTime;
            float percent = elapsedTime / movementSpeed;
            transform.position = Vector2.MoveTowards(startPosition, gridManager.GetTileCenter(endPosition), movementSpeed);
        }
        transform.position = target;
        while (gameManager.globalTimer < lastMoveTime + movementSpeed)
        {
            gameManager.incrementTime();
        }
        lastMoveTime = gameManager.globalTimer;
        Debug.Log("Start Pos:" + startPosition);
        Debug.Log("End Pos:" + endPosition);
        moving = false;
    }
     */
    private IEnumerator WaitForTime(float time)
{
        yield return new WaitForSeconds(time);
        gridManager.PlayerDijkstras();
        CalculateAllStats();
}

    #region Save and Load
    public void Save(ref PlayerSaveData data)
    {
        data.position = transform.position;
        data.att = att;
    }

    public void Load(PlayerSaveData data)
    {
        transform.position = data.position;
        att = data.att;
    }


    #endregion
}

[System.Serializable]
public struct PlayerSaveData
{
    public Vector2 position;
    public AttributeSet att;
    // Add other player-related data as needed
    // https://www.youtube.com/watch?v=1mf730eb5Wo
}
