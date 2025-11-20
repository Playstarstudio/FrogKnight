using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.WSA;
using Color = UnityEngine.Color;



public class GridManager : MonoBehaviour
{
    private static GridManager _instance;
    public static GridManager Instance { get { return _instance; } }

     public enum FOWEnum
        {
            NeverSeen,
            CurrentSeeing,
            PrevSeen
        }
    public class TileInfo
    {
        public bool traversable;
        public int rawDist;
        public float moveCost;
        public bool occupied = false;
        public bool visible;
        // enum neverSeen, currentSeeing, prevSeen
        public FOWEnum sightValue;
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

    //Store our Fog of War tiles
    [SerializeField]
    private Tilemap fowTiles;

    //Stores our range tiles
    [SerializeField]
    private Tilemap rangeTiles;

    private SortedSet<DijkstrasNodeInfo> sortedSet;


    // Stores our tiles and whether or not they are traversable
    public Dictionary<Vector2Int, TileInfo> map;

    // Stores tiles that we want to highlight when debugging
    private Dictionary<Vector2Int, Color> debugTiles;

    // If true we highlight debug tiles
    [SerializeField] private bool debug;

    //Player Related Data 
    public GameObject player;
    public Dictionary<DijkstrasNodeInfo, DijkstrasNodeInfo> playerRange;
    public Dictionary<Vector2Int, TileInfo> fowVision;
    public Dictionary<Vector2Int, DijkstrasNodeInfo> PlayerDijkstra;
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
            playerDebugTiles = new Dictionary<Vector2Int, Color>();
            fowTintedTiles = new Dictionary<Vector2Int, Color>();
            fowVision = new Dictionary<Vector2Int, TileInfo>();
            CreateGrid();
        }
        sortedSet = new SortedSet<DijkstrasNodeInfo>();
        player = GameObject.FindWithTag("Player");
        playerRange = new Dictionary<DijkstrasNodeInfo, DijkstrasNodeInfo>();
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

