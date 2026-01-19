using System.Collections;
using System.Collections.Generic;
using Ink.Runtime;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class DialogueManager : MonoBehaviour
{
    [Header("Dialogue UI")]
    public GameObject dialoguePanel;
    public TextMeshProUGUI dialogueText;
    [Header("Choices UI")]
    public GameObject[] choices;
    public TextMeshProUGUI[] choicesText;

    public static DialogueManager instance;
    public Story currentStory;
    public bool dialogueIsPlaying;

    public DialogueEnemy dialogueEnemy;
    private void Awake()
    {
        if (instance != null) //Errors out if multiple Dialogue Managers
        {
            Debug.LogWarning("Found more than one Dialogue Manager in the scene");
        }
        instance = this;
        //dialogueEnemy = Enemy.GetComponent<DialogueEnemy>();
        
    }

    private void Start()
    {
        ExitDialogueMode(); //disables dialogue panel and dialogueisplaying var

        choicesText = new TextMeshProUGUI[choices.Length];
        int index = 0;
        foreach (GameObject choice in choices)
        {
            choicesText[index] = choice.GetComponentInChildren<TextMeshProUGUI>();
            index++;
        }
    }

    private void ExitDialogueMode() //Exits the dialogue
    {
        dialogueIsPlaying = false;
        dialoguePanel.SetActive(false);
        dialogueText.text = "";
    }

    public static DialogueManager GetInstance() //Returns public instance of the dialogue manager
    {
        return instance;
    }
    private void Update()
    {
        if (!dialogueIsPlaying){ //Immediately return if no dialogue
            return;
        }

        //This line of code continues to the next line of dialogue
        if (Input.GetKeyDown("Dialogue"))
        {
            ContinueStory();
        }

    }

    

    public void EnterDialogueMode(TextAsset inkJSON)
    {
        currentStory = new Story(inkJSON.text);
        dialogueIsPlaying = true;
        dialoguePanel.SetActive(true);

        ContinueStory();
    }

    public void ContinueStory() //advances the text to the next line
    {
        if (currentStory.canContinue)
        {
            dialogueText.text = currentStory.Continue();
            DisplayChoices();

        } else
        {
            ExitDialogueMode();
        }
    }

    public bool dialogueCheck()
    { //This will interact with the enemy dialogue script and check to see if within range of dialogue. If so, it will return true to begin the dialogue state if the enemy has dialogue capabilities
        
        if (!dialogueEnemy)
        {
            return false; //By returning true, state manager will NOT enter dialogue state
        }

        if (dialogueEnemy.playerInRange) //If the enemy can be spoken to, initiate dialogue
        {
            EnterDialogueMode(dialogueEnemy.inkJSON);
            return true; //By returning true, state manager will enter dialogue state
        } else
        {
            ExitDialogueMode();
            return false; //By returning true, state manager will NOT enter dialogue state
        }
    }

    public void DisplayChoices() //Takes in choices, enables & initializes the current number of choice buttons, and hides remaining exisiting buttons
    {
        List<Choice> currentChoices = currentStory.currentChoices;

        if (currentChoices.Count > choices.Length) //Error out if too many choices
        {
            Debug.LogError("More choices were given than the UI can support. Number of choices given: " + currentChoices.Count);
        }

        int index = 0;
        foreach(Choice choice in currentChoices)
        {
            choices[index].gameObject.SetActive(true);
            choicesText[index].text = choice.text;
            index++;
        }

        for (int i = index; i< choices.Length; i++)
        {
            choices[i].gameObject.SetActive(false);
        }

        StartCoroutine(SelectFirstChoice());
    }
    private IEnumerator SelectFirstChoice() //Some code that is meant to make unitys event system play nice with the choices? Still trying to understand this tbh
    {
        // "Event System requires we clear it first then wait for at least one frame before we set the current selected object
        EventSystem.current.SetSelectedGameObject(null);
        yield return new WaitForEndOfFrame();
        EventSystem.current.SetSelectedGameObject(choices[0].gameObject);
    }

    public void MakeChoice(int choiceIndex)
    {
        currentStory.ChooseChoiceIndex(choiceIndex);
    }
}
