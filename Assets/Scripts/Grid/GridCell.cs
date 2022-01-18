using System;
using Assets.Scripts.Common;
using UnityEngine;

namespace Assets.Scripts.Grid
{
    [Serializable]
    public class GridCell
    {
        public GridCell(Vector2Int positionInGrid)
        {
            position = positionInGrid;
            axialPosition = positionInGrid.ToAxial();
        }

        public Creature occupier;
        public Vector2Int position { get; private set; }
        public Vector2Int axialPosition { get; private set; }

        public Vector2Int[] GetNeighbors()
        {
            return new[]
            {
                new Vector2Int(axialPosition.x + 1, axialPosition.y).FromAxial(),
                new Vector2Int(axialPosition.x - 1, axialPosition.y).FromAxial(),
                new Vector2Int(axialPosition.x, axialPosition.y + 1).FromAxial(),
                new Vector2Int(axialPosition.x, axialPosition.y - 1).FromAxial(),
                new Vector2Int(axialPosition.x - 1, axialPosition.y + 1).FromAxial(),
                new Vector2Int(axialPosition.x + 1, axialPosition.y - 1).FromAxial(),
            };
        }
    }
}