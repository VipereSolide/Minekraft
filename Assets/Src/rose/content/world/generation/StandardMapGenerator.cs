using com.rose.content.world.content.block;
using com.rose.fundation.extensions;
using UnityEngine;
using Random = System.Random;

namespace com.rose.content.world.generation
{
    [System.Serializable]
    public class StandardMapGenerator : AbstractMapGenerator
    {
        public NoiseSettings noiseSettings;
        public int maxHeight;

        [Header("Palette")]
        public BlockEntry surfaceBlock;
        public BlockEntry subsurfaceBlock;
        public BlockEntry[] stoneBlocks;

        private Random random;

        public int GetSurfaceHeightFromGlobalPosition(Vector3Int position)
        {
            float noise = noiseSettings.Get().GetNoise(position.x, position.z);
            noise = noise.Remap(-1, 1, 0, 1);
            return Mathf.RoundToInt(noise * maxHeight);
        }

        public int GetSurfaceHeightFromGlobalPosition(int x, int z)
        {
            return GetSurfaceHeightFromGlobalPosition(new Vector3Int(x, 0, z));
        }

        private bool Chance(float percent)
        {
            return random.Next(101) < percent;
        }

        public override BlockEntry GetBlockAtPosition(Vector3Int position, BlockMap map)
        {
            random = new();

            int surface = GetSurfaceHeightFromGlobalPosition(position);

            if (position.y > surface)
                return map.GetEntryByName("air");

            BlockEntry _surface = surfaceBlock;
            if (position.y >= 60)
                _surface = map.GetEntryByName("snow");

            if (position.y == surface)
                return map.GetEntry(_surface);

            if (position.y == surface - 1 || position.y == surface - 2 && Chance(37))
                return map.GetEntry(subsurfaceBlock);

            return map.GetEntry(stoneBlocks[random.Next(stoneBlocks.Length)]);
        }
    }
}