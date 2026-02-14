using Ink.Parsed;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static ItemSO;
using static Unity.VisualScripting.Member;

namespace Inventory.Model
{
    [CreateAssetMenu]
    public class ConsumableItemSO : ItemSO, IDestroyableItem, IItemAction
    {
        public string ActionName => "Consume";
        public void PerformAction(P_StateManager player, int i)
        {
            foreach (Modifier effect in this.effects)
            {
                AttributeModifier mod = new AttributeModifier()
                {
                    attribute = effect.attName,
                    operation = (AttributeModifier.Operator)effect.operation,
                    attModValue = effect.modifierValue
                };
                player.att.ApplyInstantModifier(mod);
            }
            player.gameLogManager.AddEntry(player, this);
        }
    }
    public interface IDestroyableItem
    {

    }

    
}