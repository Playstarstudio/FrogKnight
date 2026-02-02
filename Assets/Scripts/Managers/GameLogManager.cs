using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;

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
        if (logEntries.Count > maxEntries)
        {
            logEntries.RemoveAt(0);
        }
        logText.text = string.Join("", logEntries);
        Canvas.ForceUpdateCanvases();
        scrollRect.verticalNormalizedPosition = 0f;
    }
    public void AddEntry(Entity source, ItemSO item)
    {
        string text = PrepareDescription(source, item);
        logEntries.Add(text);
        if (logEntries.Count > maxEntries)
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
        }
        return sb.ToString();
    }
    private string PrepareDescription(Entity source, ItemSO item)
    {
        StringBuilder sb = new StringBuilder();
        sb.AppendLine();
        sb.Append(source);
        sb.Append(" drank the ");
        sb.Append(item.itemName);
        foreach (Modifier effect in item.effects)
        {
            sb.AppendLine();
            sb.Append(effect.attributeName);
            sb.Append(" to the ");
            sb.Append(source.name);
        }
        return sb.ToString();
    }
}