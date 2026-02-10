using UnityEngine;

namespace Inventory.Model
{
    [CreateAssetMenu]
    public class EquippableItemSO : ItemSO, IDestroyableItem, IItemAction
    {
        [SerializeField]
        public PartLocation Slot;
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

        public void PerformAction(P_StateManager player)
        {
            player.gameLogManager.AddEntry(player, this);
        }
    }
}