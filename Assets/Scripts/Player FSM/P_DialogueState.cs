using System.Collections.Generic;
using Ink.Parsed;
using UnityEngine;
using static GridManager;

public class P_DialogueState : P_State
{
    public bool visible = false;
    public override void EnterState(P_StateManager player)
    {
      //inputFunction = Input.GetKeyDown;  
    }
    
    public override void UpdateState(P_StateManager player)
    {
        if (!DialogueManager.GetInstance().dialogueIsPlaying)
        {
            player.SwitchState(player.baseState);
        }
        if (Input.GetKeyDown(KeyCode.F))
        {
            if (DialogueManager.GetInstance().dialogueIsPlaying)
            {
                return;
            }
            
        } 
        else if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            DialogueManager.instance.MakeChoice(0);
        } 
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            DialogueManager.instance.MakeChoice(1);
        } 
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            DialogueManager.instance.MakeChoice(2);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            DialogueManager.instance.MakeChoice(3);
        }
        else if (Input.GetKeyDown(KeyCode.Equals))
        {
            player.SwitchState(player.baseState);
        }
    }

    public override void ExitState(P_StateManager player)
    {
        DialogueManager.instance.ExitDialogueMode();
    }
}
