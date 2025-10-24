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
    public float currentPerception;
    public PerceptionState perceptionState;

    public enum PerceptionState
        {
            NeverSeen,
            HasSeen,
            Searching,
            CurrentlySeeing
    }

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
    }
    public void Activate()
    {
        if (perceptionState == PerceptionState.CurrentlySeeing)
        {
            Move();
        }
        else
        {
            PerceptionCheck();
        }
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
        else
        {
            readyTime += att.GetBaseAttributeValue(att.GetAttributeType("Move Speed"));
        }
    }
    public void PerceptionCheck()
    {
        Debug.Log("Perception Check - setting to SEEING");
        perceptionState = PerceptionState.CurrentlySeeing;
    }
    public void OnDestroy()
    {
        gameManager.RemoveTimedEntity(this.gameObject);
    }
}
