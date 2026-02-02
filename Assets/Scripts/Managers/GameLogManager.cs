using Inventory.Model;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameLogManager : MonoBehaviour
{

    public TextMeshProUGUI logText;
    public ScrollRect scrollRect;
    private List<string> logEntries = new List<string>();
    private int maxEntries = 100;
    StringBuilder sb = new StringBuilder();
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public void AddEntry(Entity source, Entity target, Ability ability)
    {
        string text = PrepareDescription(source, target, ability);
        logEntries.Add(text);
        if(logEntries.Count > maxEntries )
        {
            logEntries.RemoveAt(0);
        }
        logText.text = string.Join("", logEntries);
        Canvas.ForceUpdateCanvases();
        scrollRect.verticalNormalizedPosition = 0f;
    }
    private string PrepareDescription(Entity source, Entity target, Ability ability)
    {
        StringBuilder sb = new StringBuilder();
        //sb.Append(inventoryItem.item.description);
        sb.AppendLine();
        /* FOR IF I ADD MORE INFO PAST DESC
         */
        foreach (AbilityEffect effect in ability.abilityEffects)
        {
            sb.Append(effect.effectName);
            sb.Append(" applied to ");
            sb.Append(target.name);
            sb.Append(" by ");
            sb.Append(source.name);
            sb.AppendLine();
        }
        return sb.ToString();
    }
}
