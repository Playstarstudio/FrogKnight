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

    // Stores our tiles and whether or not they are traversable
    private Dictionary<Vector2Int, TileInfo> map;

    private Dictionary<Vector2Int, Color> debugTiles;

    [SerializeField] private bool debug;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
            traversable.CompressBounds();
            notTraversable.CompressBounds();
            map = new Dictionary<Vector2Int, TileInfo>();
            debugTiles = new Dictionary<Vector2Int, Color>();
            CreateGrid();
        }
    }

    private void Update()
    {
        if (debug)
        {
            NoTint();
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
        foreach (var tile in debugTiles)
        {
            TintTile(tile.Key, tile.Value);
        }
    }

    public void AddDebugTile(Vector2Int tilePos, Color tint)
    {
        if (debug)
        {
            debugTiles.Add(tilePos, tint);
        }
    }
    private void NoTint()
    {
        foreach (var tile in map)
        {
            TintTile(tile.Key, Color.white);
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
    public class NodeInfo : IComparable<NodeInfo>
    {
        // which position on the map does this correspond to
        public Vector2Int position;

        // parent node
        public NodeInfo parent;

        // distance from origin in moves
        public int distance;

        public int CompareTo(NodeInfo other)
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

            if (!(obj is NodeInfo))
                return false;

            NodeInfo info = (NodeInfo)obj;
            // compare elements here
            return info.position == this.position;
        }

        public override int GetHashCode()
        {
            return (int)position.GetHashCode();
        }

        public List<NodeInfo> NeighborsToNodeInfos(List<Vector2Int> neighbors, NodeInfo parent)
        {
            List<NodeInfo> nodeInfos = new List<NodeInfo>();

            foreach (Vector2Int neighbor in neighbors)
            {
                NodeInfo current = new NodeInfo();
                current.position = neighbor;
                current.distance = distance + 1;
                current.parent = parent;
                nodeInfos.Add(current);
            }

            return nodeInfos;
        }

    }

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

    public void AStar(ref List<AStarNodeInfo> outputPath, ref Dictionary<AStarNodeInfo, AStarNodeInfo> searched, ref SortedSet<AStarNodeInfo> toSearch, Vector2Int startingSquare, Vector2Int target)
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


    // checks if a square is both traversable and unoccupied
    // if we give range = -1, search whole map
    public void Dijkstras(ref Dictionary<NodeInfo, NodeInfo> searched, ref SortedSet<NodeInfo> toSearch, Vector2Int startingSquare, int range)
    {
        toSearch.Clear();
        searched.Clear();
        NodeInfo start = new NodeInfo();
        start.position = startingSquare;
        start.distance = 0;
        start.parent = null;
        toSearch.Add(start);

        while (toSearch.Count > 0)
        {
            NodeInfo current = toSearch.Min;
            int currentDist = current.distance;
            toSearch.Remove(current);
            searched.Add(current, current.parent);

            List<NodeInfo> neighbors = current.NeighborsToNodeInfos(GetNeighbors(current.position), current);

            foreach (NodeInfo neighbor in neighbors)
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

}
