using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
using static GameManager;
using Color = UnityEngine.Color;



public class GridManager : MonoBehaviour
{
    public GameManager gameManager;
    private static GridManager _instance;
    public static GridManager Instance { get { return _instance; } }
    public enum FOWEnum
    {
        NeverSeen,
        CurrentlySeeing,
        PrevSeen
    }
    public class TileInfo
    {
        public bool traversable;
        public int rawDist;
        public float moveCost;
        public bool wall = false;
        public bool isVisionBlocking = false;
        public bool occupied = false;
        public List<Entity> occupyingEntities = new List<Entity>();
        public List<ItemOnGround> occupyingItems = new List<ItemOnGround>();
        public bool LoS = false;
        public bool visible = false;
        public FOWEnum sightValue;
        public TileInfo(bool traversable)
        {
            this.traversable = traversable;
        }
        public TileInfo(bool traversable, bool isVisionBlocking, bool wall)
        {
            this.traversable = traversable;
            this.isVisionBlocking = isVisionBlocking;
            this.wall = wall;
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

    //Store our Fog of War tiles
    [SerializeField]
    private Tilemap fowTiles;

    //Stores our BG tiles
    [SerializeField]
    private Tilemap bgTiles;

    //Stores our Cosmetic tiles
    [SerializeField]
    private Tilemap cosmeticTiles;

    //Stores our Range tiles
    [SerializeField]
    public Tilemap rangeTiles;

    private SortedSet<DijkstrasNodeInfo> sortedSet;


    // Stores our tiles and all data
    public Dictionary<Vector2Int, TileInfo> map;

    // Stores tiles that we want to highlight when debugging
    private Dictionary<Vector2Int, Color> debugTiles;

    // If true we highlight debug tiles
    [SerializeField] private bool debug;

    //Player Related Data 
    public GameObject player;
    public Dictionary<DijkstrasNodeInfo, DijkstrasNodeInfo> playerRange;
    public bool playerDebugRawDistTiles;
    public bool playerDebugMoveCostTiles;
    public bool losDebug;
    public bool fowDebugTilesOn;
    public Dictionary<Vector2Int, Color> playerDebugTiles;
    public Dictionary<Vector2Int, Color> fowTintedTiles;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            traversable.CompressBounds();
            notTraversable.CompressBounds();
            doorOptions.CompressBounds();
            fowTiles.CompressBounds();
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
            fowTiles.CompressBounds();
            map = new Dictionary<Vector2Int, TileInfo>();
            CreateGrid();
        }
        debugTiles = new Dictionary<Vector2Int, Color>();
        playerDebugTiles = new Dictionary<Vector2Int, Color>();
        fowTintedTiles = new Dictionary<Vector2Int, Color>();
        sortedSet = new SortedSet<DijkstrasNodeInfo>();
        player = GameObject.FindWithTag("Player");
        playerRange = new Dictionary<DijkstrasNodeInfo, DijkstrasNodeInfo>();
        var foundEntities = FindObjectsByType<Entity>(FindObjectsSortMode.None);
        if (fowDebugTilesOn)
        {
            fowTiles.GetComponent<TilemapRenderer>().enabled = true;
        }
        else if (!fowDebugTilesOn)
        {
            fowTiles.GetComponent<TilemapRenderer>().enabled = false;
        }
    }

