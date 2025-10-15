using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static GridManager;
public class Enemy : Entity
{
    // this is an example for how to use the grid system
    // it will path towards this "target"
    [SerializeField]
    Transform target;
    public float speed;
    public bool move;

    void Start()
    {
        gridManager = GridManager.Instance;
        path = new List<AStarNodeInfo>();
        aStarSearchedList = new Dictionary<AStarNodeInfo, AStarNodeInfo>();
        aStarToSearch = new SortedSet<AStarNodeInfo>();
        gameManager = FindFirstObjectByType<GameManager>();
        readyTime = att.GetBaseAttributeValue(att.GetAttributeType("Move Speed")); ; // enemies are ready to go at time = their speed
        gridManager.map[gridManager.GetCellPosition(this.transform.position)].occupied = true;
    }
    // Update is called once per frame
    void Update()
    {
        // runs astar and puts the path (which is backwards) into 'path'
        gridManager.AStar(ref path, ref aStarSearchedList, ref aStarToSearch, gridManager.GetCellPosition(transform.position), gridManager.GetCellPosition(target.position));

        // add each of our tiles on our path to the debug tile list
        foreach (var item in path.Select(a => a.position))
        {
            gridManager.AddDebugTile(item, Color.red);
        }

        // path towards the second to last tile on our path (path is stores backwards, so its the second tile)
        if (move)
            Move();
    }
    public void Move()
    {
        if (path.Count > 2)
        {
            readyTime += att.GetBaseAttributeValue(att.GetAttributeType("Move Speed"));
            AStarNodeInfo square = path[path.Count - 2];
            while (Vector2.Distance(transform.position, gridManager.GetTileCenter(square.position)) > 0.01f)
            {
                /*
                 {
                   gridManager.map[gridManager.GetCellPosition(this.transform.position)].occupied = false;
                      gridManager.map[gridManager.GetCellPosition(this.transform.position)].occupied = true;
                    }
                 */
                transform.position = Vector2.MoveTowards(transform.position, gridManager.GetTileCenter(square.position), Time.deltaTime);
            }
        }
    }
    public void OnDestroy()
    {
        gameManager.RemoveTimedEntity(this.gameObject);
    }
}
