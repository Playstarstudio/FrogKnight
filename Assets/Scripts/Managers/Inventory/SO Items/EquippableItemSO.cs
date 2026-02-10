using UnityEngine;

namespace Inventory.Model
{
    [CreateAssetMenu]
    public class EquippableItemSO : ItemSO, IDestroyableItem, IItemAction
    {
        [SerializeField] public bool equipped;

        public string ActionName => "Equip";
        public void PerformAction(P_StateManager player)
        {
            player.gameLogManager.AddEntry(player, this);
        }
    }
}