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

    public bool playerInRange;
    public bool areaTrigger = false;
    public DialogueManager dialogueManager;
    public P_StateManager p_StateManager;
    public GameObject player;


    private void Awake() //On wake, ensures that the visual cue is off and the variables are set properly
    {
        playerInRange = false;
        visualCue.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collider) //Checks to see if, upon a collider entrance, the object entering is the player, and if so, detects them for DIALOGUE
    {
        if (collider.CompareTag("Player"))
        {
            p_StateManager = collider.gameObject.GetComponent<P_StateManager>();
            player = collider.gameObject;
            playerInRange = true;
            dialogueManager.dialogueTrigger = this;
            if (areaTrigger)
            {
                dialogueManager.dialogueCheck();
                p_StateManager.SwitchState(p_StateManager.dialogueState);
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
            //collider.GetComponent<P_BaseState>
        }
    }

    private void Update() //Updates enemy visual cue for dialogue depending on player range
    {
        if (playerInRange && !dialogueManager.dialogueIsPlaying)
        {
            visualCue.SetActive(true);

        } else
        {
            visualCue.SetActive(false);
        }
    }
}
