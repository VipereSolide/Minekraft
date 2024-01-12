using com.rose.content.world.content.block;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using static WorldData;

namespace com.rose.content.world.generation
{
    [Serializable]
    public class Chunk
    {
        [SerializeField]
        private WorldGenerationEngine world;

        [SerializeField]
        private Vector3Int coordinate;

        public BlockState[,,] blockstates;

        public BlockRenderData cache;

        public bool shouldUpdate;

        public Vector3Int Coordinate
        {
            get { return coordinate; }
        }

        public void Initialize(WorldGenerationEngine world, Vector3Int coordinate)
        {
            this.world = world;
            this.coordinate = coordinate;

            PopulateBlockStates();
        }

        public void PopulateBlockStates()
        {
            blockstates = new BlockState[chunkSize.x, chunkSize.y, chunkSize.z];

            for (int x = 0; x < chunkSize.x; x++)
            {
                for (int z = 0; z < chunkSize.z; z++)
                {
                    for (int y = 0; y < chunkSize.y; y++)
                    {
                        Vector3Int localPosition = new(x, y, z);
                        blockstates[x, y, z] = world.GetNaturalBlockAtPosition(GetGlobalPositionFromLocalPosition(localPosition)).GetDefaultBlockState();
                    }
                }
            }
        }

        public BlockRenderData GetRenderData()
        {
            void Update()
            {
                cache = new();

                for (int x = 0; x < chunkSize.x; x++)
                {
                    for (int z = 0; z < chunkSize.z; z++)
                    {
                        for (int y = 0; y < chunkSize.y; y++)
                        {
                            Vector3Int pos = new(x, y, z);
                            cache.Add(GetRenderDataAtPosition(pos));
                        }
                    }
                }
            }

            if (shouldUpdate || cache == null)
            {
                if (cache == null)
                {
                    Debug.Log("Updating chunk asynchrounously...");
                    Parallel.Invoke(Update);
                }
                else
                {
                    Debug.Log("Updating chunk synchrounously...");
                    Update();
                }

                shouldUpdate = false;
            }

            return cache;
        }

        /// <param name="localPosition">Position of the block inside the chunk (I.e. 1, 8, 31)</param>
        protected virtual Tuple<BlockEntry, HashSet<Matrix4x4>> GetRenderDataAtPosition(Vector3Int localPosition)
        {
            // The rendered position's coordinates in global space instead of chunk space.
            Vector3Int globalPosition = GetGlobalPositionFromLocalPosition(localPosition);
            BlockEntry blockAtPosition = GetBlockState(localPosition).entry;

            if (blockAtPosition == null || blockAtPosition.name == "air")
                return null;

            bool[] facesVisibleState = GetVisibleFacesAtPosition(localPosition);
            HashSet<Matrix4x4> renderedFaces = new(facesVisibleState.Length);

            for (int i = 0; i < facesVisibleState.Length; i++)
                if (facesVisibleState[i])
                    renderedFaces.Add(GetRenderedFace(globalPosition, i));

            return new Tuple<BlockEntry, HashSet<Matrix4x4>>(blockAtPosition, renderedFaces);
        }

        public Matrix4x4 GetRenderedFace(Vector3Int globalPosition, int faceIndex)
        {
            Matrix4x4 renderedFace = new();
            Vector3 offsetFromNeighbourIndex = GetOffsetFromNeighbourIndex(faceIndex);
            Vector3 position = globalPosition + offsetFromNeighbourIndex / 2;
            Quaternion rotation = Quaternion.LookRotation(-GetDirectionFromNeighbourIndex(faceIndex));
            renderedFace.SetTRS(position, rotation, Vector3.one);

            return renderedFace;
        }

        public Vector3Int GetChunkGlobalCoordinate()
        {
            return world.GetChunkGlobalPosition(coordinate);
        }

        /// <summary>
        /// Calculates the world position of a block of this chunk from a local position within this chunk.
        /// </summary>
        /// <param name="localPosition">The position of the block whos global position you seek.</param>
        /// <returns>A Vector3Int containing the global position of a local position of this chunk.</returns>
        public Vector3Int GetGlobalPositionFromLocalPosition(Vector3Int localPosition)
        {
            return GetChunkGlobalCoordinate() + localPosition;
        }

        /// <summary>
        /// Queries every neighbouring blocks of the given global position.
        /// </summary>
        /// <returns>
        /// A BlockState Array of size 6 containing the neighbouring blocks of the given global position.
        /// The order of the neighbours is as following: x, mX, y, mY, z, mZ.
        /// </returns>
        public BlockState[] GetBlockNeighbours(Vector3Int globalPosition)
        {
            return new BlockState[6]
            {
            world.GetBlockState(globalPosition + GetOffsetFromNeighbourIndex(0)),
            world.GetBlockState(globalPosition + GetOffsetFromNeighbourIndex(1)),
            world.GetBlockState(globalPosition + GetOffsetFromNeighbourIndex(2)),
            world.GetBlockState(globalPosition + GetOffsetFromNeighbourIndex(3)),
            world.GetBlockState(globalPosition + GetOffsetFromNeighbourIndex(4)),
            world.GetBlockState(globalPosition + GetOffsetFromNeighbourIndex(5)),
            };
        }

