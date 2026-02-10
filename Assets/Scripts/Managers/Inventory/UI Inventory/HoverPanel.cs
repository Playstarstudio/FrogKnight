using System.Text;
using TMPro;
using UnityEngine;

public class HoverPanel : MonoBehaviour
{
    public TMP_Text itemName;
    public TMP_Text itemDescription;
    public GameObject panel;

    public void PrepareHoverPanel(ItemSO item)
    {
        itemName.text = PrepareHoverPanelName(item);
        itemDescription.text = PrepareHoverPanelDescription(item);
    }
    public string PrepareHoverPanelName(ItemSO item)
    {
        return item.itemName;
    }
    public string PrepareHoverPanelDescription(ItemSO item)
    {
        StringBuilder sb = new StringBuilder();
        //sb.Append(inventoryItem.item.description);
        sb.AppendLine();
        /* FOR IF I ADD MORE INFO PAST DESC
         */
        for (int i = 0; i < item.effects.Count; i++)
        {
            sb.Append(item.effects[i].operation);
            sb.Append("s ");
            sb.Append(item.effects[i].modifierValue);
            sb.Append(" ");
            sb.Append(item.effects[i].attName.name);
            sb.AppendLine();
        }
        return sb.ToString();
    }
    public void Toggle(bool val)
    {
        gameObject.SetActive(val);
    }
}
