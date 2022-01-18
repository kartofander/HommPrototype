using System.Collections.Generic;

namespace Assets.Scripts.Pathfinding
{
    public class Node<T>
    {
        public Node<T> previous;
        public T obj;
        public int level;

        public List<T> BuildPath()
        {
            var result = new List<T>();
            var currentNode = this;

            while (currentNode.previous != null)
            {
                result.Add(currentNode.obj);
                currentNode = currentNode.previous;
            }

            result.Reverse();
            return result;
        }
    }
}