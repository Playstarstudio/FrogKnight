using System.Text;
using TMPro;
using UnityEngine;

public class HoverPanel : MonoBehaviour
{
    public TMP_Text itemName;
    public TMP_Text itemDescription;

    public void PrepareHoverPanel(InventoryItem item)
    {
        PrepareHoverPanelName(item);
        PrepareHoverPanelDescription(item);
    }
    public string PrepareHoverPanelName(InventoryItem item)
    {
        return item.item.itemName;
    }
    public string PrepareHoverPanelDescription(InventoryItem item)
    {
        StringBuilder sb = new StringBuilder();
        //sb.Append(inventoryItem.item.description);
        sb.AppendLine();
        /* FOR IF I ADD MORE INFO PAST DESC
         */
        for (int i = 0; i < item.item.effects.Count; i++)
        {
            sb.Append(item.item.effects[i].operation);
            sb.Append("s ");
            sb.Append(item.item.effects[i].modifierValue);
            sb.Append(" ");
            sb.Append(item.item.effects[i].attName.name);
            sb.AppendLine();
        }
        return sb.ToString();
    }
}
