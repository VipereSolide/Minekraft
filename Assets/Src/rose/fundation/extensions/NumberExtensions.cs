using UnityEngine;

namespace com.rose.fundation.extensions
{
    public static class NumberExtensions
    {
        public static float Remap(this float value, float from1, float to1, float from2, float to2)
        {
            return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
        }

        public static Vector3 WithX(this Vector3 value, float x) { return new Vector3(x, value.y, value.z); }
        public static Vector3 WithY(this Vector3 value, float y) { return new Vector3(value.x, y, value.z); }
        public static Vector3 WithZ(this Vector3 value, float z) { return new Vector3(value.x, value.y, z); }

        public static Vector3Int WithX(this Vector3Int value, int x) { return new Vector3Int(x, value.y, value.z); }
        public static Vector3Int WithY(this Vector3Int value, int y) { return new Vector3Int(value.x, y, value.z); }
        public static Vector3Int WithZ(this Vector3Int value, int z) { return new Vector3Int(value.x, value.y, z); }
    }
}