using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GridManager : MonoBehaviour
{
    private static GridManager _instance;
    public static GridManager Instance { get { return _instance; } }
    private class TileInfo
    {
        public bool traversable;

        public TileInfo(bool traversable)
        {
            this.traversable = traversable;
        }
    }

    // Stores our traversable tiles
    [SerializeField]
    private Tilemap traversable;

    // Stores our non-traversable tiles
    [SerializeField]
    private Tilemap notTraversable;

    // Stores our door-friendly tiles
    [SerializeField]
    private Tilemap doorOptions;

    // Stores our tiles and whether or not they are traversable
    private Dictionary<Vector2Int, TileInfo> map;

    // Stores tiles that we want to highlight when debugging
    private Dictionary<Vector2Int, Color> debugTiles;

    // If true we highlight debug tiles
    [SerializeField] private bool debug;

    //Player Related Data 
    public GameObject player;
    public Dictionary<DijkstrasNodeInfo, DijkstrasNodeInfo> playerRange;
    private PlayerMovement playerMovement;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            traversable.CompressBounds();
            notTraversable.CompressBounds();
            doorOptions.CompressBounds();
            Instance.MergeIntoTraversable(ref traversable);
            Instance.MergeIntoNonTraversable(ref notTraversable);
            Instance.MergeIntoDoor(ref doorOptions);
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
            traversable.CompressBounds();
            notTraversable.CompressBounds();
            doorOptions.CompressBounds();
            map = new Dictionary<Vector2Int, TileInfo>();
            debugTiles = new Dictionary<Vector2Int, Color>();
            CreateGrid();
        }
        player = GameObject.FindWithTag("PLayer");
        playerMovement = player.GetComponent<PlayerMovement>();
    }

    public Vector2 GetTileCenter(Vector2Int gridPos)
    {
        TileInfo tile;
        bool exists = map.TryGetValue(gridPos, out tile);
        Vector3Int posn = new Vector3Int(gridPos.x, gridPos.y, 0);

        if (!exists)
        {
            throw new ArgumentException("tile does not exist on grid");
        }

        if (tile.traversable)
        {
            return (Vector2)traversable.GetCellCenterWorld(new Vector3Int(gridPos.x, gridPos.y, 0));
        }
        else
        {
            return (Vector2)notTraversable.GetCellCenterWorld(new Vector3Int(gridPos.x, gridPos.y, 0));
        }
    }

    public void AddDebugTile(Vector2Int tilePos, Color tint)
    {
        if (debug)
        {
            debugTiles.Add(tilePos, tint);
        }
    }

    public Vector2Int GetCellPosition(Vector3 worldPos)
    {
        Vector3Int pos3 = traversable.WorldToCell(worldPos);
        Vector2Int pos = new(pos3.x, pos3.y);
        return pos;
    }

    public Vector3 GetWorldPosition(Vector2Int cellPosition)
    {
        return traversable.GetCellCenterWorld(new Vector3Int(cellPosition.x, cellPosition.y));
    }

    public Vector2Int MouseToGrid()
    {
        return GetCellPosition(Camera.main.ScreenToWorldPoint(Input.mousePosition));
    }

    // utility class for running dijkstras
    public class DijkstrasNodeInfo : IComparable<DijkstrasNodeInfo>
    {
        // which position on the map does this correspond to
        public Vector2Int position;

        // parent node
        public DijkstrasNodeInfo parent;

        // distance from origin in moves
        public int distance;

        public int CompareTo(DijkstrasNodeInfo other)
        {
            int dist = distance - other.distance;
            if (dist == 0)
            {
                dist = position.x - other.position.x;
                if (dist == 0)
                {
                    return position.y - other.position.y;
                }
                else
                {
                    return dist;
                }
            }
            else
            {
                return dist;
            }
        }

        public override bool Equals(object? obj)
        {
            if (obj == null) return false;

            if (!(obj is DijkstrasNodeInfo))
                return false;

            DijkstrasNodeInfo info = (DijkstrasNodeInfo)obj;
            // compare elements here
            return info.position == this.position;
        }

        public override int GetHashCode()
        {
            return (int)position.GetHashCode();
        }

        public List<DijkstrasNodeInfo> NeighborsToNodeInfos(List<Vector2Int> neighbors, DijkstrasNodeInfo parent)
        {
            List<DijkstrasNodeInfo> nodeInfos = new List<DijkstrasNodeInfo>();

            foreach (Vector2Int neighbor in neighbors)
            {
                DijkstrasNodeInfo current = new DijkstrasNodeInfo();
                current.position = neighbor;
                current.distance = distance + 1;
                current.parent = parent;
                nodeInfos.Add(current);
            }

            return nodeInfos;
        }

    }

    // utility class for running astar
    public class AStarNodeInfo : IComparable<AStarNodeInfo>
    {
        // which position on the map does this correspond to
        public Vector2Int position;

        // parent node
        public AStarNodeInfo parent;

        // distance from origin in moves
        public int distance;

        // estimated distance to destination
        public int hueristic;

        public int CompareTo(AStarNodeInfo other)
        {
            int dist = (distance + hueristic) - (other.distance + other.hueristic);
            if (dist == 0)
            {
                dist = position.x - other.position.x;
                if (dist == 0)
                {
                    return position.y - other.position.y;
                }
                else
                {
                    return dist;
                }
            }
            else
            {
                return dist;
            }
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;

            if (!(obj is AStarNodeInfo))
                return false;

            AStarNodeInfo info = (AStarNodeInfo)obj;
            // compare elements here
            return info.position == this.position;
        }

        public override int GetHashCode()
        {
            return (int)position.GetHashCode();
        }

        public List<AStarNodeInfo> NeighborsToNodeInfos(List<Vector2Int> neighbors, AStarNodeInfo parent, Vector2Int target)
        {
            List<AStarNodeInfo> nodeInfos = new List<AStarNodeInfo>();

            foreach (Vector2Int neighbor in neighbors)
            {
                AStarNodeInfo current = new AStarNodeInfo();
                current.position = neighbor;
                current.distance = distance + 1;
                current.parent = parent;
                current.hueristic = (int)Vector2Int.Distance(current.position, target);
                nodeInfos.Add(current);
            }

            return nodeInfos;
        }
    }

    /* @brief Runs AStar
     * 
     * @param outputPath list to output our path to, stored "backwards", our starting pos at end of list and target in pos 0
     * @param searched dictionary for storing which nodes we have searched 
     * @param toSearch sorted set for storing our list of nodes to search in ascending order of distance + heuristic
     * @param startingSquare the square to start our search from
     * @param target our target square to find
     */
    public void AStar(ref List<AStarNodeInfo> outputPath, ref Dictionary<AStarNodeInfo, AStarNodeInfo> searched,
        ref SortedSet<AStarNodeInfo> toSearch, Vector2Int startingSquare, Vector2Int target)
    {
        toSearch.Clear();
        searched.Clear();
        outputPath.Clear();
        AStarNodeInfo start = new AStarNodeInfo();
        start.position = startingSquare;
        start.distance = 0;
        start.parent = null;
        start.hueristic = (int)Vector2Int.Distance(startingSquare, target);
        toSearch.Add(start);

        while (toSearch.Count > 0)
        {
            AStarNodeInfo current = toSearch.Min;

            if (current.position == target)
            {
                while (current != null)
                {
                    outputPath.Add(current);
                    current = current.parent;
                }
                return;
            }
            toSearch.Remove(current);
            searched.Add(current, current.parent);

            List<AStarNodeInfo> neighbors = current.NeighborsToNodeInfos(GetNeighbors(current.position), current, target);

            foreach (AStarNodeInfo neighbor in neighbors)
            {

                // if the node isnt on the map, ignore it
                if (!map.ContainsKey(neighbor.position))
                {
                    continue;
                }

                // if it isnt traversible ignore this node
                if (!map[neighbor.position].traversable)
                {
                    continue;
                }

                // if already in searched list, dont add
                if (searched.ContainsKey(neighbor))
                {
                    continue;
                }

                bool inSearch = toSearch.Any<AStarNodeInfo>(val => val.position == neighbor.position);

                if (!inSearch)
                {
                    toSearch.Add(neighbor);
                }

            }

        }

    }


    /*
     * @brief Runs Dijkstras 
     * 
     * @param searched dictionary to store searched nodes in
     * @param toSearch sorted set of nodes in ascending order of distance to start
     * @param startingSquare square to start search from
     * @param range range in squares around startingSquare to search, searches whole grid if -1
     */
    public void Dijkstras(ref Dictionary<DijkstrasNodeInfo, DijkstrasNodeInfo> searched, ref SortedSet<DijkstrasNodeInfo> toSearch, Vector2Int startingSquare, int range)
    {
        toSearch.Clear();
        searched.Clear();
        DijkstrasNodeInfo start = new DijkstrasNodeInfo();
        start.position = startingSquare;
        start.distance = 0;
        start.parent = null;
        toSearch.Add(start);

        while (toSearch.Count > 0)
        {
            DijkstrasNodeInfo current = toSearch.Min;
            int currentDist = current.distance;
            toSearch.Remove(current);
            searched.Add(current, current.parent);

            List<DijkstrasNodeInfo> neighbors = current.NeighborsToNodeInfos(GetNeighbors(current.position), current);

            foreach (DijkstrasNodeInfo neighbor in neighbors)
            {

                // if the node isnt on the map, ignore it
                if (!map.ContainsKey(neighbor.position))
                {
                    continue;
                }

                // if were out of our range, ignore these nodes
                int distance = currentDist + 1;
                if (distance > range && range != -1)
                {
                    continue;
                }

                // if it isnt traversible ignore this node
                if (!map[neighbor.position].traversable)
                {
                    continue;
                }

                // if already in searched list, dont add
                if (searched.ContainsKey(neighbor))
                {
                    continue;
                }

                bool inSearch = toSearch.Contains(neighbor);

                if (!inSearch)
                {
                    toSearch.Add(neighbor);
                }

            }

        }

    }

    public bool OnTraversableTile(Vector3 worldPosition)
    {
        Vector3Int tilePosn = traversable.WorldToCell(worldPosition);
        TileBase tile = traversable.GetTile(tilePosn);

        return tile != null;
    }

    private void Update()
    {
        if (debug)
        {
            TintDebug();
            debugTiles.Clear();
        }
    }
    private void CreateGrid()
    {
        for (int x = traversable.cellBounds.xMin; x < traversable.cellBounds.xMax; x++)
        {
            for (int y = traversable.cellBounds.yMin; y < traversable.cellBounds.yMax; y++)
            {
                Vector3 worldPosition = traversable.CellToWorld(new Vector3Int(x, y, 0));
                if (notTraversable.HasTile(notTraversable.WorldToCell(worldPosition)))
                {
                    map.Add(new Vector2Int(x, y), new TileInfo(false));
                }
                else
                {
                    map.Add(new Vector2Int(x, y), new TileInfo(true));
                }
            }
        }

    }

    public bool TraversableCheck(Vector2Int pos)
    {
        TileInfo tile;
        bool exists = map.TryGetValue(pos, out tile);
        Vector3Int posn = new Vector3Int(pos.x, pos.y, 0);
        if (!exists)
        {
            return false;
        }
        if (tile.traversable)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    private void TintTile(Vector2Int gridPos, Color color)
    {
        TileInfo tile;
        bool exists = map.TryGetValue(gridPos, out tile);
        Vector3Int posn = new Vector3Int(gridPos.x, gridPos.y, 0);

        if (!exists)
        {
            throw new ArgumentException("tile does not exist on grid");
        }

        if (tile.traversable)
        {
            traversable.SetTileFlags(posn, TileFlags.None);
            traversable.SetColor(posn, color);
        }
        else
        {
            notTraversable.SetTileFlags(posn, TileFlags.None);
            notTraversable.SetColor(posn, color);
        }

    }

    private void TintDebug()
    {
        foreach (var tile in map)
        {
            if (debugTiles.ContainsKey(tile.Key))
            {
                TintTile(tile.Key, debugTiles[tile.Key]);
            }
            else
            {
                TintTile(tile.Key, Color.white);
            }
        }
    }
    private List<Vector2Int> GetNeighbors(Vector2Int position)
    {
        List<Vector2Int> neighbors = new List<Vector2Int>();

        if (map.ContainsKey(new Vector2Int(position.x - 1, position.y)))
        {
            neighbors.Add(new Vector2Int(position.x - 1, position.y));
        }

        if (map.ContainsKey(new Vector2Int(position.x + 1, position.y)))
        {
            neighbors.Add(new Vector2Int(position.x + 1, position.y));
        }

        if (map.ContainsKey(new Vector2Int(position.x, position.y - 1)))
        {
            neighbors.Add(new Vector2Int(position.x, position.y - 1));
        }

        if (map.ContainsKey(new Vector2Int(position.x, position.y + 1)))
        {
            neighbors.Add(new Vector2Int(position.x, position.y + 1));
        }

        return neighbors;

    }
    private void MergeTilemaps(ref Tilemap destTilemap, ref Tilemap sourceTilemap, bool traversable)
    {
        int x;
        int y;
        int z;
        for (x = sourceTilemap.cellBounds.min.x; x < sourceTilemap.cellBounds.max.x; x++)
        {
            for (y = sourceTilemap.cellBounds.min.y; y < sourceTilemap.cellBounds.max.y; y++)
            {
                for (z = sourceTilemap.cellBounds.min.z; z < sourceTilemap.cellBounds.max.z; z++)
                {
                    TileBase t = sourceTilemap.GetTile(new Vector3Int(x, y, z));
                    if (t != null)
                    {
                        destTilemap.SetTile(new Vector3Int(x, y, z), t);
                        if (!(map.ContainsKey(new Vector2Int(x, y))))
                        {
                            map.Add(new Vector2Int(x, y), new TileInfo(traversable));
                        }
                        else
                        {
                            map[new Vector2Int(x, y)] = new TileInfo(traversable);
                        }

                    }
                }
            }

        }
    }
    private void MergeIntoTraversable(ref Tilemap sourceTilemap)
    {
        MergeTilemaps(ref traversable, ref sourceTilemap, true);
    }
    private void MergeIntoNonTraversable(ref Tilemap sourceTilemap)
    {
        MergeTilemaps(ref notTraversable, ref sourceTilemap, false);
    }
    private void MergeIntoDoor(ref Tilemap sourceTilemap)
    {
        MergeTilemaps(ref doorOptions, ref sourceTilemap, false);
    }



    /*
    * @brief Converts the current grid map to a sorted set for Dijkstra
    * 
       private Dictionary<Vector2Int, TileInfo> map;
    */
    public SortedSet<DijkstrasNodeInfo> MapToSortedSet()
    {
        SortedSet<DijkstrasNodeInfo> sortedSet = new SortedSet<DijkstrasNodeInfo>();
        DijkstrasNodeInfo currentNode;

        foreach (KeyValuePair<Vector2Int, TileInfo> tile in map)
        {
            currentNode = new DijkstrasNodeInfo();
            currentNode.position = tile.Key;
            currentNode.parent = null;
            currentNode.distance = 1;
            sortedSet.Add(currentNode);
        }
        return sortedSet;
    }


     /*
     * @brief Runs Dijkstras 
     * 
     * @param searched dictionary to store searched nodes in
     * @param toSearch sorted set of nodes in ascending order of distance to start
     * @param startingSquare square to start search from
     * @param range range in squares around startingSquare to search, searches whole grid if -1
     */
    public void PlayerDijkstras()
    {
        SortedSet<DijkstrasNodeInfo> toSearch;
        toSearch = MapToSortedSet();
        Vector2 playerVector2 = new Vector2(player.transform.position.x, player.transform.position.y);
        Vector2Int playerTransform = Vector2Int.RoundToInt(playerVector2);
        Dijkstras(ref playerRange,ref toSearch,playerTransform,-1);
    }

}
