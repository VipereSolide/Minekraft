using com.rose.fundation;
using System;
using UnityEngine;

namespace com.rose.content.world.generation
{
    [Serializable]
    public class ChunkUpdateRoutine : UpdateRoutine
    {
        public ChunkUpdateRoutine(int capacity) : base(capacity)
        {

        }

        protected override void DoOnChunk(Chunk chunk, int index)
        {
            chunk.UpdateRenderDataCache();
        }

        protected override void FinishUpdatingChunkAtIndex(int index)
        {
            base.FinishUpdatingChunkAtIndex(index);

            Debug.Log($"ChunkUpdateRoutine: Worker {index} done ({waitingList.Count} left).");
        }
    }
}