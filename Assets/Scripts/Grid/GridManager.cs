using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Common;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Assets.Scripts.Grid
{
    public class GridManager : MonoBehaviour
    {
        [SerializeField] private UnityEngine.Grid grid;
        [SerializeField] private Tilemap tilemap;

        [SerializeField] private TileBase defaultTile;
        [SerializeField] private TileBase selectedTile;
        [SerializeField] private TileBase availableTile;
        [SerializeField] private TileBase hostileTile;

        public const int GridWidth = 15;
        public const int GridHeight = 11;

        private GridCell[,] cells;

        public static GridManager instance;

        void Awake()
        {
            instance = this;
            cells = new GridCell[GridWidth, GridHeight];

            for (var x = 0; x < GridWidth; x++)
            {
                for (var y = 0; y < GridHeight; y++)
                {
                    var pos = new Vector2Int(x, y);
                    cells[x, y] = new GridCell(pos);
                }
            }
        }

        public void SetCreatureToCell(Vector2Int pos, Creature creature)
        {
            cells[pos.x, pos.y].occupier = creature;
        }

        public void SetTile(Vector2Int position, TileType tileType)
        {
            switch (tileType)
            {
                case TileType.Default:
                    tilemap.SetTile(position.ToVector3Int(), defaultTile);
                    break;
                case TileType.Selected:
                    tilemap.SetTile(position.ToVector3Int(), selectedTile);
                    break;
                case TileType.Available:
                    tilemap.SetTile(position.ToVector3Int(), availableTile);
                    break;
                case TileType.Hostile:
                    tilemap.SetTile(position.ToVector3Int(), hostileTile);
                    break;
                default:
                    tilemap.SetTile(position.ToVector3Int(), null);
                    break;
            }
        }

        public void ClearTiles()
        {
            for (var x = 0; x < GridWidth; x++)
            {
                for (var y = 0; y < GridHeight; y++)
                {
                    var cell = instance.cells[x, y];
                    instance.SetTile(cell.position, TileType.Default);
                }
            }
        }

        public GridCell GetCellByWorld(Vector3 worldPosition, bool checkIfWithinField = true)
        {
            var gridPos = GetGridPositionFromWorld(worldPosition, checkIfWithinField);

            return gridPos == null
                ? null
                : cells[gridPos.Value.x, gridPos.Value.y];
        }

        public Vector2Int? GetGridPositionFromWorld(Vector3 worldPosition, bool checkIfWithinField = true)
        {
            var position = grid.WorldToCell(worldPosition).ToVector2Int();
            return IsPositionWithinField(position) || checkIfWithinField == false
                ? position
                : (Vector2Int?)null;
        }

        public Vector3? GetWorldPositionFromGrid(Vector2Int position, bool checkIfWithinField = true)
        {
            return IsPositionWithinField(position) || checkIfWithinField == false
                ? grid.GetCellCenterWorld(position.ToVector3Int())
                : (Vector3?)null;
        }

        public GridCell GetCellFromMousePosition(bool checkIfWithinField = true)
        {
            var mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePos.z = 0;
            return GetCellByWorld(mousePos, checkIfWithinField);
        }

        public Vector2Int? GetGridPositionFromMousePosition(bool checkIfWithinField = true)
        {
            var mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePos.z = 0;
            return GetGridPositionFromWorld(mousePos, checkIfWithinField);
        }

        public GridCell GetCellFromGridPosition(Vector2Int position, bool checkIfWithinField = true)
        {
            return IsPositionWithinField(position) || checkIfWithinField == false
                ? cells[position.x, position.y]
                : null;
        }

        public bool IsPositionWithinField(Vector2Int position)
        {
            return position.x >= 0
                   && position.x <= GridWidth - 1
                   && position.y >= 0
                   && position.y <= GridHeight - 1;
        }

        public bool CheckIfNeighbors(Vector2Int a, Vector2Int b)
        {
            var aCell = cells[a.x, a.y];
            var aNeighbors = aCell.GetNeighbors();
            return aNeighbors.Any(neighbor => neighbor == b);
        }

        public GridCell GetRandomFreeNeighbor(Vector2Int position)
        {
            var cell = cells[position.x, position.y];
            var neighbors = cell.GetNeighbors();

            var list = new List<GridCell>();

            foreach (var neighbor in neighbors)
            {
                if (IsPositionWithinField(neighbor))
                {
                    var neighborCell = cells[neighbor.x, neighbor.y];
                    if (neighborCell.occupier == null)
                    {
                        list.Add(neighborCell);
                    }
                }
            }

            if (list.Count == 0) return null;

            var randomIndex = Random.Range(0, list.Count);
            return list[randomIndex];
        }
    }
}