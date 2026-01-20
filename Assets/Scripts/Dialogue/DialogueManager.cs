using System.Collections;
using System.Collections.Generic;
using Ink.Runtime;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SearchService;

public class DialogueManager : MonoBehaviour
{
    [Header("Dialogue UI")]
    public GameObject dialoguePanel;
    public TextMeshProUGUI dialogueText;
    public TextMeshProUGUI displayNameText;
    public Animator portraitAnimator;
    public Animator layoutAnimator;

    [Header("Choices UI")]
    public GameObject[] choices;
    public TextMeshProUGUI[] choicesText;

    public static DialogueManager instance;
    public Story currentStory;
    public bool dialogueIsPlaying;

    public const string SPEAKER_TAG = "speaker";
    public const string PORTRAIT_TAG = "portrait";
    public const string LAYOUT_TAG = "layout";

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

        layoutAnimator = dialoguePanel.GetComponent<Animator>(); //gets the layout animator

        choicesText = new TextMeshProUGUI[choices.Length]; //gets all the choices into the appropriate list
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

        displayNameText.text = "???"; //resets dialogue tags to default
        portraitAnimator.Play("default");
        layoutAnimator.Play("right");

        ContinueStory();
    }

    public void ContinueStory() //advances the text to the next line if the text can continue, else ends dialogue
    {
        if (currentStory.canContinue)
        {
            dialogueText.text = currentStory.Continue(); //Advances to the next line of text
            DisplayChoices();
            HandleTags(currentStory.currentTags); //Takes in JSON text file tags

        } else
        {
            ExitDialogueMode();
        }
    }

    public void HandleTags(List<string> currentTags)
    {
        foreach (string tag in currentTags)
        {
            string[] splitTag = tag.Split(':'); //Read and parse through the tags
            if (splitTag.Length != 2)
            {
                Debug.LogError("Tag could not be appropriately parsed: " + tag); //Errors out if tag can't be read properly
            }
            string tagKey = splitTag[0].Trim();
            string tagValue = splitTag[1].Trim();

            switch (tagKey) //takes tag and implements it into the Dialogue IN PROGRESS
            {
                case SPEAKER_TAG:
                    displayNameText.text = tagValue;
                    Debug.Log("Speaker=" + tagValue);
                    break;
                case PORTRAIT_TAG:
                    portraitAnimator.Play(tagValue);
                    Debug.Log("Portrait=" + tagValue);
                    break;
                case LAYOUT_TAG:
                    layoutAnimator.Play(tagValue);
                    Debug.Log("Layout=" + tagValue);
                    break;
                default:
                    break;
            }
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
