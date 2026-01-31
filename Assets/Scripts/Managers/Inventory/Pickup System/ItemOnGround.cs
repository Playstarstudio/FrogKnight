using Inventory.Model;
using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ItemOnGround : MonoBehaviour
{
    [field: SerializeField]
    public ItemSO inventoryItem {  get; set; }

    [field: SerializeField]
    public int quantity {  get; set; }
    [SerializeField] AudioSource audioSource;

    [SerializeField] float duration = 0.3f;

    public GridManager gridManager;

    [SerializeField] public Vector2Int currentTile = new Vector2Int();

    private void Start()
    {
        gridManager = FindFirstObjectByType<GridManager>();
        GetComponent<SpriteRenderer>().sprite = inventoryItem.image;
        currentTile = gridManager.GetCellPosition(this.transform.position);
        PlaceItem(currentTile);
    }
    public void DestroyItem()
    {
        GetComponent<SpriteRenderer>().enabled = false;
        gridManager.MapRemoveItem(this, gridManager.GetCellPosition(transform.position));
        StartCoroutine(AnimateItemPickup());
    }

    public void PlaceItem(Vector2Int placeLoc)
    {
        GetComponent<SpriteRenderer>().enabled = true;
        gridManager.MapAddItem(this, placeLoc);
    }

    private IEnumerator AnimateItemPickup()
    {
        if (audioSource != null)
            audioSource.Play();
        Vector3 startScale = transform.localScale;
        Vector3 endScale = Vector3.zero;
        float currentTime = 0;
        while (currentTime < duration)
        {
            currentTime += Time.deltaTime;
            transform.localScale = Vector3.Lerp(startScale, endScale, currentTime / duration);
            yield return null;
        }
        transform.localScale = endScale;
            Destroy(gameObject);
    }
}
