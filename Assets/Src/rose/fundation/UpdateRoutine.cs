using com.rose.content.world.generation;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace com.rose.fundation
{
    public abstract class UpdateRoutine
    {
        public Queue<Chunk> waitingList;
        public Chunk[] updatingChunks;
        public readonly int capacity;

        public UpdateRoutine(int capacity)
        {
            this.capacity = capacity;
            updatingChunks = new Chunk[capacity];
            waitingList = new();
        }

        public virtual bool AreAllWorkersFree()
        {
            foreach (var worker in updatingChunks)
                if (worker != null)
                    return false;

            return true;
        }

        public virtual bool IsFree()
        {
            return waitingList.Count == 0 && AreAllWorkersFree();
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

        protected abstract void DoOnChunk(Chunk chunk, int index);

        protected virtual async void SetUpdatingChunkAtIndex(Chunk chunk, int index)
        {
            updatingChunks[index] = chunk;

            await Task.Run(() =>
            {
                DoOnChunk(chunk, index);
                FinishUpdatingChunkAtIndex(index);
            });
        }

        protected virtual void FinishUpdatingChunkAtIndex(int index)
        {
            updatingChunks[index] = null;
        }
    }
}