using Inventory.Model;
using System;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static Inventory.Model.EquippableItemSO;
using static ItemSO;

public class EquipmentSlot : MonoBehaviour, IDropHandler, IPointerClickHandler, IBeginDragHandler, IEndDragHandler, IDragHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] public PartLocation acceptedPartLocation;
    [SerializeField] public EquippableItemSO equippableItem;
    [SerializeField] private Image selfImage;
    [SerializeField] private Image itemImage;
    [SerializeField] private Image borderImage;
    [SerializeField] private Image itemBG;
    [SerializeField] private Sprite slotDefault;
    [SerializeField]
    public TMP_Text itemName;
    [SerializeField]
    public TMP_Text description;
    [SerializeField]
    public HoverPanel hoverPanel;

    public ItemSO item { get; private set; }
    public bool IsEmpty => item == null;
    public PartLocation GetSlotType() => acceptedPartLocation;
    public bool empty = true;


    public event Action<EquipmentSlot> OnItemDroppedOnSlot;
    public event Action<EquipmentSlot> OnSlotClicked;
    public event Action<EquipmentSlot> OnSlotBeginDrag;
    public event Action<EquipmentSlot> OnSlotEndDrag;
    public event Action<EquipmentSlot> OnRightMouseBtnClick;
    public event Action<EquipmentSlot> OnPointerEnter;
    public event Action<EquipmentSlot> OnPointerExit;
    private void Start()
    {
        if(acceptedPartLocation == PartLocation.NA)
        {
            selfImage.enabled = false;
            itemImage.gameObject.SetActive(false);
            itemBG.gameObject.SetActive(false);
            borderImage.gameObject.SetActive(false);
        }
        this.itemImage.enabled = true;
        this.itemImage.sprite = slotDefault;
        
    }
    public bool TryEquip(ItemSO newItem, int qty)
    {
        // Validate that the item can go in this slot
        EquippableItemSO equippable = newItem as EquippableItemSO;
        if (equippable == null || equippable.slot != acceptedPartLocation)
            return false;

        this.item = newItem;
        this.itemImage.enabled = true;
        this.itemImage.sprite = newItem.image;
        return true;
    }
    public void SetData(ItemSO newItem)
    {
        if (newItem == null)
        {
            ResetSlot();
            return;
        }
        this.borderImage.enabled = false;
        this.item = newItem;
        this.itemImage.sprite = newItem.image;
        this.hoverPanel.PrepareHoverPanel(item);
        this.hoverPanel.Toggle(false);
        empty = false;
    }

    private void ResetSlot()
    {
        this.borderImage.enabled = false;
        this.item = null;
        this.itemImage.sprite = slotDefault;
        this.hoverPanel.itemName.text = string.Empty;
        this.hoverPanel.itemDescription.text = string.Empty;
        empty = true;
    }

    public void DeselectSlot()
    {
        this.borderImage.enabled = false;
    }
    public InventoryItem Unequip(ItemSO item)
    {
        InventoryItem unequippedItem = new InventoryItem
        {
            item = item,
            quantity = 1
        };
        ResetSlot();
        return unequippedItem;
    }

    public void OnDrop(PointerEventData eventData)
    {
        OnItemDroppedOnSlot?.Invoke(this);
    }

    public void Select()
    {
        this.borderImage.enabled = true;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            OnRightMouseBtnClick?.Invoke(this);
        }
        else
        {
            OnSlotClicked?.Invoke(this);
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (IsEmpty) return;
        OnSlotBeginDrag?.Invoke(this);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        OnSlotEndDrag?.Invoke(this);
    }

    public void OnDrag(PointerEventData eventData)
    {
    }
    void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
    {
        OnPointerEnter?.Invoke(this);
    }

    void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
    {
        OnPointerExit?.Invoke(this);
    }
}
