using Ink.Runtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager instance;
    [Header("Dialogue UI")]
    public GameObject dialoguePanel;
    public GameObject continueIcon;
    public TextMeshProUGUI dialogueText;
    public TextMeshProUGUI displayNameText;
    public Animator portraitAnimator;
    public Animator layoutAnimator;

    [Header("Choices UI")]
    public GameObject[] choices;
    public TextMeshProUGUI[] choicesText;

    [Header("Audio")]
    private AudioSource audioSource;
    public DialogueAudioInfoSO defaultAudioInfo;
    [HideInInspector] public DialogueAudioInfoSO currentAudioInfo;
    public DialogueAudioInfoSO[] audioInfos;
    public Dictionary<string, DialogueAudioInfoSO> audioInfoDictionary;
    public bool makePredictable;

    [Header("Essentials")]
    public Story currentStory;
    public bool dialogueIsPlaying;
    public TextAsset loadGlobalsJSON;
    public float typingSpeed = 0.02f;   // Dialogue speed (smaller is faster)
    public Coroutine displayLineCoroutine;
    [HideInInspector] public int coroutineIndex = 1;
    public bool canContinueToNextLine = false;
    [HideInInspector] public string nextLine = "";

    [Header("Other Scripts")]
    public DialogueTrigger dialogueTrigger;
    public DialogueVariables dialogueVariables;
    public InkExternalFunctions inkExternalFunctions;

    [Header("Ink tags")]
    public const string SPEAKER_TAG = "speaker";
    public const string PORTRAIT_TAG = "portrait";
    public const string LAYOUT_TAG = "layout";
    public const string AUDIO_TAG = "audio";

    private void Awake()
    {
        if (instance != null) //Errors out if multiple Dialogue Managers
        {
            Debug.LogWarning("Found more than one Dialogue Manager in the scene");
        }
        instance = this;

        dialogueVariables = new DialogueVariables(loadGlobalsJSON);
        inkExternalFunctions = new InkExternalFunctions();

        audioSource  = this.gameObject.AddComponent<AudioSource>();
        currentAudioInfo = defaultAudioInfo;
    }

    private void Start()
    {
        //resets & disables dialogue panel and dialogueisplaying var
        dialogueIsPlaying = false;
        dialogueText.text = "";
        dialoguePanel.SetActive(false);

        layoutAnimator = dialoguePanel.GetComponent<Animator>(); //gets the layout animator

        //gets all the choice buttons into the appropriate list
        choicesText = new TextMeshProUGUI[choices.Length]; 
        int index = 0;
        foreach (GameObject choice in choices)
        {
            choicesText[index] = choice.GetComponentInChildren<TextMeshProUGUI>();
            index++;
        }

        InitializeAudioInfoDictionary();
    }

    public void ExitDialogueMode() //Exits the dialogue
    {
        dialogueVariables.StopListening(currentStory);  //Disables ink-UI variable talking
        inkExternalFunctions.Unbind(currentStory);  //Disables ink  external functions

        dialogueIsPlaying = false;
        dialogueText.text = "";
        dialoguePanel.SetActive(false);

        SetCurrentAudioInfo(defaultAudioInfo.id);
    }

    public static DialogueManager GetInstance() //Returns public instance of the dialogue manager
    {
        return instance;
    }

    public void EnterDialogueMode(TextAsset inkJSON)
    {
        currentStory = new Story(inkJSON.text); //Takes in the story
        dialogueIsPlaying = true;
        dialoguePanel.SetActive(true);

        dialogueVariables.StartListening(currentStory);  //Enables ink-UI variable talking
        inkExternalFunctions.Bind(currentStory, dialogueTrigger.gameObject);    //Enables ink external functions

        displayNameText.text = "???"; //resets dialogue tags to default + right
        portraitAnimator.Play("default");
        layoutAnimator.Play("right");

        ContinueStory();
    }

    public void ContinueStory() //advances the text to the next line if the text can continue, else ends dialogue
    {
        if (currentStory.canContinue)
        { /*
            if (displayLineCoroutine != null)
            {
                StopCoroutine(displayLineCoroutine);
            }*/
            
            nextLine = currentStory.Continue(); //Reads the next line of the story

            if (nextLine.Equals("") && !currentStory.canContinue) //Exits if last line is external function
            {
                ExitDialogueMode();
            }
            else
            {
                HandleTags(currentStory.currentTags);                                //Takes in JSON text file tags
                displayLineCoroutine = StartCoroutine(DisplayLine(nextLine));        //Advances to the next line of text
            }
        }
        else
        {
            ExitDialogueMode();
        }
    }

    private IEnumerator DisplayLine(string line)    //Handles the dialogue coming out one character at a time
    {
        dialogueText.text = line;    // sets dialogue to full line and then makes text invisible
        dialogueText.maxVisibleCharacters = 0;
        continueIcon.SetActive(false);  // deactivates the continue icon UI
        HideChoices();  // clears the choices UI
        
        canContinueToNextLine = false;

        //bool isAddingRichTextTag = false;

        for (coroutineIndex = 1; coroutineIndex < line.Length; coroutineIndex++) // adds dialogue text to the UI
        {
            PlayDialogueSound(dialogueText.maxVisibleCharacters, dialogueText.text[dialogueText.maxVisibleCharacters]);
            dialogueText.maxVisibleCharacters = coroutineIndex;
            yield return new WaitForSeconds(typingSpeed);
        }

        continueIcon.SetActive(true);   // Re-enables continue icon
        DisplayChoices();   // Displays dialogue choices

        canContinueToNextLine = true;
    }

    private void PlayDialogueSound(int currentDisplayedCharacterCount, char currentCharacter) //plays text audio in a random format
    {
        //sets the proper variables up from the NPC's audio manager
        AudioClip[] dialogueTypingSoundClips = currentAudioInfo.dialogueTypingSoundClips;
        int frequencyLevel = currentAudioInfo.frequencyLevel;
        float minPitch = currentAudioInfo.minPitch;
        float maxPitch = currentAudioInfo.maxPitch;
        bool stopAudioSource = currentAudioInfo.stopAudioSource;
        
        if (currentDisplayedCharacterCount % frequencyLevel == 0)
        {
            if (stopAudioSource)    //stops previous audio
            {
                audioSource.Stop();
            }

            AudioClip soundClip = null;
            
            if (makePredictable)    //match audio pitch to set letters
            {
                int hashCode = currentCharacter.GetHashCode();
                int predictableIndex = hashCode % dialogueTypingSoundClips.Length;  // sets letter sound
                soundClip = dialogueTypingSoundClips[predictableIndex];

                int minPitchInt = (int) (minPitch * 100);
                int maxPitchInt = (int) (maxPitch * 100);
                int pitchRangeInt = maxPitchInt - minPitchInt;  //gets pitch range

                if (pitchRangeInt != 0) //ensures range isn't 0 and sets the pitch
                {
                    int predictablePitchInt = (hashCode % pitchRangeInt) + minPitchInt;
                    float predictablePitch = predictablePitchInt / 100f;
                    audioSource.pitch = predictablePitch;
                }
                else
                {
                    audioSource.pitch = minPitch; //sets pitch to minimum if range is 0
                }
            } 
            else    //randomized audio per character
            {
                int randomIndex = Random.Range(0, dialogueTypingSoundClips.Length);
                soundClip = dialogueTypingSoundClips[randomIndex];
                audioSource.pitch = Random.Range(minPitch, maxPitch); //pitch
            }
            audioSource.PlayOneShot(soundClip); //plays sound
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
                    //Debug.Log("Speaker=" + tagValue);
                    break;
                case PORTRAIT_TAG:
                    portraitAnimator.Play(tagValue);
                    //Debug.Log("Portrait=" + tagValue);
                    break;
                case LAYOUT_TAG:
                    layoutAnimator.Play(tagValue);
                    //Debug.Log("Layout=" + tagValue);
                    break;
                case AUDIO_TAG:
                    SetCurrentAudioInfo(tagValue);
                    Debug.Log("Audio=" + tagValue);
                    break;
                default:
                    Debug.Log("Tag came in but is not currently being handled: " + tag);
                    break;
            }
        }
    }

    public bool dialogueCheck()
    { //This will interact with the enemy dialogue script and check to see if within range of dialogue. If so, it will return true to begin the dialogue state if the enemy has dialogue capabilities

        if (!dialogueTrigger)
        {
            return false; //By returning true, state manager will NOT enter dialogue state
        }

        if (dialogueTrigger.playerInRange) //If the enemy can be spoken to, initiate dialogue
        {
            EnterDialogueMode(dialogueTrigger.inkJSON);
            return true; //By returning true, state manager will enter dialogue state
        }
        else
        {
            ExitDialogueMode();
            return false; //By returning false, state manager will NOT enter dialogue state
        }
    }

    public void DisplayChoices() //Takes in choices, enables & initializes the current number of choice buttons, and hides remaining exisiting buttons
    {
        List<Choice> currentChoices = currentStory.currentChoices;

        EventSystem.current.SetSelectedGameObject(null);

        if (currentChoices.Count > choices.Length) //Error out if too many choices
        {
            Debug.LogError("More choices were given than the UI can support. Number of choices given: " + currentChoices.Count);
        }

        int index = 0;
        foreach (Choice choice in currentChoices)
        {
            choices[index].gameObject.SetActive(true);
            choicesText[index].text = choice.text;
            index++;
        }

        for (int i = index; i < choices.Length; i++)
        {
            choices[i].gameObject.SetActive(false);
        }

        if (currentChoices.Count > 0)
        {
            continueIcon.SetActive(false);
        }
        //StartCoroutine(SelectFirstChoice());
    }

    public void HideChoices()
    {
        foreach (GameObject choiceButton in choices)
        {
            choiceButton.SetActive(false);
        }
    }
    private IEnumerator SelectFirstChoice() //Some code that is meant to make unitys event system play nice with the choices? Still trying to understand this tbh
    {
        // "Event System requires we clear it first then wait for at least one frame before we set the current selected object"
        EventSystem.current.SetSelectedGameObject(null);
        yield return new WaitForEndOfFrame();
        EventSystem.current.SetSelectedGameObject(choices[0].gameObject);
    }

    public void MakeChoice(int choiceIndex)
    {
        currentStory.ChooseChoiceIndex(choiceIndex);
        ContinueStory();
    }

    public Ink.Runtime.Object GetVariableState(string variableName)
    {
        dialogueVariables.variables.TryGetValue(variableName, out Ink.Runtime.Object variableValue);
        if (variableValue == null)
        {
            Debug.LogWarning("Ink Variable was found to be null: " + variableName);
        }
        return variableValue;

        //Pair above code with following code found in another script
        // string variableName = ((Ink.Runtime.StringValue) DialogueManager.GetInstance().GetVariableState("variableName")).value
        // Can be done with booleans, floats, and ints as well, just change the code accordingly
    }

    private void InitializeAudioInfoDictionary()
    {
        audioInfoDictionary = new Dictionary<string, DialogueAudioInfoSO>();
        audioInfoDictionary.Add(defaultAudioInfo.id, defaultAudioInfo);
        foreach (DialogueAudioInfoSO audioInfo in audioInfos)
        {
            audioInfoDictionary .Add(audioInfo.id, audioInfo);
        }
    }

    private void SetCurrentAudioInfo(string id)
    {
        DialogueAudioInfoSO audioInfo = null;
        audioInfoDictionary.TryGetValue(id, out audioInfo);
        if (audioInfo != null)
        {
            currentAudioInfo = audioInfo;
        }
        else
        {
            Debug.LogWarning("Failed to find audio for id: " + id);
        }
    }
}