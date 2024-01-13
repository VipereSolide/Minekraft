using com.rose.content.world.content.block;
using com.rose.content.world.entity.player;
using com.rose.debugging.world.generation;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace com.rose.content.world.generation
{
    public class WorldGenerationEngine : MonoBehaviour
    {
        public static WorldGenerationEngine Instance
        {
            get; private set;
        }

        private void SetSingleton()
        {
            Instance = this;
        }

        [Header("Settings")]
        public bool randomSeed;
        public Vector3Int mapSize = new(1, 4, 1);
        public BlockMap blocks;

        [Space]
        public Player player;
        public Mesh faceMesh;
        public Material faceBaseMaterial;

        [Header("Runtime Data")]
        public StandardMapGenerator generator = new();
        public Chunk[,,] chunks;
        public ChunkUpdateRoutine updateRoutine;

        public Plane[] planes;

        private void Awake()
        {
            SetSingleton();
            WorldGenerationDebugger.WorldGenerationEngineAwakens();

            player.transform.position = (mapSize * WorldData.chunkSize) / 2;
            Initialize();
        }

        private void Update()
        {
            InitializeNearbyChunks();
            UpdateChunksRenderDataCache();
            Render();

            updateRoutine.UpdateWaitingList();
        }

        private void OnDrawGizmos()
        {
            if (!WorldGenerationDebugger.showChunkBorders)
                return;

            foreach (var chunk in GetNearbyChunks(player.GetGlobalPosition(), 3))
            {
                if (!ShouldChunkBeRendered(chunk))
                    continue;

                Gizmos.color = (chunk.Coordinate == player.GetChunkCoordinate()) ? new Color(1, 0.5F, 0.25F, 1) : Gizmos.color = new Color(1, 1, 1, 0.05F);
                Gizmos.DrawWireCube(chunk.GetBounds().center, chunk.GetBounds().size);
            }
        }

        private void Initialize()
        {
            if (randomSeed)
                generator.noiseSettings.seed = new System.Random().Next(int.MaxValue);

            updateRoutine = new(WorldData.chunkUpdateRoutineWorkers);
            chunks = new Chunk[mapSize.x, mapSize.y, mapSize.z];

            for (int mapX = 0; mapX < mapSize.x; mapX++)
                for (int mapZ = 0; mapZ < mapSize.z; mapZ++)
                    for (int mapY = 0; mapY < mapSize.y; mapY++)
                        chunks[mapX, mapY, mapZ] = new Chunk(this, new Vector3Int(mapX, mapY, mapZ));
        }

        protected virtual async void InitializeNearbyChunks()
        {
            foreach (var chunk in GetNearbyChunks())
            {
                if (!chunk.IsInitialized && ShouldChunkBeRendered(chunk))
                    await Task.Run(() => chunk.Initialize());
            }
        }

        public virtual bool ShouldChunkBeRendered(Chunk chunk)
        {
            //float distanceFromCameraVertical = Vector3.Distance(chunk.GetChunkGlobalCoordinate().WithX(0).WithZ(0), player.GetGlobalPosition().WithX(0).WithZ(0));
            //if ( || distanceFromCameraVertical > WorldData.verticalRenderDistance)
            //    return false;

            planes = GeometryUtility.CalculateFrustumPlanes(player.playerCamera);
            return GeometryUtility.TestPlanesAABB(planes, chunk.GetBounds());
        }

        protected virtual async void UpdateChunksRenderDataCache()
        {
            foreach (var chunk in GetNearbyChunks())
            {
                if (chunk.IsInitialized && ShouldChunkBeRendered(chunk))
                {
                    if (chunk.hasRenderedChunkOnce)
                    {
                        chunk.UpdateRenderDataCache();
                    }
                    else
                    {
                        //await Task.Run(() => chunk.UpdateRenderDataCache());
                        updateRoutine.RegisterChunkUpdate(chunk);
                    }
                }
            }
        }

        public HashSet<Chunk> GetNearbyChunks(Vector3Int globalPosition, int radius)
        {
            Vector3Int chunkPosition = GetChunkCoordinateFromGlobalPosition(globalPosition);
            int halfRadius = Mathf.RoundToInt((float) radius / 2);
            HashSet<Chunk> result = new();

            for (int x = -halfRadius; x < halfRadius; x++)
            {
                for (int y = -halfRadius; y < halfRadius; y++)
                {
                    for (int z = -halfRadius; z < halfRadius; z++)
                    {
                        Vector3Int currentCoordinate = chunkPosition + new Vector3Int(x, y, z);

                        // Respect lower bounds.
                        if (currentCoordinate.x >= 0 && currentCoordinate.y >= 0 && currentCoordinate.z >= 0)
                            // Respect higher bounds.
                            if (currentCoordinate.x < mapSize.x && currentCoordinate.y < mapSize.y && currentCoordinate.z < mapSize.z)
                                result.Add(chunks[currentCoordinate.x, currentCoordinate.y, currentCoordinate.z]);
                    }
                }
            }

            return result;
        }

        public HashSet<Chunk> GetNearbyChunks()
        {
            return GetNearbyChunks(player.GetGlobalPosition(), WorldData.horizontalRenderDistance);
        }

        protected virtual void Render()
        {
            foreach (var chunk in GetNearbyChunks())
            {
                if (chunk.IsInitialized && ShouldChunkBeRendered(chunk))
                {
                    if (chunk.cache == null)
                        continue;

                    foreach (var key in chunk.cache.blocks)
                    {
                        if (key == null)
                        {
                            Debug.LogWarning("CANNOT RENDER NULL KEY!");
                            continue;
                        }

                        if (key.voxels == null)
                        {
                            Debug.LogWarning("CANNOT RENDER NULL VOXELS!");
                            continue;
                        }

                        HashSet<Matrix4x4>[] faceData = key.GetVoxelData();
                        for (byte i = 0; i < faceData.Length; i++)
                        {
                            if (faceData[i] == null)
                                continue;

                            Graphics.DrawMeshInstanced(faceMesh, 0, key.blockEntry.GetModifiedMaterial(faceBaseMaterial, i), faceData[i].ToArray());
                        }
                    }
                }
            }
        }

        public void RegisterWorldChange(Vector3Int globalPosition, BlockState newState, bool updateFlag = true)
        {
            GetChunkFromGlobalPosition(globalPosition).SetBlockState(GetChunkLocalPositionFromGlobalPosition(globalPosition), newState, updateFlag);
        }

        public Chunk[] AllChunks()
        {
            List<Chunk> chunks = new();

            foreach (var chunk in this.chunks)
                chunks.Add(chunk);

            return chunks.ToArray();
        }

        public static Vector3Int GetChunkCoordinateFromGlobalPosition(Vector3Int position)
        {
            return new Vector3Int(Mathf.RoundToInt(position.x / WorldData.chunkSize.x), Mathf.RoundToInt(position.y / WorldData.chunkSize.y), Mathf.RoundToInt(position.z / WorldData.chunkSize.z));
        }

        public static Vector3Int GetChunkGlobalPosition(Vector3Int chunkGlobalPosition)
        {
            return new Vector3Int(chunkGlobalPosition.x * WorldData.chunkSize.x, chunkGlobalPosition.y * WorldData.chunkSize.y, chunkGlobalPosition.z * WorldData.chunkSize.z);
        }

        public static Vector3Int GetChunkLocalPositionFromGlobalPosition(Vector3Int position)
        {
            Vector3Int chunkGlobalPosition = GetChunkGlobalPosition(GetChunkCoordinateFromGlobalPosition(position));
            Vector3Int relativeCoordinates = new(position.x - chunkGlobalPosition.x, position.y - chunkGlobalPosition.y, position.z - chunkGlobalPosition.z);
            return relativeCoordinates;
        }

        public BlockEntry GetNaturalBlockAtPosition(Vector3Int globalPosition)
        {
            return generator.GetBlockAtPosition(globalPosition, blocks);
        }

        public Chunk GetChunkFromCoordinate(Vector3Int chunkCoordinate)
        {
            // Respect lower bounds.
            if (chunkCoordinate.x >= 0 && chunkCoordinate.y >= 0 && chunkCoordinate.z >= 0)
                // Respect higher bounds.
                if (chunkCoordinate.x < mapSize.x && chunkCoordinate.y < mapSize.y && chunkCoordinate.z < mapSize.z)
                    return chunks[chunkCoordinate.x, chunkCoordinate.y, chunkCoordinate.z];

            return null;
        }

        public Chunk GetChunkFromGlobalPosition(Vector3Int globalPosition)
        {
            return GetChunkFromCoordinate(GetChunkCoordinateFromGlobalPosition(globalPosition));
        }

        public BlockState GetBlockState(Vector3Int globalPosition)
        {
            Vector3Int localPosition = GetChunkLocalPositionFromGlobalPosition(globalPosition);
            // Respect lower bounds.
            if (localPosition.x < 0 || localPosition.y < 0 || localPosition.z < 0)
                return null;

            // Respect higher bounds.
            if (localPosition.x >= WorldData.chunkSize.x || localPosition.y >= WorldData.chunkSize.y || localPosition.z >= WorldData.chunkSize.z)
                return null;

            Chunk chunk = GetChunkFromGlobalPosition(globalPosition);

            if (chunk == null || !chunk.IsInitialized)
                return null;

            return chunk.GetBlockState(localPosition);
        }
    }
}