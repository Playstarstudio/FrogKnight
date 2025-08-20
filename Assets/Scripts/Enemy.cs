using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEngine;
using static GridManager;

public class Enemy : MonoBehaviour
{
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
        gridManager.NoTint();
        gridManager.AStar(ref path, ref aStarSearchedList, ref aStarToSearch, gridManager.GetCellPosition(transform.position), gridManager.GetCellPosition(target.position));
        gridManager.TintTiles(path.Select(a => a.position).ToList(), Color.red);

        if (path.Count > 2)
        {
        AStarNodeInfo square = path[path.Count - 2];
        transform.position = Vector2.MoveTowards(transform.position, gridManager.GetTileCenter(square.position), Time.deltaTime);
        }

    }
}
