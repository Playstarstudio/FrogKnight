using System.Collections.Generic;
using System.Linq;
using static GridManager;
using UnityEngine;

public class DialogueTrigger : MonoBehaviour //Validates and triggers dialogue
{
    [HideInInspector] public GameObject visualCue;

    [Header("Ink JSON")]
    public TextAsset inkJSON; //The compiled JSON of an Ink dialogue file

    [Header("Area Trigger Toggle")]
    public bool areaTrigger = false; //Controls whether the dialogue triggers on area or not
    [HideInInspector] public bool playerInRange;
    [HideInInspector] public DialogueManager dialogueManager;
    [HideInInspector] public P_StateManager p_StateManager;

    private void Awake() //On wake, ensures that the visual cue is off and the variables are set properly
    {
        playerInRange = false;
        visualCue.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collider) //Checks to see if, upon a collider entrance, the object entering is the player, and if so, detects them for DIALOGUE
    {
        if (collider.CompareTag("Player")) //Ensures it is player
        {
            playerInRange = true;   //Communicates to other scripts that the player is in range of this NPC
            dialogueManager.dialogueTrigger = this; //Makes the dialogue manager focus on this NPC
            p_StateManager = collider.gameObject.GetComponent<P_StateManager>(); //Gets state manager
            if (areaTrigger) //If this is an area trigger, complete same functionality as if player entered dialogue state manually upon entering the 2D collider
            {
                if (dialogueManager.dialogueCheck()) //starts dialogue through the dialogue check
                {
                    p_StateManager.SwitchState(p_StateManager.dialogueState); //changes to dialogue state
                }
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collider) //Checks to see if, upon a collider exit, the object exiting is the player to turn off dialogue capabilities if needed
    {
        if (collider.CompareTag("Player"))
        {
            if (dialogueManager.dialogueTrigger==this)
            {
                dialogueManager.dialogueTrigger=null;
            }
            playerInRange = false;
        }
    }

    private void Update() //Updates enemy visual cue for dialogue depending on player range
    {
        if (playerInRange && !dialogueManager.dialogueIsPlaying && !areaTrigger)
        {
            visualCue.SetActive(true);

        } else
        {
            visualCue.SetActive(false);
        }
    }
    
}