    private void Start()
    {
        ColorRangeTiles();
        PlayerDijkstras();
    }
    private void Update()
    {
        /*
        if (debug)
        {
            TintDebug();
            debugTiles.Clear();
        }
        if (playerDebugTilesOn)
        {
            TintPlayerDebug();
            playerDebugTiles.Clear();
        }
         */

        if (playerDebugRawDistTiles)
        {
            playerDebugMoveCostTiles = false;
        }
        else if (playerDebugMoveCostTiles)
        {
            playerDebugRawDistTiles = false;
        }
    }
    /*
     * @brief Gets the center of the tile at the given grid position
     *             ** WHEN TO USE THIS METHOD VS. GETCELLPOSITION **
     *        This method, GetTileCenter, gives us the **center** of the cell
     *        that we are concerned with in **world space**. This is the spatial position of
     *        the cell, so we want to use it when were asking the grid something
     *        about this tile where we care about where it is in space. GetCellPosition
     *        will give the origin of this tile, which is how we identify the tile in the grid,
     *        so we want to use GetCellPosition when are are asking questions about this specific tile
     *        in the context of the grid.
     * 
     * @param gridPos the position in grid coordinates of the cell we
     *        are interested in
     *        
     * @return the coordinates, in world space, of the center of the given tile
     */
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

