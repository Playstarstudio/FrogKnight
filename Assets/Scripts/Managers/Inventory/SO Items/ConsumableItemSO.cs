using UnityEngine;
using static Unity.VisualScripting.Member;

namespace Inventory.Model
{
    [CreateAssetMenu]
    public class ConsumableItemSO : ItemSO, IDestroyableItem, IItemAction
    {
        public string ActionName => "Consume";
        public AudioClip actionSFX => throw new System.NotImplementedException();
        public void PerformAction(P_StateManager player)
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
        }

    }
    public interface IDestroyableItem
    {

    }
    public interface IItemAction
    {
        public string ActionName { get; }
        public AudioClip actionSFX { get; }
        void PerformAction(P_StateManager player);
    }
    
}