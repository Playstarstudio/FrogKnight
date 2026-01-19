using System.Collections.Generic;
using System.Linq;
using static GridManager;
using UnityEngine;

[CreateAssetMenu(fileName = "DialogueEnemy", menuName = "Scriptable Objects/DialogueEnemy")]
public class DialogueEnemy : MonoBehaviour
{
    [HeaderAttribute("Visual Cue")]
    public GameObject visualCue;

    [Header("Ink JSON")]
    public TextAsset inkJSON;

    public bool playerInRange;


    private void Awake() //On wake, ensures that the visual cue is off and the variables are set properly
    {
        playerInRange = false;
        visualCue.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collider) //Checks to see if, upon a collider entrance, the object entering is the player, and if so, detects them for DIALOGUE
    {
        if (collider.CompareTag("Player"))
        {
            playerInRange = true;
            DialogueManager.instance.dialogueEnemy = this;
            
        }
    }

    private void OnTriggerExit2D(Collider2D collider) //Checks to see if, upon a collider exit, the object exiting is the player to turn off dialogue capabilities if needed
    {
        if (collider.CompareTag("Player"))
        {
            if (DialogueManager.instance.dialogueEnemy==this)
            {
                DialogueManager.instance.dialogueEnemy=null;
            }
            playerInRange = false;
            //collider.GetComponent<P_BaseState>
        }
    }

    private void Update() //Updates enemy visual cue for dialogue depending on player range
    {
        if (playerInRange && !DialogueManager.GetInstance().dialogueIsPlaying)
        {
            visualCue.SetActive(true);

        } else
        {
            visualCue.SetActive(false);
        }
    }
}
