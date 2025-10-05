using System;
using UnityEngine;

public class P_AbilityState : P_State
{
    public override void EnterState(P_StateManager player)
    {
        inputFunction = Input.GetKeyDown;
        //no movement
        //turn on range indicator
        //turn on mouse cursor
        //turn on ability ui
        //select ability

    }

    public override void UpdateState(P_StateManager player)
    {
        //on click, try cast ability
        // if successful, go to base state
        // if not, stay in ability state
        // give indicator of why can't cast
        System.Func<KeyCode, bool> inputFunction;
        inputFunction = Input.GetKeyDown;
        if (inputFunction(KeyCode.Mouse0))
        {

            //Vector2 target = new Vector2 (Input.mousePosition.x, Input.mousePosition.y);
            Vector2Int targetCenter = player.gridManager.MouseToGrid();
            //target = player.gridManager.GetTileCenter(targetCenter);
            if(player.casting == null)
            {
                Debug.Log("No ability selected");
                player.currentState = player.baseState;
                player.currentState.EnterState(player);
                return;
            }
            if(player.casting.manaCost >= player.p_Att.GetBaseAttributeValue(player.p_Att.GetAttributeType("MP")))
            {
                Debug.Log("Not enough MP");
                player.currentState = player.baseState;
                player.currentState.EnterState(player);
                return;
            }
            else
            {
                Debug.Log("first cast check passed");
                player.casting.CastAbility(player, targetCenter);
            }
            //get all entities on that tile
            //attempt to cast ability on that entity
            //if successful
            if (player.castSuccess)
            {
            player.currentState = player.baseState;
            player.currentState.EnterState(player);
            }
            else
            {
                Debug.Log("Ability cast failed");
                player.currentState = player.baseState;
                player.currentState.EnterState(player);
            }
            //if not successful, stay in ability state
        }
    }

    public override void ExitState(P_StateManager player)
    {
        base.ExitState(player);
        //turn off range indicator
        //turn off mouse cursor
        //turn off ability ui
        //return to base state
    }
}
