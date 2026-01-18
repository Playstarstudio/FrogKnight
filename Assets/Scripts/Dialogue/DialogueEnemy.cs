using System.Collections.Generic;
using System.Linq;
using static GridManager;
using UnityEngine;

[CreateAssetMenu(fileName = "DialogueEnemy", menuName = "Scriptable Objects/DialogueEnemy")]
public class DialogueEnemy : MonoBehaviour
{
    [HeaderAttribute("Visual Cue")]
    [SerializeField] private GameObject visualCue;

    [Header("Ink JSON")]
    [SerializeField] private TextAsset inkJSON;

    private bool playerInRange;


    private void Awake() //On wake, ensures that the visual cue is off and the variables are set properly
    {
        playerInRange = false;
        visualCue.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collider) //Checks to see if, upon a collider entrance, the object entering is the player, and if so, detects them for DIALOGUE
    {
        if (collider.gameObject.tag == "Player")
        {
            playerInRange = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collider) //Checks to see if, upon a collider exit, the object exiting is the player to turn off dialogue capabilities if needed
    {
        if (collider.gameObject.tag == "Player")
        {
            playerInRange = false;
        }
    }

    private void Update() //Updates enemy visual cue for dialogue depending on player range
    {
        if (playerInRange)
        {
            visualCue.SetActive(true);

        } else
        {
            visualCue.SetActive(false);
        }
    }
}
