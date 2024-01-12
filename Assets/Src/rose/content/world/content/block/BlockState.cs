using System;

namespace com.rose.content.world.content.block
{
    [Serializable]
    public class BlockState
    {
        public BlockEntry entry;

        public BlockState() { }

        public BlockState(BlockEntry entry)
        {
            this.entry = entry;
        }
    }
}