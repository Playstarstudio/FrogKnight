using UnityEngine;
using UnityEngine.UI;

public class P_AbilityState : P_State
{
    public override void EnterState(P_StateManager player)
    {
        inputFunction = Input.GetKeyDown;
        player.casting.gameObject.GetComponent<RawImage>().color = player.casting.activeColor;
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
            if (player.casting == null)
            {
                Debug.Log("No ability selected");
                player.SwitchState(player.baseState);
                return;
            }
            if (player.casting.ability.manaCost >= player.p_Att.GetBaseAttributeValue(player.p_Att.GetAttributeType("MP")))
            {
                Debug.Log("Not enough MP");
                player.SwitchState(player.baseState);
                return;
            }
            else
            {
                Debug.Log("first cast check passed");
                player.casting.ability.TryCastAbility(player, targetCenter);
            }
            //get all entities on that tile
            //attempt to cast ability on that entity
            //if successful
            if (player.castSuccess)
            {
                Debug.Log("Ability cast successful");
                player.SwitchState(player.baseState);
            }
            else
            {
                Debug.Log("Ability cast failed");
                player.SwitchState(player.baseState);
            }
            //if not successful, stay in ability state
        }
        else if (inputFunction(KeyCode.Mouse1))
        {
            Debug.Log("Ability cast cancelled");
            player.SwitchState(player.baseState);
        }
    }

    public override void ExitState(P_StateManager player)
    {
        player.casting.gameObject.GetComponent<RawImage>().color = Color.white;
        //turn off range indicator
        //turn off mouse cursor
        //turn off ability ui
        //return to base state
    }
}
