using UnityEngine;

namespace Inventory.Model
{
    public class EquippableItemSO : ItemSO, IDestroyableItem, IItemAction
    {
        public enum PartLocation
        {
            PrimaryHand, //0
            OffHand, //1
            TwoHand, //2
            Torso, //3
            Head, //4
            Glove, //5
            Pants, //6
            Foot //7
        }

        public string ActionName => "Equip";

        public AudioClip actionSFX {  get; private set; }

        public void PerformAction(P_StateManager player)
        {
            throw new System.NotImplementedException();
        }
    }
}