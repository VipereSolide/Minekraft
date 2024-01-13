using com.rose.content.world.content.block;
using com.rose.fundation.extensions;
using UnityEngine;
using Random = System.Random;

namespace com.rose.content.world.generation
{
    [System.Serializable]
    public class StandardMapGenerator : AbstractMapGenerator
    {
        public NoiseSettings[] noiseSettings;
        public int maxHeight;

        [Header("Palette")]
        public BlockEntry surfaceBlock;
        public BlockEntry subsurfaceBlock;
        public BlockEntry[] stoneBlocks;

        private Random random;

        private float MinimumOfAllNoises(Vector3Int pos)
        {
            float min = 9999999999;
            foreach (var n in noiseSettings)
            {
                var v = n.Get().GetNoise(pos.x, pos.z);
                if (v < min)
                    min = v;
            }
            return min;
        }

        private float AddedOfAllNoises(Vector3Int pos)
        {
            float r = 0;
            foreach (var n in noiseSettings)
                r += n.Get().GetNoise(pos.x, pos.z);
            return r;
        }

        public int GetSurfaceHeightFromGlobalPosition(Vector3Int position)
        {
            float noise = AddedOfAllNoises(position);

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

            BlockEntry surfaceBlock = this.surfaceBlock;
            BlockEntry subsurfaceBlock = this.subsurfaceBlock;

            if (position.y >= 120 + random.Next(-10, 10))
            {
                surfaceBlock = map.GetEntryByName("snow");
                subsurfaceBlock = map.GetEntryByName("snow");
            }

            if (position.y == surface)
                return map.GetEntry(surfaceBlock);

            if (position.y == surface - 1 || position.y == surface - 2 && Chance(37))
                return map.GetEntry(subsurfaceBlock);

            return map.GetEntry(stoneBlocks[random.Next(stoneBlocks.Length)]);
        }
    }
}