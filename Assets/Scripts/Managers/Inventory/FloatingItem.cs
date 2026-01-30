using Inventory.UI;
using UnityEngine;

public class FloatingItem : MonoBehaviour
{
    [SerializeField] private Canvas canvas;
    [SerializeField] private Camera mainCam;
    [SerializeField] private InventoryItem itemPrefab;

    public void Awake()
    {
        mainCam = Camera.main;
        itemPrefab = GetComponentInChildren<InventoryItem>();
    }

    public void SetData(Sprite sprite, int quantity)
    {
        itemPrefab.SetData(sprite, quantity);
    }

    private void Update()
    {
        Vector2 pos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.transform as RectTransform,
            Input.mousePosition,
            null,
            out pos);
        transform.position = canvas.transform.TransformPoint(pos);
    }

    public void Toggle(bool value)
    {
        gameObject.SetActive(value);
    }
}
