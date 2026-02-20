public class Door : Entity
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gridManager.MapAddEntity(this, gridManager.GetCellPosition(this.transform.position));
    }

    private void OnDestroy()
    {
        gridManager.MapRemoveEntity(this, gridManager.GetCellPosition(this.transform.position));

        gameManager.RemoveTimedEntity(this.gameObject);
    }

    public override void TryDestroy()
    {
        throw new System.NotImplementedException();
    }
}
