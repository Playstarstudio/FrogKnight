using Ink.Parsed;
using System.Collections.Generic;
using UnityEngine;
using static ItemSO;

namespace Inventory.Model
{
    [CreateAssetMenu]
    public class EquippableItemSO : ItemSO, IDestroyableItem, IItemAction
    {
        [SerializeField] public bool equipped;
        public string ActionName => "Equip";
        public void PerformAction(P_StateManager player, int i)
        {
            player.gameLogManager.AddEntry(player, this);
            if (this.slot == PartLocation.Ring)
            {
                EquipmentSlot[] acceptedSlots = new EquipmentSlot[2];
                foreach (EquipmentSlot slot in player.inventoryManager.equipmentSlots)
                {
                    if (slot.acceptedPartLocation != this.slot)
                    {
                        {
                            continue;
                        }
                    }
                    else if (slot.acceptedPartLocation == this.slot)
                    {
                        if (slot.IsEmpty)
                        {
                            player.inventoryManager.HandleTryActionEquip(slot, i);
                            return;
                        }
                    }
                }
            }
            foreach (EquipmentSlot slot in player.inventoryManager.equipmentSlots)
            {
                if (slot.acceptedPartLocation != this.slot)
                {
                    {
                        continue;
                    }
                }
                else if (slot.acceptedPartLocation == this.slot)
                {
                    player.inventoryManager.HandleTryActionEquip(slot, i);
                    return;
                }
            }
        }
    }
}
