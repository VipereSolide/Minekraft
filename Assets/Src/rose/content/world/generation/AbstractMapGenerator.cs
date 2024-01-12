using com.rose.content.world.content.block;
using UnityEngine;

namespace com.rose.content.world.generation
{
    [System.Serializable]
    public abstract class AbstractMapGenerator
    {
        /// <summary>
        /// Determines what block will be at a given position.
        /// </summary>
        /// <param name="position">The position of the block that is to be determined.</param>
        /// <param name="map">The list of generable blocks to pick from.</param>
        /// <returns>A BlockEntry containing information about the determined block.</returns>
        public abstract BlockEntry GetBlockAtPosition(Vector3Int position, BlockMap map);
    }
}