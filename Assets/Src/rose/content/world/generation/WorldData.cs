using UnityEngine;

public static class WorldData
{
    public static readonly Vector3Int chunkSize = new(16, 16, 16);
    public static readonly int textureSize = 16;

    public static int horizontalRenderDistance = 8;
    public static int verticalRenderDistance = 2 * 16;
}