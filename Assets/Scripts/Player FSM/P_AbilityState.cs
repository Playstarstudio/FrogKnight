using UnityEngine;
using UnityEngine.UI;

public class P_AbilityState : P_State
{
    public bool visible = false;
    public override void EnterState(P_StateManager player)
    {
        if (player.casting == player.melee)
        {
            Debug.Log("casting melee");
            player.casting.TryCastAbility(player, player.targetingTile);
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
        }
        else
        {
            inputFunction = Input.GetKeyDown;
            player.castingSlot.GetComponent<Image>().color = player.castingSlot.activeColor;
        }

        // NEED TO FIX ****************
        //player.casting.GetComponent<RawImage>().color = player.casting.activeColor;

        //no movement
        //turn on range indicator
        //turn on mouse cursor
        //turn on ability ui
        //select ability

    }

    public override void UpdateState(P_StateManager player)
    {
        // on click, try cast ability
        // if successful, go to base state
        // if not, stay in ability state
        // give indicator of why can't cast
        /*
        List<Vector2Int> rangeTile= player.gridManager.GetBresenhamLine(player.gridManager.GetCellPosition(player.transform.position), player.gridManager.MouseToGrid());
        foreach (Vector2Int tile in rangeTile)
        {
            player.gridManager.TintTile(tile, Color.yellow);
        }
         */
        HandleTargeting(player);

        System.Func<KeyCode, bool> inputFunction;
        inputFunction = Input.GetKeyDown;
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (!visible)
            {
                DisplayAbilityRange(player);
                visible = true;
            }
            else
            {
                //ClearAllTintedTiles(player,);
                visible = false;
            }
        }
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
            if (player.casting.manaCost >= player.att.GetBaseAttributeValue(player.att.GetAttributeType("MP")))
            {
                Debug.Log("Not enough MP");
                player.SwitchState(player.baseState);
                return;
            }
            else
            {
                Debug.Log("first cast check passed");
                player.casting.TryCastAbility(player, targetCenter);
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

    private void HandleTargeting(P_StateManager player)
    {
        foreach (Vector2Int cell in player.casting.cells)
        {
            ClearOneTile(player, cell);
        }
        Vector2Int mouseCell = player.gridManager.MouseToGrid();
        player.casting.getCells(player, mouseCell);
        foreach (Vector2Int cell in player.casting.cells)
        {
            player.gridManager.TintTile(cell, new Color(1f, 0, 0, .5f), player.gridManager.rangeTiles);
        }
    }

    public override void ExitState(P_StateManager player)
    {
        foreach (Vector2Int cell in player.casting.cells)
        {
            ClearOneTile(player, cell);
        }
        if (player.casting != player.melee)
        {
            player.castingSlot.GetComponent<Image>().color = Color.white;
        }
        player.casting = null;
        player.castingSlot = null;
        //turn off range indicator
        //turn off mouse cursor
        //turn off ability ui
        //return to base state
    }

    public void DisplayAbilityRange(P_StateManager player)
    {

        int abilityRange = player.casting.range;
        foreach (var entry in player.gridManager.playerRange)
        {
            if (entry.Key.rawDist <= abilityRange)
            {
                player.gridManager.TintTile(entry.Key.position, Color.red);
            }
        }
    }

    public void ClearOneTile(P_StateManager player, Vector2Int cell)
    {
        player.gridManager.TintTile(cell, new Color(0, 0, 0, 0), player.gridManager.rangeTiles);
    }
    public void ClearAllTintedTiles(P_StateManager player, Vector2Int cell)
    {

    }
}
