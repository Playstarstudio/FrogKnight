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
    private void Awake()
    {
    }
    void Start()
    {
        gridManager = GridManager.Instance;
        path = new List<AStarNodeInfo>();
        aStarSearchedList = new Dictionary<AStarNodeInfo, AStarNodeInfo>();
        aStarToSearch = new SortedSet<AStarNodeInfo>();
        gameManager = FindFirstObjectByType<GameManager>();
        this.transform.position = gridManager.GetTileCenter(gridManager.GetCellPosition(this.transform.position));
        gridManager.MapAddEntity(this, gridManager.GetCellPosition(this.transform.position));
        readyTime = att.GetBaseAttributeValue(att.GetAttributeType("Move Speed")); ; // enemies are ready to go at time = their speed
    }
    // Update is called once per frame
    void Update()
    {

        // add each of our tiles on our path to the debug tile list
        foreach (var item in path.Select(a => a.position))
        {
            gridManager.AddDebugTile(item, Color.red);
        }
    }
    public void Activate(float _time, float _globalTimer)
    {
        PerceptionCheck(_time, _globalTimer);
        if (perceptionState == PerceptionState.CurrentlySeeing)
        {
            Move();
        }
        else if (perceptionState == PerceptionState.HasSeen)
        {
            readyTime += att.GetBaseAttributeValue(att.GetAttributeType("Move Speed"));
        }
        else
        {
            readyTime += att.GetBaseAttributeValue(att.GetAttributeType("Move Speed"));
        }
    }
    public void Move()
    {
        Vector2Int fromPos = gridManager.GetCellPosition(this.transform.position);
        // runs astar and puts the path (which is backwards) into 'path'
        gridManager.AStar(ref path, ref aStarSearchedList, ref aStarToSearch, gridManager.GetCellPosition(transform.position), gridManager.GetCellPosition(target.position));
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
                currentPos = gridManager.GetCellPosition(transform.position);
            }
            gridManager.MapMoveEntity(this, fromPos, currentPos);
        }
        else if (path.Count <= 2)
        {
            readyTime += att.GetBaseAttributeValue(att.GetAttributeType("Move Speed"));
        }
    }
    public void PerceptionCheck(float _time, float _globalTimer)
    {
        TileInfo targetTile;
        TileInfo myTile;
        int manhattanDistance = gridManager.ManhattanDistanceToTile(gridManager.GetCellPosition(this.transform.position), gridManager.GetCellPosition(target.position));
        float vision = att.GetBaseAttributeValue(att.GetAttributeType("Vision Range"));
        gridManager.map.TryGetValue(gridManager.GetCellPosition(target.position), out targetTile);
        gridManager.map.TryGetValue(gridManager.GetCellPosition(this.transform.position), out myTile);
        if (manhattanDistance > att.GetBaseAttributeValue(att.GetAttributeType("Vision Range")))
        {
            Debug.Log("Perception Check - out of range");
            currentPerception -= (_time*(manhattanDistance/vision));
            currentPerception = Mathf.Clamp(currentPerception, 0, 5);
        }
        else if (manhattanDistance <= att.GetBaseAttributeValue(att.GetAttributeType("Vision Range")))
        {
            if (myTile.LoS)
            {
                currentPerception += (_time*(vision/manhattanDistance));
                currentPerception = Mathf.Clamp(currentPerception, 0, 5);
                Debug.Log("Perception Check - in sight");
            }
            else
            {
                currentPerception -= (_time * (manhattanDistance/vision));
                currentPerception = Mathf.Clamp(currentPerception, 0, 5);
                Debug.Log("Perception Check - in range but not in sight");
            }

        }
        if (currentPerception >= 5)
        {
            perceptionState = PerceptionState.CurrentlySeeing;
            Debug.Log("Perception State: CURRENTLY SEEING");
        }
        if (currentPerception < 2)
        {
                perceptionState = PerceptionState.HasSeen;
                Debug.Log("Perception State: HAS SEEN");

        }
        gridManager.CalculateTileData(currentPos);
    }
    public void OnDestroy()
    {
        gridManager.MapRemoveEntity(this,currentTile);
        gameManager.RemoveTimedEntity(this.gameObject);
    }
}
