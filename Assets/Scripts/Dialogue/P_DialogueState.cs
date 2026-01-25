using System.Collections.Generic;
using Ink.Parsed;
using UnityEngine;
using static GridManager;

public class P_DialogueState : P_State
{
    public bool visible = false;
    public override void EnterState(P_StateManager player)
    {
      inputFunction = Input.GetKeyDown;  
    }
    
    public override void UpdateState(P_StateManager player)
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            if (DialogueManager.GetInstance().dialogueIsPlaying)
            {
                return;
            }
            if (!DialogueManager.instance.dialogueCheck())
            {
                player.SwitchState(player.baseState);
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
    }

    public override void ExitState(P_StateManager player)
    {
        base.ExitState(player);
    }
}
