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
            Random r = new();
            return r.Next(101) < percent;
        }

        public override BlockEntry GetBlockAtPosition(Vector3Int position, BlockMap map)
        {
            int surface = GetSurfaceHeightFromGlobalPosition(position);

            if (position.y > surface)
                return map.GetEntryByName("air");

            if (position.y == surface && Chance(45))
                return map.GetEntryByName("gravel");

            return Chance(35) ? map.GetEntryByName("andesite") : map.GetEntryByName("stone");
        }
    }
}