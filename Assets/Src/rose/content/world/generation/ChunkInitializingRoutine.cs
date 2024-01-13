using com.rose.fundation;
using UnityEngine;

namespace com.rose.content.world.generation
{
    public class ChunkInitializingRoutine : UpdateRoutine
    {
        private int targetCount;

        public ChunkInitializingRoutine(int capacity) : base(capacity)
        {
        }

        public void SetTargetCount(int targetCount)
        {
            this.targetCount = targetCount;
        }

        protected override void DoOnChunk(Chunk chunk, int index)
        {
            chunk.Initialize();
        }

        protected override void FinishUpdatingChunkAtIndex(int index)
        {
            base.FinishUpdatingChunkAtIndex(index);

            // Debug.Log($"Initializing routine...                                        Worker {index} done ({waitingList.Count} left).");
            Debug.Log($"Initializing routine...                                        {Mathf.RoundToInt(100 - (waitingList.Count * 100 / (float) targetCount))}%");
        }
    }
}