        public bool[] GetVisibleFacesWithNeighbours(BlockState[] neighbours)
        {
            bool[] result = new bool[neighbours.Length];

            for (int i = 0; i < neighbours.Length; i++)
            {
                if (neighbours[i] == null)
                    result[i] = false;
                else
                    result[i] = neighbours[i].entry.isTransparent;
            }

            return result;
        }

        /// <summary>
        /// Returns all visible faces from a local position.
        /// </summary>
        /// <param name="localPosition">The position in the chunk.</param>
        /// <returns>A Boolean array of size 6 containing the visible state of every faces at this position.</returns>
        public bool[] GetVisibleFacesAtPosition(Vector3Int localPosition)
        {
            Vector3Int globalPosition = GetGlobalPositionFromLocalPosition(localPosition);
            var neighbours = GetBlockNeighbours(globalPosition);
            return GetVisibleFacesWithNeighbours(neighbours);
        }

        public Vector3 GetDirectionFromNeighbourIndex(int neighbourIndex)
        {
            return neighbourIndex switch
            {
                0 => Vector3.right,
                1 => -Vector3.right,
                2 => Vector3.up,
                3 => -Vector3.up,
                4 => Vector3.forward,
                5 => -Vector3.forward,
                _ => Vector3.right,
            };
        }

        public Vector3Int GetOffsetFromNeighbourIndex(int neighbourIndex)
        {
            return neighbourIndex switch
            {
                0 => Vector3Int.right,
                1 => -Vector3Int.right,
                2 => Vector3Int.up,
                3 => -Vector3Int.up,
                4 => Vector3Int.forward,
                5 => -Vector3Int.forward,
                _ => Vector3Int.right,
            };
        }

        /// <summary>
        /// Gets a blockstate in this chunk using a local position vector3 int instead of 3 coordinates in an array.
        /// </summary>
        /// <param name="localPosition">The position of the blockstate in the chunk.</param>
        /// <returns>A BlockState that is inside the chunk at the given local position.</returns>
        public BlockState GetBlockState(Vector3Int localPosition)
        {
            return blockstates[localPosition.x, localPosition.y, localPosition.z];
        }

        /// <summary>
        /// Checks whether the given local position is on any border with any chunk.
        /// </summary>
        /// <param name="localPosition">What position you wish to check.</param>
        /// <param name="borderedChunks">All the chunks the local position is bordering with.</param>
        /// <returns>True if the position is bordering with any chunk, and false otherwise.</returns>
        public bool IsLocalPositionBordering(Vector3Int localPosition, out HashSet<Chunk> borderedChunks)
        {
            borderedChunks = new HashSet<Chunk>();

            if (localPosition.x == 0)
            {
                Chunk toAdd = world.GetChunkFromCoordinate(coordinate - Vector3Int.right);
                if (toAdd != null)
                    borderedChunks.Add(toAdd);
            }

            if (localPosition.x == chunkSize.x - 1)
            {
                Chunk toAdd = world.GetChunkFromCoordinate(coordinate + Vector3Int.right);
                if (toAdd != null)
                    borderedChunks.Add(toAdd);
            }

            if (localPosition.y == 0)
            {
                Chunk toAdd = world.GetChunkFromCoordinate(coordinate - Vector3Int.up);
                if (toAdd != null)
                    borderedChunks.Add(toAdd);
            }

            if (localPosition.y == chunkSize.y - 1)
            {
                Chunk toAdd = world.GetChunkFromCoordinate(coordinate + Vector3Int.up);
                if (toAdd != null)
                    borderedChunks.Add(toAdd);
            }

            if (localPosition.z == 0)
            {
                Chunk toAdd = world.GetChunkFromCoordinate(coordinate - Vector3Int.forward);
                if (toAdd != null)
                    borderedChunks.Add(toAdd);
            }

            if (localPosition.z == chunkSize.z - 1)
            {
                Chunk toAdd = world.GetChunkFromCoordinate(coordinate + Vector3Int.forward);
                if (toAdd != null)
                    borderedChunks.Add(toAdd);
            }

            return borderedChunks.Count > 0;
        }

        public void SetBlockState(Vector3Int localPosition, BlockState newState, bool updateFlag = true)
        {
            blockstates[localPosition.x, localPosition.y, localPosition.z] = newState;

            if (updateFlag)
            {
                shouldUpdate = true;

                // If the block broken is one that's bordering another chunk, we also need to update that other chunk.
                // Otherwise we'll have a blank face that will go away only when updating the bordered chunk.

                if (IsLocalPositionBordering(localPosition, out HashSet<Chunk> borderedChunks))
                {
                    foreach (var chunk in borderedChunks)
                    {
                        chunk.shouldUpdate = true;
                    }
                }
            }
        }
    }
}