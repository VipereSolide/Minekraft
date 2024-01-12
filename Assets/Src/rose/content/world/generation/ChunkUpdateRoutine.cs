using System.Collections.Generic;

namespace com.rose.content.world.generation
{
    public class ChunkUpdateRoutine
    {
        public Queue<Chunk> waitingList;
        public Chunk[] updatingChunks;
        public readonly int capacity;

        public ChunkUpdateRoutine(int capacity)
        {
            this.capacity = capacity;
            updatingChunks = new Chunk[capacity];
            waitingList = new();
        }

        public void UpdateWaitingList()
        {
            if (waitingList.Count > 0)
                for (int i = 0; i < capacity; i++)
                    if (updatingChunks[i] == null)
                        SetUpdatingChunkAtIndex(waitingList.Dequeue(), i);
        }

        public void RegisterChunkUpdate(Chunk chunk)
        {
            for (int i = 0; i < capacity; i++)
            {
                if (updatingChunks[i] == null)
                {
                    SetUpdatingChunkAtIndex(chunk, i);
                    return;
                }
            }

            waitingList.Enqueue(chunk);
        }

        protected virtual void SetUpdatingChunkAtIndex(Chunk chunk, int index)
        {
            updatingChunks[index] = chunk;
        }
    }
}