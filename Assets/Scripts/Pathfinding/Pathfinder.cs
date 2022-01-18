using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Grid;
using UnityEngine;

namespace Assets.Scripts.Pathfinding
{
    public class PathfinderResult
    {
        public List<Node<Vector2Int>> moveReachableNodes = new List<Node<Vector2Int>>();
        public List<Node<Vector2Int>> hitReachableNodes = new List<Node<Vector2Int>>();
    }

    public class Pathfinder
    {
        public static PathfinderResult FindAvailablePositions(
            Vector2Int start, 
            IEnumerable<Vector2Int> obstacles,
            int width, 
            int height, 
            int availableSteps)
        {
            var queue = new Queue<Node<Vector2Int>>();
            var result = new PathfinderResult();
            var explored = new HashSet<Vector2Int>();

            var startNode = new Node<Vector2Int>
            {
                obj = start,
                level = 0,
            };

            explored.Add(start);
            queue.Enqueue(startNode);

            while (queue.Count > 0)
            {
                var currentNode = queue.Dequeue();
                if (currentNode.level >= availableSteps + 1)
                {
                    continue;
                }

                var currentPosition = currentNode.obj;

                var gridCell = new GridCell(currentPosition);
                var neighbors = gridCell.GetNeighbors();

                foreach (var neighbor in neighbors)
                {
                    if (IsPositionWithinField(neighbor, width, height)
                        && explored.Contains(neighbor) == false)
                    {
                        var neighborNode = new Node<Vector2Int>()
                        {
                            obj = neighbor,
                            previous = currentNode,
                            level = currentNode.level + 1,
                        };

                        result.hitReachableNodes.Add(neighborNode);

                        if (obstacles.Contains(neighbor))
                        {
                            continue;
                        }
                        
                        queue.Enqueue(neighborNode);
                        explored.Add(neighbor);

                        if (neighborNode.level <= availableSteps)
                        {
                            result.moveReachableNodes.Add(neighborNode);
                        }
                    }
                }
            }

            return result;
        }

        private static bool IsPositionWithinField(Vector2Int position, int width, int height)
        {
            return position.x >= 0
                   && position.x <= width - 1
                   && position.y >= 0
                   && position.y <= height - 1;
        }
    }
}
