using Inventory.Model;
using NUnit.Framework.Constraints;
using System;
using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventoryItem : MonoBehaviour, IPointerClickHandler, IBeginDragHandler, IEndDragHandler, IDropHandler, IDragHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField]
    public ItemSO item;
    [SerializeField]
    private Image itemImage;
    [SerializeField]
    private Image borderImage;
    [SerializeField]
    public TMP_Text itemName;
    [SerializeField]
    public TMP_Text description;
    [SerializeField]
    private TMP_Text quantityTxt;
    [SerializeField]
    public int quantity;
    [SerializeField]
    public HoverPanel hoverPanel;

    public event Action<InventoryItem> OnItemClicked, OnItemDroppedOn, OnItemBeginDrag, OnItemEndDrag, OnRightMouseBtnClick, OnPointerEnter, OnPointerExit;

    public bool empty = true;

    public void Awake()
    {
        ResetData();
        Deselect();
    }
    public void ResetData()
    {
        this.itemImage.gameObject.SetActive(false);
        this.item = null;
        this.quantity = 0;
        this.hoverPanel.itemName.text = string.Empty;
        this.hoverPanel.itemDescription.text = string.Empty;
        empty = true;
    }

    public void Deselect()
    {
        this.borderImage.enabled = false;
    }
    public void SetFloatingData(Sprite sprite, int quantity)
    {
        this.itemImage.gameObject.SetActive(true);
        this.itemImage.sprite = sprite;
        this.quantityTxt.text = quantity + "";
        empty = false;
    }
    public void SetData(ItemSO item, Sprite sprite, int quantity)
    {
        this.item = item;
        this.quantity = quantity;
        this.itemImage.gameObject.SetActive(true);
        this.itemImage.sprite = sprite;
        this.quantityTxt.text = quantity + "";
        this.hoverPanel.PrepareHoverPanel(item);
        this.hoverPanel.Toggle(false);
        Debug.Log(this.hoverPanel.itemDescription);
        empty = false;
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
            OnItemClicked?.Invoke(this);
        }

    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (empty) return;
        OnItemBeginDrag?.Invoke(this);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        OnItemEndDrag?.Invoke(this);
    }

    public void OnDrop(PointerEventData eventData)
    {
        OnItemDroppedOn?.Invoke(this);
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
