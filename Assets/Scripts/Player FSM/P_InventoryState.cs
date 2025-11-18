using UnityEngine;

public class P_InventoryState : P_State
{
    public override void EnterState(P_StateManager player)
    {

    }

    public override void UpdateState(P_StateManager player)
    {
        if (Input.GetButtonDown("Inventory"))
        {
            if (!player.inventoryManager.InventoryCheck())
            {
                player.SwitchState(player.baseState);
            }
        }
    }

    public override void ExitState(P_StateManager player)
    {
        base.ExitState(player);
    }
}
