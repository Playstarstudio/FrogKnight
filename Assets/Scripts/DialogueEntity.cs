using System.Collections.Generic;
using UnityEngine;
using static GridManager;

public class DialogueEntity : Entity
{
    public override void TryDestroy()
    {
        throw new System.NotImplementedException();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gridManager = GridManager.Instance;
        path = new List<AStarNodeInfo>();
        aStarSearchedList = new Dictionary<AStarNodeInfo, AStarNodeInfo>();
        aStarToSearch = new SortedSet<AStarNodeInfo>();
        gameManager = FindFirstObjectByType<GameManager>();
        readyTime = 2;
        //readyTime = this.att.GetCurrentAttributeValue(this.att.GetAttributeType("Move Speed")); ; // enemies are ready to go at time = their speed
        currentTile = gridManager.GetCellPosition(this.transform.position);
        this.transform.position = gridManager.GetTileCenter(gridManager.GetCellPosition(this.transform.position));
        gridManager.MapAddEntity(this, currentTile);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
