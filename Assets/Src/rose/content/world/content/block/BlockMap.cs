using UnityEngine;

namespace com.rose.content.world.content.block
{
    [CreateAssetMenu(fileName = "New Block Map", menuName = "world/content/new block map", order = 0)]
    public class BlockMap : ScriptableObject
    {
        public BlockEntry[] entries;

        public BlockEntry GetEntryByName(string name)
        {
            foreach (var entry in entries)
                if (entry.name == name)
                    return entry;

            return null;
        }
    }
}