using UnityEngine;

namespace Inventory.Model
{
    public class EquippableItemSO : ItemSO, IDestroyableItem, IItemAction
    {
        public string ActionName => "Equip";

        public AudioClip actionSFX {  get; private set; }

        public void PerformAction(P_StateManager player)
        {
            throw new System.NotImplementedException();
        }
    }
}