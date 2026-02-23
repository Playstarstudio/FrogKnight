using System.Collections.Generic;
using System.Linq;
using static GridManager;
using UnityEngine;

//[CreateAssetMenu(fileName = "DialogueEnemy", menuName = "Scriptable Objects/DialogueEnemy")]
public class DialogueTrigger : MonoBehaviour
{
    [HeaderAttribute("Visual Cue")]
    public GameObject visualCue;

    [Header("Ink JSON")]
    public TextAsset inkJSON;

    [Header("Area Trigger Toggle")]
    public bool areaTrigger = false;
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
        if (collider.CompareTag("Player")) //ensure it is player
        {
            playerInRange = true;   //communicates to other scripts that the player is in range of this NPC
            dialogueManager.dialogueTrigger = this; //makes the dialogue manager focus on this NPC
            p_StateManager = collider.gameObject.GetComponent<P_StateManager>(); //gets state manager
            if (areaTrigger) //If this is an area trigger, complete same functionality as if player entered dialogue state manually upon entering the 2D collider
            {
                if (dialogueManager.dialogueCheck())
                {
                    p_StateManager.SwitchState(p_StateManager.dialogueState);
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
