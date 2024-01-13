using UnityEngine;

public static class WorldData
{
    public static readonly Vector3Int chunkSize = new(16, 16, 16);
    public static readonly int textureSize = 16;

    /// <summary>
    /// Whether the block placing indicator (green one) should be active or not.
    /// </summary>
    public static readonly bool activateBlockPlaceIndicator = false;

    /// <summary>
    /// Amount of chunks that can be treated at the same time.
    /// </summary>
    public static int chunkUpdateRoutineWorkers = 6;

    /// <summary>
    /// Number of chunks rendered around the player.
    /// </summary>
    public static int horizontalRenderDistance = 8;
    //public static int verticalRenderDistance = 2 * 16;
}