    public void AddDebugTile(Vector2Int tilePos, Color tint)
    {
        if (debug)
        {
            debugTiles.Add(tilePos, tint);
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
                // if it isnt traversible ignore this node
                if (!map[neighbor.position].traversable)
                {
                    continue;
                }
                neighbor.visible = HasLineOfSight(startingSquare, neighbor.position);
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
    private void CreateGrid()
    {
        for (int x = traversable.cellBounds.xMin-20; x < traversable.cellBounds.xMax+20; x++)
        {
            for (int y = traversable.cellBounds.yMin-20; y < traversable.cellBounds.yMax+20; y++)
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
    public void PlayerDijkstras()
    {
        playerDebugTiles.Clear();
        fowTintedTiles.Clear();
        fowVision.Clear();
        playerRange.Clear();
        SortedSet<DijkstrasNodeInfo> toSearch;
        toSearch = MapToSortedSet();
        Vector2 playerVector2 = new Vector2(player.transform.position.x, player.transform.position.y);
        Vector2Int playerTransform = Vector2Int.RoundToInt(playerVector2);
        Dijkstras(ref playerRange, ref toSearch, playerTransform, -1);
        if (PlayerDijkstra == null)
        {
            PlayerDijkstra = new Dictionary<Vector2Int, DijkstrasNodeInfo>();
        }
        else
        {
            PlayerDijkstra.Clear();
        }
        foreach (var entry in playerRange)
        {
            if (entry.Key != null)
            {

                PlayerDijkstra.Add(entry.Key.position, entry.Key);
                ChangeFOWValue(entry.Key);
            }
        }
        if (playerDebugRawDistTiles || playerDebugMoveCostTiles)
            ColorPlayerDebugTiles();
        if (fowDebugTilesOn)
            ColorFOWTiles();
    }
    void ConvertPlayerRangetoFOWVision()
    {
        TileInfo tile;
        
        foreach (var entry in playerRange)
        {
            bool exists = map.TryGetValue(entry.Key.position, out tile);
            fowVision.Add(entry.Key.position, tile);
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
                    fowTintedTiles.Add(new Vector2Int(x, y), Color.black);
                }
                else
                {
                    fowTintedTiles.Add(new Vector2Int(x, y), Color.black);
                }
                TintFOWTiles();
            }
        }
    }
    void ChangeFOWValue(DijkstrasNodeInfo node)
    {

        TileInfo tile;
        bool exists = map.TryGetValue(node.position, out tile);
        var visionAttribute = player.GetComponent<P_StateManager>().att.GetAttributeType("Vision Range");
        int visionRange = (int)player.GetComponent<P_StateManager>().att.GetCurrentAttributeValue(visionAttribute);
        if (!exists)
        {
            throw new ArgumentException("tile does not exist on grid");
        }

        if (node.visible)
        {
            if (node.rawDist <= visionRange)
            {
                tile.sightValue = FOWEnum.CurrentSeeing;
            }
        }
        else if (!node.visible || node.rawDist > visionRange)
        {
            if (tile.sightValue == FOWEnum.CurrentSeeing)
            {
                tile.sightValue = FOWEnum.PrevSeen;
            }
        }

    }
    private void TintFOWTiles()
    {
        Debug.Log("FOW Debug Tiles Count: " + fowTintedTiles.Count());
        TintDebugTiles(fowTintedTiles);
        if (fowTintedTiles.Count() >= 1)
        {
            foreach (var tile in fowTintedTiles)
            {
                if (fowTintedTiles.ContainsKey(tile.Key))
                {
                    //TintTile(tile.Key, fowTintedTiles[tile.Key]);
                    fowTiles.SetColor(new Vector3Int(tile.Key.x, tile.Key.y, 0), fowTintedTiles[tile.Key]);
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
    void ColorFOWTiles()
    {
        TileInfo currentTile;
        Color tileTint = Color.white;
        var visionAttribute = player.GetComponent<P_StateManager>().att.GetAttributeType("Vision Range");
        int visionRange = (int)player.GetComponent<P_StateManager>().att.GetCurrentAttributeValue(visionAttribute);
        ConvertPlayerRangetoFOWVision();

        if (fowDebugTilesOn)
        {
            foreach (var entry in fowVision)
            {
                currentTile = entry.Value;
                if (currentTile.sightValue == FOWEnum.NeverSeen)
                {
                    tileTint = new Color(0, 0, 0, 1);
                }
                else if (currentTile.sightValue == FOWEnum.PrevSeen)
                {
                    tileTint = new Color(0, 0, 0, (float)0.5);
                }
                else if (currentTile.sightValue == FOWEnum.CurrentSeeing)
                {
                    tileTint = new Color(0, 0, 0, 0);
                }
                //Debug.Log("Key Value: " + map.FirstOrDefault(x => x.Value == currentTile).Key);
                AddFOWDebugTile(map.FirstOrDefault(x => x.Value == currentTile).Key, tileTint);
            }
            TintFOWTiles();
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
    /*
     */
    public bool HasLineOfSight(Vector2Int from, Vector2Int to)
    {
        // If start and end are the same position
        if (from == to) return true;
        int distSqr = (to - from).sqrMagnitude;
        if (distSqr <= 2) return true; //adjacent tiles always have LOS

        List<Vector2Int> line = GetBresenhamLine(from, to);

        int step = distSqr > 20 ? 2 : 1; // Adjust threshold based on your needs
        for (int i = 1; i < line.Count - 1; i += step)
        {
            if (IsVisionBlocking(line[i]))
                return false;
        }
        return true;
        /*
        Vector2 start = new Vector2(from.x + 0.5f, from.y + 0.5f); // Center of tile
            Vector2 end = new Vector2(to.x + 0.5f, to.y + 0.5f);
            Vector2 direction = (end - start).normalized;
            float distance = Vector2.Distance(start, end);

            // Adjust layer mask for your obstacles
            RaycastHit2D[] hit = Physics2D.RaycastAll(start, direction, distance - 0.1f);
            if (hit.Length > 0)
            {
                for (int i = 0; i < hit.Length; i++)
                {
                    if (hit[i].collider != null)
                    {
                        // Check if the hit object is an obstacle
                        if (hit[i].collider.gameObject.CompareTag("Obstacle"))
                        {
                            return false; // Line of sight is blocked
                        }
                        else
                        {
                            continue; // Not an obstacle, continue checking
                        }
                    }
                }
            }
            return true;
        }
        */
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

    public bool IsVisionBlocking(Vector2Int tile)
    {
        if (map[tile].traversable == false)
        {
            return true;
        }
        Collider2D[] colliders = Physics2D.OverlapPointAll(tile);
        foreach (Collider2D collider in colliders)
        {
            Entity entity = collider.GetComponent<Entity>();
            if (entity != null)
            {
                if (entity.gameObject.CompareTag("Obstacle"))
                {
                    return true;
                }
            }
        }

        return false;

    }
    public bool GetTileOccupancy(Vector2Int tile)
    {
        if (map[tile].occupied)
            return true;
        else
            return false;
    }

}
