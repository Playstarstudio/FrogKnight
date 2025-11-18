using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class InventoryDescription : MonoBehaviour
{
    [SerializeField]
    private Image itemImage;
    [SerializeField]
    public TMP_Text itemName;
    [SerializeField]
    public TMP_Text description;
    public void ResetDescription()
    {
        this.itemImage.gameObject.SetActive(false);
        this.itemName.text = "";
        this.description.text = "";
    }
    public void SetDescription(Sprite sprite, string itemName, string itemDescription)
    {
        this.itemImage.gameObject.SetActive(true);
        this.itemImage.sprite = sprite;
        this.itemName.text = itemName;
        this.description.text = itemDescription;
    }
}
