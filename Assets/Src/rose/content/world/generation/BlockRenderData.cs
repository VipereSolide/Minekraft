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
            public HashSet<Matrix4x4> voxels;

            public BlockKey(BlockEntry blockEntry, HashSet<Matrix4x4> voxels)
            {
                this.blockEntry = blockEntry;
                this.voxels = voxels;
            }

            public BlockKey(Tuple<BlockEntry, HashSet<Matrix4x4>> tuple)
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

        public void Add(Tuple<BlockEntry, HashSet<Matrix4x4>> data)
        {
            if (data == null)
                return;

            if (blocks.Contains(data.Item1))
                blocks[data.Item1].voxels.AddRange(data.Item2);
            else
                blocks.Add(new(data));
        }

        public void Add(Tuple<BlockEntry, Matrix4x4[]> data)
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