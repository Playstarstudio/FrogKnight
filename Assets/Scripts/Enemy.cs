using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEngine;
using static GridManager;

public class Enemy : MonoBehaviour
{
    // this is an example for how to use the grid system
    // it will path towards this "target"
    [SerializeField]
    Transform target;

    // keep a ref to the gridmanager
    private GridManager gridManager;

    // data structures for pathfinding
    private List<AStarNodeInfo> path;
    private Dictionary<AStarNodeInfo, AStarNodeInfo> aStarSearchedList;
    private SortedSet<AStarNodeInfo> aStarToSearch;

    void Start()
    {
        gridManager = GridManager.Instance;
        path = new List<AStarNodeInfo>();
        aStarSearchedList = new Dictionary<AStarNodeInfo, AStarNodeInfo>();
        aStarToSearch = new SortedSet<AStarNodeInfo>();
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
        if (path.Count > 2)
        {
            AStarNodeInfo square = path[path.Count - 2];
            transform.position = Vector2.MoveTowards(transform.position, gridManager.GetTileCenter(square.position), Time.deltaTime);
        }

    }
}
