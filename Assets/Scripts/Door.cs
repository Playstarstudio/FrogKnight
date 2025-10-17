public class Door : Entity
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gridManager.map[gridManager.GetCellPosition(this.transform.position)].occupied = true;
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnDestroy()
    {
        gridManager.map[gridManager.GetCellPosition(this.transform.position)].occupied = false;
        gameManager.RemoveTimedEntity(this.gameObject);
    }
}
