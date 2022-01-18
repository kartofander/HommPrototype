using UnityEngine;

namespace Assets.Scripts.Grid
{
    public static class VectorExtensions
    {
        public static Vector2Int ToAxial(this Vector2Int vector)
        {
            return new Vector2Int(vector.x - vector.y / 2, vector.y);
        }

        public static Vector2Int FromAxial(this Vector2Int vector)
        {
            return new Vector2Int(vector.x + vector.y / 2, vector.y);
        }

        public static Vector3Int ToVector3Int(this Vector2Int vector)
        {
            return new Vector3Int(vector.x, vector.y, 0);
        }

        public static Vector2Int ToVector2Int(this Vector3Int vector)
        {
            return new Vector2Int(vector.x, vector.y);
        }
    }
}
