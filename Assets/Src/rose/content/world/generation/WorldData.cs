using UnityEngine;

public static class WorldData
{
    public static readonly Vector3Int chunkSize = new(16, 16, 16);
    public static readonly int textureSize = 16;

    public static int viewDistance = Mathf.RoundToInt(chunkSize.magnitude * 9);
}