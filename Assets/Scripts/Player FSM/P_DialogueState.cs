using UnityEngine;

public class P_DialogueState : P_State
{
    public override void EnterState(P_StateManager player)
    {
       
    }
    
    public override void UpdateState(P_StateManager player)
    {
        if (!DialogueManager.GetInstance().dialogueIsPlaying) 
        {
            player.SwitchState(player.baseState);
        }

        if (DialogueManager.GetInstance().canContinueToNextLine) 
        {
            if ((Input.GetKeyDown(KeyCode.F) || Input.GetKeyDown(KeyCode.Mouse0)) && DialogueManager.instance.currentStory.currentChoices.Count == 0 )
            {
                DialogueManager.instance.ContinueStory();            
            }
            else if (Input.GetKeyDown(KeyCode.Alpha1) && DialogueManager.instance.currentStory.currentChoices.Count > 0 )
            {
                DialogueManager.instance.MakeChoice(0);
            } 
            else if (Input.GetKeyDown(KeyCode.Alpha2) && DialogueManager.instance.currentStory.currentChoices.Count > 0 )
            {
                DialogueManager.instance.MakeChoice(1);
            } 
            else if (Input.GetKeyDown(KeyCode.Alpha3) && DialogueManager.instance.currentStory.currentChoices.Count > 0 )
            {
                DialogueManager.instance.MakeChoice(2);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha4) && DialogueManager.instance.currentStory.currentChoices.Count > 0 )
            {
                DialogueManager.instance.MakeChoice(3);
            }
        }

        if (Input.GetKeyDown(KeyCode.Equals)) 
        {
            player.SwitchState(player.baseState);
            DialogueManager.instance.ExitDialogueMode();
        }
    }

    public override void ExitState(P_StateManager player)
    {
        
    }
}
