using System;
using System.Collections.Generic;
using System.Text;
using Unity.VisualScripting;
using UnityEngine;
using static Inventory.Model.InventorySO;

public class StatPanelContainer : MonoBehaviour
{
    [SerializeField] public StatText[] statTexts;
    [SerializeField] private P_StateManager player;

    private void Start()
    {
        player = FindFirstObjectByType<P_StateManager>();
        statTexts = GetComponentsInChildren<StatText>();
        foreach (StatText stat in statTexts)
        {
            stat.statName.text = stat.attribute.name.ToString();
            if (stat.attribute.MaxAttribute != null)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append(player.att.GetCurrentAttributeValue(stat.attribute));
                sb.Append("/");
                sb.Append (player.att.GetCurrentAttributeValue(stat.attribute.MaxAttribute));
                stat.statValue.text = sb.ToString();
            }
            else
            {
                stat.statValue.text = player.att.GetCurrentAttributeValue(stat.attribute).ToString();
            }
        }
    }
}
