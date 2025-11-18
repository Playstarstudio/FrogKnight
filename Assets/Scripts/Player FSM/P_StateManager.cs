using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class P_StateManager : Entity
{

    #region States
    public string stateName;
    public P_State currentState;
    public P_State previousState;
    public P_BaseState baseState = new P_BaseState();
    public P_AbilityState abilityState = new P_AbilityState();
    public P_InventoryState inventoryState = new P_InventoryState();
    /*
    public P_CharacterState characterstate = new P_CharacterState();
    public P_DialogueState dialogueState = new P_DialogueState();
    public P_PausedState pausedState = new P_PausedState();
    public P_OverworldState p_OverworldState = new P_OverworldState();
    public P_CraftingState p_CraftingState = new P_CraftingState();
    public P_ShopState p_ShopState = new P_ShopState();
    */

    #endregion
    public List<Vector2Int> rangeTile;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public InventoryManager inventoryManager;
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