    /*
     * @brief Gets the origin of a cell (the corner) given a position in world space
     *                ** WHEN TO USE THIS METHOD VS. GETTILECENTER **
     *        This method, GetCellPosition, gives us the **origin** of the cell
     *        that we are concerned with. This is the "official" position of
     *        the cell, so we want to use it when were asking the grid something
     *        about this tile (GetWorldPosition, AddPlayerDebugTile, etc.). GetTileCenter
     *        will give the geometric center of this tile, which is useful for drawing line
     *        of sight, calculating distance, or another operation where we care about
     *        physical space.
     * 
     * @param worldPos the position in world position that we are looking at
     * 
     * @return the coordinates of orin of the relevant cell
     */
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
        public int rawDist;
        public int moveCost;
        public bool LoS;
        public bool visible;
        public int CompareTo(DijkstrasNodeInfo other)
        {
            int dist = rawDist - other.rawDist;
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
                current.rawDist = rawDist + 1;
                //temporary
                current.moveCost = 1;
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

    public bool SetDijkstraVisibility(Vector2Int from, Vector2Int to)
    {
        // If start and end are the same position

        if (from == to)
        {
            map[to].LoS = true;
            map[to].visible = true;
            return true;
        }
        float dist = (to - from).magnitude;
        if (dist <= 1.5)
        {
            map[to].LoS = true;
            map[to].visible = true;
            return true; //adjacent tiles always have LOS
        }
        List<Vector2Int> line = GetBresenhamLine(from, to);
        int step = dist > 20 ? 2 : 1; // Adjust threshold based on your needs
        for (int i = 1; i < line.Count - 1; i++)
        {
            if (map[line[i]].wall == true || map[line[i]].isVisionBlocking == true)
            {
                map[to].LoS = false;
                map[to].visible = false;
                return true;
            }
            else if (!map.ContainsKey(line[i]))
            {
                return false;
            }
            if (IsVisionBlockingCheck(line[i]))
            {
                map[to].LoS = false;
                map[to].visible = false;
                return false;
            }
            else map[to].LoS = true;
        }
        if (map[to].LoS)
        {
            AttributeType visionAttribute = player.GetComponent<P_StateManager>().att.GetAttributeType("Vision Range");
            int visionRange = (int)player.GetComponent<P_StateManager>().att.GetCurrentAttributeValue(visionAttribute);
            if (dist <= visionRange)
            {
                map[to].visible = true;
                return true;
            }
            else
            {
                map[to].visible = false;
                return false;
            }
        }
        else if (!map[to].LoS)
        {
            map[to].visible = false;
        }
        if (map[to].visible && map[to].LoS)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public List<Vector2Int> GetBresenhamLine(Vector2Int from, Vector2Int to)
    {
        List<Vector2Int> line = new List<Vector2Int>();

        int x = from.x;
        int y = from.y;
        int x2 = to.x;
        int y2 = to.y;

        int w = x2 - x;
        int h = y2 - y;
        int dx1 = 0, dy1 = 0, dx2 = 0, dy2 = 0;

        if (w < 0) dx1 = -1; else if (w > 0) dx1 = 1;
        if (h < 0) dy1 = -1; else if (h > 0) dy1 = 1;
        if (w < 0) dx2 = -1; else if (w > 0) dx2 = 1;

        int longest = Mathf.Abs(w);
        int shortest = Mathf.Abs(h);

        if (!(longest > shortest))
        {
            longest = Mathf.Abs(h);
            shortest = Mathf.Abs(w);
            if (h < 0) dy2 = -1; else if (h > 0) dy2 = 1;
            dx2 = 0;
        }

        int numerator = longest >> 1; // Divide by 2

        for (int i = 0; i <= longest; i++)
        {
            line.Add(new Vector2Int(x, y));

            numerator += shortest;
            if (!(numerator < longest))
            {
                numerator -= longest;
                x += dx1;
                y += dy1;
            }
            else
            {
                x += dx2;
                y += dy2;
            }
        }

        return line;
    }

    public bool IsVisionBlockingCheck(Vector2Int tile)
    {
        if (map[tile].wall == true || map[tile].isVisionBlocking == true)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    public bool GetTileOccupancy(Vector2Int tile)
    {
        if (map[tile].occupied)
            return true;
        else
            return false;
    }

    public void PlayerDijkstras()
    {
        playerDebugTiles.Clear();
        fowTintedTiles.Clear();
        playerRange.Clear();
        SortedSet<DijkstrasNodeInfo> toSearch;
        toSearch = MapToSortedSet();
        Vector2Int playerTransform = GetCellPosition(player.transform.position);
        AttributeType visionAttribute = player.GetComponent<P_StateManager>().att.GetAttributeType("Vision Range");
        int playerVision = (int)player.GetComponent<P_StateManager>().att.GetCurrentAttributeValue(visionAttribute);
        Dijkstras(ref playerRange, ref toSearch, playerTransform, -1);
        map[playerTransform].visible = true;
        map[playerTransform].LoS = true;
        ChangeFOWValue(playerTransform);
        /*
        foreach (var entry in playerRange)
        {
            if (entry.Key != null)
            {
                tile = entry.Key.position;
                SetDijkstraVisibility(playerTransform, tile);
                ChangeFOWValue(tile);
            }
        }
         */
        Vector2Int min = new Vector2Int();
        Vector2Int max = new Vector2Int();
        MinMaxRange(out min, out max);
        for (int x = min.x - 5; x < max.x + 5; x++)
        {
            for (int y = min.y - 5; y < max.y + 5; y++)
            {
                SetDijkstraVisibility(playerTransform, new Vector2Int(x, y));
                ChangeFOWValue(new Vector2Int(x, y));
            }
        }
        foreach (TimedEntity entity in gameManager.sortedTimedEntities)
        {
            var timedEntity = entity.entity.GetComponent<Enemy>();
            DisplayOrHideEntity(timedEntity);
        }
        if (playerDebugRawDistTiles || playerDebugMoveCostTiles)
            ColorPlayerDebugTiles();
        if (fowDebugTilesOn)
            ColorFOWTiles(min, max);
    }


    private bool MinMaxRange(out Vector2Int min, out Vector2Int max)
    {
        min = new Vector2Int(int.MaxValue, int.MaxValue);
        max = new Vector2Int(int.MinValue, int.MinValue);
        if (playerRange == null || playerRange.Count == 0)
        {
            Debug.Log("Player range is empty or null");
            min = Vector2Int.zero;
            max = Vector2Int.zero;
            return false;
        }
        foreach (var entry in playerRange.Keys)
        {
            Vector2Int pos = entry.position;
            if (pos.x < min.x)
                min.x = pos.x;
            if (pos.y < min.y)
                min.y = pos.y;
            if (pos.x > max.x)
                max.x = pos.x;
            if (pos.y > max.y)
                max.y = pos.y;
        }
        return true;
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
                if (GetTileOccupancy(neighbor.position) && neighbor.position != target)// == true && current.position != startingSquare)
                {
                    Debug.Log($"Neighbor {neighbor.position} is occupied - skipped");
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
        //if we exit the while loop, no path was found
        searched.Clear();
        target = startingSquare;
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
        start.rawDist = 0;
        start.moveCost = 1;
        start.parent = null;
        toSearch.Add(start);

        while (toSearch.Count > 0)
        {
            DijkstrasNodeInfo current = toSearch.Min;
            int currentDist = current.rawDist;
            int currentMoveCost = current.moveCost;
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
                /*
                // if were out of our range, ignore these nodes
                int distance = currentDist + 1;
                int moveCost = currentMoveCost + 1;
                if (distance > range && range != -1)
                {
                    continue;
                }
                 */
                /*
            if (GetTileOccupancy(neighbor.position))// == true && current.position != startingSquare)
            {
                Debug.Log($"Neighbor {neighbor.position} is occupied - skipped");
                continue;
            }
                 */
                /////////////SetDijkstraVisibility(startingSquare, neighbor);
                //SetDijkstraVisibility(startingSquare, neighbor);
                if (!map[neighbor.position].traversable)
                {
                    // if it isnt traversible ignore this node
                    continue;
                }
                if (searched.ContainsKey(neighbor))
                {
                    // if already in searched list, dont 
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


    private void CreateGrid()
    {
        for (int x = traversable.cellBounds.xMin - 20; x < traversable.cellBounds.xMax + 20; x++)
        {
            for (int y = traversable.cellBounds.yMin - 20; y < traversable.cellBounds.yMax + 20; y++)
            {
                Vector3 worldPosition = traversable.CellToWorld(new Vector3Int(x, y, 0));
                if (notTraversable.HasTile(notTraversable.WorldToCell(worldPosition)))
                {
                    map.Add(new Vector2Int(x, y), new TileInfo(false, true, true));
                }
                else
                {
                    map.Add(new Vector2Int(x, y), new TileInfo(true, false, false));
                }
                map[new Vector2Int(x, y)].occupyingEntities = FindEntities(new Vector2Int(x, y));
                CalculateTileData(new Vector2Int(x, y));
            }
        }
    }
    private List<Entity> FindEntities(Vector2Int pos)
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(GetTileCenter(pos), 0.1f);
        List<Entity> entities = new List<Entity>();
        foreach (var collider in colliders)
        {
            Entity entity = collider.GetComponent<Entity>();
            if (entity != null)
            {
                entities.Add(entity);
                map[pos].occupied = true;
            }
        }
        return entities;
    }
    public bool TryGetEntityOnTile(Vector2Int pos, out Entity entityOnTile)
    {
        TileInfo tileInfo;
        map.TryGetValue(pos, out tileInfo);
        if (tileInfo.occupyingEntities.Count == 0)
        {
            entityOnTile = null;
            return false;
        }
        //foreach (Entity entity in tileInfo.occupyingEntities)
        for (int i = 0; i < tileInfo.occupyingEntities.Count; i++)
        {
            if (tileInfo.occupyingEntities[i].GetType() == typeof(P_StateManager) || tileInfo.occupyingEntities[i].GetType() == typeof(Enemy))
            {
                entityOnTile = tileInfo.occupyingEntities[i];
                Debug.Log("IFOUNDONE");
                return true;
            }
            else
            {
                continue;
            }
        }
        entityOnTile = null;
        Debug.Log("NOONEFOUNDIDIOT");
        return false;
    }
    public bool TryGetItemsOnTile(Vector2Int pos, out List<ItemOnGround> itemList)
    {
        TileInfo tileInfo;
        map.TryGetValue(pos, out tileInfo);
        if (tileInfo.occupyingItems.Count == 0)
        {
            itemList = new List<ItemOnGround>();
            return false;
        }
        else
        {
            itemList = tileInfo.occupyingItems;
            return true;
        }
    }

    public bool TraversableCheck(Vector2Int pos)
    {
        TileInfo tile;
        map.TryGetValue(pos, out tile);
        if (tile != null)
        {
            Vector3Int posn = new Vector3Int(pos.x, pos.y, 0);
        }
        else if (tile == null)
        {
            return false;
        }
        if (tile.traversable && !tile.occupied)
        {
            return true;
        }
        else
        {
            return false;
        }
    }


    /* @brief Gets the manhattan distance from one tile to another
     * 
     * @param start starting tile
     * @param end end tile
     * 
     * @return the manhattan distance from the start square to the end square as an integer
     */
    public int ManhattanDistanceToTile(Vector2Int start, Vector2Int end)
    {
        return Math.Abs(start.x - end.x) + Math.Abs(start.y - end.y);
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
    private void MergeIntoFOW(ref Tilemap sourceTilemap)
    {
        MergeTilemaps(ref fowTiles, ref sourceTilemap, false);
    }
    private void MergeIntoBG(ref Tilemap sourceTilemap)
    {
        MergeTilemaps(ref bgTiles, ref sourceTilemap, false);
    }
    private void MergeIntoRange(ref Tilemap sourceTilemap)
    {
        MergeTilemaps(ref rangeTiles, ref sourceTilemap, false);
    }
    private void MergeIntoCosmetics(ref Tilemap sourceTilemap)
    {
        MergeTilemaps(ref cosmeticTiles, ref sourceTilemap, false);
    }


    /*
    * @brief Converts the current grid map to a sorted set for Dijkstra
    * 
       private Dictionary<Vector2Int, TileInfo> map;
    */
    public SortedSet<DijkstrasNodeInfo> MapToSortedSet()
    {
        sortedSet.Clear();
        DijkstrasNodeInfo currentNode;

        foreach (KeyValuePair<Vector2Int, TileInfo> tile in map)
        {
            currentNode = new DijkstrasNodeInfo();
            currentNode.position = tile.Key;
            currentNode.parent = null;
            currentNode.rawDist = 1;
            currentNode.moveCost = 1;
            sortedSet.Add(currentNode);
        }
        return sortedSet;
    }


    /*
    * @brief Runs Dijkstras from player's position
    * 
    */
    private void ColorRangeTiles()
    {
        rangeTiles.GetComponent<TilemapRenderer>().enabled = true;
        for (int x = rangeTiles.cellBounds.xMin - 20; x < fowTiles.cellBounds.xMax + 20; x++)
        {
            for (int y = rangeTiles.cellBounds.yMin - 20; y < fowTiles.cellBounds.yMax + 20; y++)
            {
                Vector3 worldPosition = fowTiles.CellToWorld(new Vector3Int(x, y, 0));
                Vector2Int cell = GetCellPosition(worldPosition);
                if (rangeTiles.HasTile(fowTiles.WorldToCell(worldPosition)))
                {
                    TintTile(cell, new Color(0, 0, 0, 0), rangeTiles);
                }
            }
        }
    }

    //applies the color to the FOW tilemap

    private void CreateFOWGrid()
    {
        for (int x = fowTiles.cellBounds.xMin - 20; x < fowTiles.cellBounds.xMax + 20; x++)
        {
            for (int y = fowTiles.cellBounds.yMin - 20; y < fowTiles.cellBounds.yMax + 20; y++)
            {
                Vector3 worldPosition = fowTiles.CellToWorld(new Vector3Int(x, y, 0));
                if (fowTiles.HasTile(fowTiles.WorldToCell(worldPosition)))
                {
                    fowTintedTiles.Add(new Vector2Int(x, y), Color.white);
                }
                else
                {
                    fowTintedTiles.Add(new Vector2Int(x, y), Color.white);
                }
                TintFOWTiles();
            }
        }
    }

    void ChangeFOWValue(Vector2Int node)
    {
        TileInfo tile;
        bool exists = map.TryGetValue(node, out tile);
        var visionAttribute = player.GetComponent<P_StateManager>().att.GetAttributeType("Vision Range");
        int visionRange = (int)player.GetComponent<P_StateManager>().att.GetCurrentAttributeValue(visionAttribute);
        if (!exists)
        {
            throw new ArgumentException("tile does not exist on grid");
        }
        if (tile.visible)
        {
            if (tile.rawDist <= visionRange)
            {
                tile.sightValue = FOWEnum.CurrentlySeeing;
            }
        }
        else if (!tile.visible || tile.rawDist > visionRange)
        {
            if (tile.sightValue == FOWEnum.CurrentlySeeing)
            {
                tile.sightValue = FOWEnum.PrevSeen;
            }
        }
        HandleDisplayOrHideAll(node);
    }
    void ColorFOWTiles(Vector2Int min, Vector2Int max)
    {
        TileInfo currentTile;
        Color tileTint = Color.white;
        AttributeType visionAttribute = player.GetComponent<P_StateManager>().att.GetAttributeType("Vision Range");
        int visionRange = (int)player.GetComponent<P_StateManager>().att.GetCurrentAttributeValue(visionAttribute);

        if (fowDebugTilesOn)
        {
            for (int x = min.x - 5; x < max.x + 5; x++)
            {
                for (int y = min.y - 5; y < max.y + 5; y++)
                {
                    Vector2Int tile = new Vector2Int(x, y);
                    if (tile != null)
                    {
                        map.TryGetValue(tile, out currentTile);
                        if (currentTile == null)
                        {
                            continue;
                        }
                    }
                    if (map[tile].sightValue == FOWEnum.NeverSeen)
                    {
                        tileTint = new Color(9, 10, 20, 1);
                    }
                    else if (map[tile].sightValue == FOWEnum.PrevSeen)
                    {
                        tileTint = new Color(90, 10, 20, .6f);
                    }
                    else if (map[tile].sightValue == FOWEnum.CurrentlySeeing)
                    {
                        tileTint = new Color(0, 0, 0, 0);
                    }
                    //Debug.Log("Key Value: " + map.FirstOrDefault(x => x.Value == currentTile).Key);
                    AddFOWDebugTile(tile, tileTint);
                }
            }
            TintFOWTiles();
        }
    }
    private void TintFOWTiles()
    {
        //Debug.Log("FOW Debug Tiles Count: " + fowTintedTiles.Count());
        //TintDebugTiles(fowTintedTiles);
        if (fowTintedTiles.Count() >= 1)
        {
            foreach (var tile in fowTintedTiles)
            {
                if (fowTintedTiles.ContainsKey(tile.Key))
                {
                    //TintTile(tile.Key, fowTintedTiles[tile.KeyDisplayOrHideEntity
                    TintTile(tile.Key, fowTintedTiles[tile.Key], fowTiles);
                }
                else
                {
                    TintTile(tile.Key, Color.white, fowTiles);
                }
            }
        }
        else
        {
            return;
        }
    }

    public void CalculateTileData(Vector2Int tile)
    {
        /*
        public bool traversable; does need to be calculated
        public bool isVisionBlocking;
        public bool occupied = false;
         */
        TileInfo tileInfo;
        map.TryGetValue(tile, out tileInfo);
        if (tileInfo == null)
        {

            return;
        }
        if (map[tile].wall)
        {
            map[tile].occupied = true;
            map[tile].isVisionBlocking = true;
            map[tile].traversable = false;
            ChangeFOWValue(tile);

            return;
        }
        else if (map[tile].occupyingEntities.Count == 0)
        {
            map[tile].occupied = false;
            map[tile].isVisionBlocking = false;
            map[tile].traversable = true;
            ChangeFOWValue(tile);

            return;
        }
        if (map[tile].occupyingEntities.Count > 0)
        {
            map[tile].occupied = true;
            foreach (var entity in map[tile].occupyingEntities)
            {
                if (entity.CompareTag("Obstacle"))
                {
                    map[tile].occupied = true;
                    map[tile].isVisionBlocking = true;
                    map[tile].traversable = false;
                    ChangeFOWValue(tile);

                    return;
                }
                else if (entity.CompareTag("Entity"))
                {
                    map[tile].occupied = true;
                    map[tile].isVisionBlocking = false;
                    map[tile].traversable = false;
                }
            }
        }
        ChangeFOWValue(tile);
        return;
    }
    public void MapMoveEntity(Entity entity, Vector2Int from, Vector2Int to)
    {
        MapRemoveEntity(entity, from);
        MapAddEntity(entity, to);
        entity.currentTile = to;
    }
    public void MapAddEntity(Entity entity, Vector2Int tilePos)
    {
        map[tilePos].occupyingEntities.Add(entity);
        entity.currentTile = tilePos;
        CalculateTileData(tilePos);
        DisplayOrHideEntity(entity);
    }
    public void MapRemoveEntity(Entity entity, Vector2Int tilePos)
    {
        map[tilePos].occupyingEntities.RemoveAll(e => e == entity);
        CalculateTileData(tilePos);
        DisplayOrHideEntity(entity);

    }
    public void MapAddItem(ItemOnGround item, Vector2Int tilePos)
    {
        map[tilePos].occupyingItems.Add(item);
        CalculateTileData(tilePos);
        DisplayOrHideItem(item, tilePos);
    }
    public void MapRemoveItem(ItemOnGround item, Vector2Int tilePos)
    {
        map[tilePos].occupyingItems.Remove(item);
        Destroy(item);
        CalculateTileData(tilePos);
    }
    public void MapMoveItem(ItemOnGround item, Vector2Int from, Vector2Int to)
    {
        map[from].occupyingItems.Remove(item);
        map[to].occupyingItems.Add(item);
        CalculateTileData(from);
        CalculateTileData(to);
        DisplayOrHideItem(item, to);
    }
    public void DisplayOrHideEntity(Entity gameObject)
    {
        Vector2 objectPos = GetTileCenter(GetCellPosition(gameObject.transform.position));
        if (objectPos != null)
        {
            SpriteRenderer render = gameObject.GetComponent<SpriteRenderer>();
            if (map[GetCellPosition(objectPos)].visible)
            {
                render.enabled = true;
            }
            else if (!map[GetCellPosition(objectPos)].visible)
            {
                render.enabled = false;
            }
        }
    }
    public void DisplayOrHideItem(ItemOnGround gameObject, Vector2Int tile)
    {
        Vector2 objectPos = GetTileCenter(tile);
        if (objectPos != null)
        {
            SpriteRenderer render = gameObject.GetComponent<SpriteRenderer>();
            if (map[GetCellPosition(objectPos)].visible)
            {
                render.enabled = true;
            }
            else if (!map[GetCellPosition(objectPos)].visible)
            {
                render.enabled = false;
            }
        }
    }

    public void HandleDisplayOrHideAll(Vector2Int tile)
    {
        TileInfo tileInfo;
        if (map.TryGetValue(tile, out tileInfo))
        {
            if (tileInfo.visible)
            {
                foreach (Entity entity in tileInfo.occupyingEntities)
                {
                    if (entity != null)
                    {
                        entity.GetComponent<SpriteRenderer>().enabled = true;
                    }
                }
                foreach (ItemOnGround item in tileInfo.occupyingItems)
                {
                    if (item != null)
                    {
                        item.GetComponent<SpriteRenderer>().enabled = true;
                    }
                }
            }
            else if (!tileInfo.visible)
            {
                foreach (Entity entity in tileInfo.occupyingEntities)
                {
                    if (entity != null)
                    {
                        entity.GetComponent<SpriteRenderer>().enabled = false;
                    }
                }
                foreach (ItemOnGround item in tileInfo.occupyingItems)
                {
                    if (item != null)
                    {
                        item.GetComponent<SpriteRenderer>().enabled = false;
                    }
                }
            }
        }
    }

    #region tile coloring
    public void TintTile(Vector2Int gridPos, Color color)
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
    public void TintTile(Vector2Int gridPos, Color color, Tilemap _tileMap)
    {
        TileInfo tile;
        bool exists = map.TryGetValue(gridPos, out tile);
        Vector3Int posn = new Vector3Int(gridPos.x, gridPos.y, 0);

        if (!exists)
        {
            throw new ArgumentException("tile does not exist on grid");
        }
        else
        {
            _tileMap.SetTileFlags(posn, TileFlags.None);
            _tileMap.SetColor(posn, color);
            //Debug.Log(color);
        }

    }
    #endregion
    #region debug
    public void AddDebugTile(Vector2Int tilePos, Color tint)
    {
        if (debug)
        {
            debugTiles.Add(tilePos, tint);
        }
    }
    private void TintDebugTiles(Dictionary<Vector2Int, Color> dictionary)
    {
        if (dictionary.Count() >= 1)
        {
            foreach (var tile in dictionary)
            {
                if (dictionary.ContainsKey(tile.Key))
                {
                    TintTile(tile.Key, dictionary[tile.Key]);
                }
                else
                {
                    TintTile(tile.Key, Color.white);
                }
            }
        }
        else
        {
            return;
        }
    }

    public void AddPlayerDebugTile(Vector2Int tilePos, Color tint)
    {
        if (!playerDebugTiles.ContainsKey(tilePos) && (playerDebugRawDistTiles || playerDebugMoveCostTiles))
        {
            playerDebugTiles.Add(tilePos, tint);
        }
        else
        {
            Debug.Log(tilePos + "already exists in Dictonary");
        }
    }
    public void AddFOWDebugTile(Vector2Int pos, Color tint)
    {
        if (!fowTintedTiles.ContainsKey(pos) && (fowDebugTilesOn))
        {
            fowTintedTiles.Add(pos, tint);
        }
        else
        {
            Debug.Log(pos + "already exists in Dictonary");
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
    void ColorPlayerDebugTiles()
    {
        //Raw Dist Debug will be Blue
        //Move Cost Debug will be Purple
        Dictionary<DijkstrasNodeInfo, DijkstrasNodeInfo>.KeyCollection allKeys = playerRange.Keys;
        Vector2Int currentPos;
        int currentRawDistance;
        int currentMoveCost;
        Color tileTint;
        int rValue;
        int gValue;
        int bValue;
        if (playerDebugRawDistTiles)
        {
            foreach (var item in allKeys)
            {
                currentRawDistance = item.rawDist;
                currentPos = item.position;
                rValue = currentRawDistance * 2;
                gValue = currentRawDistance * 2;
                if (rValue > 150)
                    rValue = 150;
                if (item.visible == false && this.losDebug)
                    rValue = 0;
                tileTint = new Color(rValue * .025f, 0, 200);
                AddPlayerDebugTile(currentPos, tileTint);
            }
        }
        else if (playerDebugMoveCostTiles)
        {
            foreach (var item in allKeys)
            {
                currentMoveCost = item.moveCost;
                currentPos = item.position;

                rValue = currentMoveCost * 10;
                bValue = currentMoveCost * 10;
                if (rValue > 255)
                    rValue = 255;
                if (bValue > 255)
                    bValue = 255;
                tileTint = new Color(rValue, 0, bValue);
                AddPlayerDebugTile(currentPos, tileTint);
            }
        }
        TintPlayerDebug();
    }

    //Adds every tile to FOW dictionary


    private void TintPlayerDebug()
    {
        //Debug.Log("Player Debug Tiles Count: " + playerDebugTiles.Count());
        TintDebugTiles(playerDebugTiles);
    }
    #endregion


    public bool ReturnTileData(Vector2Int tile)
    {
        CalculateTileData(tile);
        Debug.Log(map[tile].ToString());
        return true;
    }
}
