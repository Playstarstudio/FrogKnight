using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    private P_StateManager playerStateManager;
    public UIInventoryPage InventoryMenu;
    private bool isInventoryOpen = false;
    public int inventorySize = 10;

    private void Start()
    {
        isInventoryOpen = false;
        InventoryMenu.InitializeInventoryUI(inventorySize);
        InventoryMenu.Hide();
    }

    public bool InventoryCheck()
    {
        if (playerStateManager == null)
        {
            playerStateManager = FindFirstObjectByType<P_StateManager>();
        }
        if (isInventoryOpen)
        {
            InventoryMenu.Hide();
            isInventoryOpen = false;
            return false;
        }
        else
        {
            InventoryMenu.Show();
            isInventoryOpen = true;
            return true;
        }
    }
}
