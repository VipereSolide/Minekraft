using com.rose.content.world.content.block;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Unity.VisualScripting;
using UnityEngine;

namespace com.rose.content.world.generation
{
    public class RenderedVoxelCollection : KeyedCollection<Vector3Int, RenderedVoxelCollection.VoxelKey>
    {
        public class VoxelKey
        {
            public Vector3Int globalPosition;
            public Matrix4x4[] rendered;

            public VoxelKey(Vector3Int globalPosition, Matrix4x4[] rendered)
            {
                this.globalPosition = globalPosition;
                this.rendered = rendered;
            }
        }

        protected override Vector3Int GetKeyForItem(VoxelKey item)
        {
            return item.globalPosition;
        }
    }

    public class RenderedBlockCollection : KeyedCollection<BlockEntry, RenderedBlockCollection.BlockKey>
    {
        public class BlockKey
        {
            public BlockEntry blockEntry;
            public HashSet<FaceData> voxels;

            /// <summary>
            /// Sorts every voxel into a 'Matrix For Face' array, where every item of the array corresponds to a face, and the corresponding HashMap contains every Matrices.
            /// </summary>
            /// <returns>
            /// A Array of HashSets of Matrices. The Matrices are the rendered data. The HashSets contain all the different faces and their render data. The array corresponds
            /// to the 6 faces.
            /// </returns>
            public HashSet<Matrix4x4>[] GetVoxelData()
            {
                HashSet<Matrix4x4>[] result = new HashSet<Matrix4x4>[6];

                foreach (var faceData in voxels)
                {
                    if (result[faceData.faceIndex] == null)
                        result[faceData.faceIndex] = new();

                    result[faceData.faceIndex].Add(faceData.face);
                }

                return result;
            }

            public BlockKey(BlockEntry blockEntry, HashSet<FaceData> voxels)
            {
                this.blockEntry = blockEntry;
                this.voxels = voxels;
            }

            public BlockKey(Tuple<BlockEntry, HashSet<FaceData>> tuple)
            {
                blockEntry = tuple.Item1;
                voxels = tuple.Item2;
            }
        }

        protected override BlockEntry GetKeyForItem(BlockKey item)
        {
            return item.blockEntry;
        }
    }

    [Serializable]
    public class BlockRenderData
    {
        public RenderedBlockCollection blocks = new();

        public void Add(Tuple<BlockEntry, HashSet<FaceData>> data)
        {
            if (data == null)
                return;

            if (blocks.Contains(data.Item1))
                blocks[data.Item1].voxels.AddRange(data.Item2);
            else
                blocks.Add(new(data));
        }

        public void Add(Tuple<BlockEntry, FaceData[]> data)
        {
            if (data == null)
                return;

            if (blocks.Contains(data.Item1))
                blocks[data.Item1].voxels.AddRange(data.Item2);
            else
                blocks.Add(new(data.Item1, data.Item2.ToHashSet()));
        }
    }
}