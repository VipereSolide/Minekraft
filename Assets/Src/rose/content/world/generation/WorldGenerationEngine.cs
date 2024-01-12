using com.rose.content.world.content.block;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

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
        public Vector3Int mapSize = new(1, 4, 1);
        public BlockMap blocks;

        [Space]
        public Mesh faceMesh;
        public Material faceBaseMaterial;

        [Header("Runtime Data")]
        public StandardMapGenerator generator = new();
        public Chunk[,,] chunks;

        private void Awake()
        {
            SetSingleton();
            Initialize();
        }

        private void Update()
        {
            Render();
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = new Color(10, 50, 255);
            foreach (var chunk in chunks)
            {
                Vector3Int pos = new Vector3Int(chunk.Coordinate.x * WorldData.chunkSize.x, chunk.Coordinate.y * WorldData.chunkSize.y, chunk.Coordinate.z * WorldData.chunkSize.z) + (WorldData.chunkSize / 2);
                Gizmos.DrawWireCube(pos, WorldData.chunkSize);
            }
        }

        private void Initialize()
        {
            void InitializeChunks()
            {
                chunks = new Chunk[mapSize.x, mapSize.y, mapSize.z];

                for (int mapX = 0; mapX < mapSize.x; mapX++)
                {
                    for (int mapZ = 0; mapZ < mapSize.z; mapZ++)
                    {
                        for (int mapY = 0; mapY < mapSize.y; mapY++)
                        {
                            chunks[mapX, mapY, mapZ] = new Chunk();
                            var chunk = chunks[mapX, mapY, mapZ];

                            chunk.Initialize(this, new Vector3Int(mapX, mapY, mapZ));
                        }
                    }
                }
            }

            InitializeChunks();
        }

        protected virtual void Render()
        {
            foreach (var chunk in chunks)
            {
                foreach (var key in chunk.GetRenderData().blocks)
                {
                    Graphics.DrawMeshInstanced(faceMesh, 0, key.blockEntry.GetModifiedMaterial(faceBaseMaterial), key.voxels.ToArray());
                }
            }
        }

        public void RegisterWorldChange(Vector3Int globalPosition, BlockState newState, bool updateFlag = true)
        {
            Chunk chunk = GetChunkFromGlobalPosition(globalPosition);
            Vector3Int localPosition = GetChunkLocalPositionFromGlobalPosition(globalPosition);
            chunk.SetBlockState(localPosition, newState, updateFlag);
        }

        public Chunk[] AllChunks()
        {
            List<Chunk> chunks = new List<Chunk>();
            foreach (var chunk in this.chunks)
                chunks.Add(chunk);

            return chunks.ToArray();
        }

        public Vector3Int GetChunkCoordinateFromGlobalPosition(Vector3Int position)
        {
            return new Vector3Int(Mathf.RoundToInt(position.x / WorldData.chunkSize.x), Mathf.RoundToInt(position.y / WorldData.chunkSize.y), Mathf.RoundToInt(position.z / WorldData.chunkSize.z));
        }

        public Vector3Int GetChunkGlobalPosition(Vector3Int chunkGlobalPosition)
        {
            return new Vector3Int(chunkGlobalPosition.x * WorldData.chunkSize.x, chunkGlobalPosition.y * WorldData.chunkSize.y, chunkGlobalPosition.z * WorldData.chunkSize.z);
        }

        public Vector3Int GetChunkLocalPositionFromGlobalPosition(Vector3Int position)
        {
            Vector3Int chunkGlobalPosition = GetChunkGlobalPosition(GetChunkCoordinateFromGlobalPosition(position));
            Vector3Int relativeCoordinates = new(position.x - chunkGlobalPosition.x, position.y - chunkGlobalPosition.y, position.z - chunkGlobalPosition.z);
            return relativeCoordinates;
        }

        public BlockEntry GetNaturalBlockAtPosition(Vector3Int globalPosition)
        {
            //if (modifiedBlockEntries.ContainsKey(position))
            //    return modifiedBlockEntries[position];

            return generator.GetBlockAtPosition(globalPosition, blocks);
        }

        public Chunk GetChunkFromCoordinate(Vector3Int chunkCoordinate)
        {
            foreach (var chunk in chunks)
                if (chunk.Coordinate == chunkCoordinate)
                    return chunk;

            return null;
        }

        public Chunk GetChunkFromGlobalPosition(Vector3Int globalPosition)
        {
            return GetChunkFromCoordinate(GetChunkCoordinateFromGlobalPosition(globalPosition));
        }

        public BlockState GetBlockState(Vector3Int globalPosition)
        {
            Vector3Int localPosition = GetChunkLocalPositionFromGlobalPosition(globalPosition);
            if (localPosition.x < 0 || localPosition.y < 0 || localPosition.z < 0)
            {
                return null;
            }

            Chunk chunk = GetChunkFromGlobalPosition(globalPosition);

            if (chunk == null)
                return null;

            return chunk.GetBlockState(localPosition);
        }
    }
}