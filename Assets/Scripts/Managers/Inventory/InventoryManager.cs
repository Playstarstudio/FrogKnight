using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public GameObject InventoryMenu;
    private bool isInventoryOpen = false;

    private void Start()
    {
        isInventoryOpen = false;
    }
    private void Update()
    {
        if (Input.GetButtonDown("Inventory") && isInventoryOpen)
        {
            InventoryMenu.SetActive(false);
            isInventoryOpen = false;
        }
        else if (Input.GetButtonDown("Inventory") && !isInventoryOpen)
        {
            InventoryMenu.SetActive(true);
            isInventoryOpen = true;
        }
    }
}
