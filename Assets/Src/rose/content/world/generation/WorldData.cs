using UnityEngine;

public static class WorldData
{
    public static readonly Vector3Int chunkSize = new(32, 32, 32);
    public static readonly int textureSize = 16;

    /// <summary>
    /// Whether the block placing indicator (green one) should be active or not.
    /// </summary>
    public static readonly bool activateBlockPlaceIndicator = true;

    /// <summary>
    /// Amount of chunks that can be treated at the same time.
    /// </summary>
    public static int chunkUpdateRoutineWorkers = 4;

    /// <summary>
    /// Maximum distance after which chunks are not rendered anymore around the player position. Do not mistake this for the generationDistance;
    /// The render distance is only what chunks are visually rendered, while the generationDistance is what chunks are being generated (populate
    /// block map, first cache update).
    /// </summary>
    public static int renderDistance = 10;

    /// <summary>
    /// Maximum distance after which chunks do not generate around the player position.
    /// </summary>
    public static int generationDistance = 4;
}