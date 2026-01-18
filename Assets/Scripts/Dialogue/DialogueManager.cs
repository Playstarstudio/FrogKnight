using Ink.Runtime;
using TMPro;
using UnityEngine;

public class DialogueManager : MonoBehaviour
{
    [Header("Dialogue UI")]
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private TextMeshProUGUI dialogueText;

    private Story currentStory;

    private bool dialogueIsPlaying;
    
    private static DialogueManager instance;

    private void Awake()
    {
        if (instance != null) //Errors out if multiple Dialogue Managers
        {
            Debug.LogWarning("Found more than one Dialogue Manager in the scene");
        }
        instance = this;
    }

    private void Start()
    {
        dialogueIsPlaying = false;
        dialoguePanel.SetActive(false);
    }

    private void ExitDialogueMode() //Exits the dialogue
    {
        dialogueIsPlaying = false;
        dialoguePanel.SetActive(false);
    }

    /*private void Update()
    {
        if (!dialogueIsPlaying){ //Immediately return if no dialogue
            return;
        }

    }*/

    public static DialogueManager GetInstance() //Returns public instance of the dialogue manager
    {
        return instance;
    }

    public void EnterDialogueMode(TextAsset inkJSON)
    {
        currentStory = new Story(inkJSON.text);
        dialogueIsPlaying = true;
        dialoguePanel.SetActive(true);

        ContinueStory();
    }

    public void ContinueStory()
    {
        if (currentStory.canContinue)
        {
            dialogueText.text = currentStory.Continue();
        } else
        {
            ExitDialogueMode();
        }
    }

    public bool dialogueCheck()
    { //This will interact with the enemy script and check to see if within range of dialogue. If so, it will begin dialogue state if the enemy has dialogue capabilities. Placeholder code for now
        if (dialogueIsPlaying)
        {
            ContinueStory();
            return true;
        } else
        {
            ContinueStory();
            return false;
        }
    }
}
