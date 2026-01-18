using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class P_DialogueState : P_State
{
    public bool visible = false;
    public override void EnterState(P_StateManager player)
    {
        inputFunction = Input.GetKeyDown;
    }
    
    public override void UpdateState(P_StateManager player)
    {
        if (Input.GetButtonDown("Dialogue"))
        {
            if (!player.dialogueManager.dialogueCheck())
            {
                player.SwitchState(player.baseState);
            }
        }

        /*System.Func<KeyCode, bool> inputFunction;
        inputFunction = Input.GetKeyDown;
        if (Input.GetKeyDown(KeyCode.Return))
        {
            DialogueManager.dialogueCheck();
        }*/
    }

    public override void ExitState(P_StateManager player)
    {
        base.ExitState(player);
    }
}
