using UnityEngine;

namespace com.rose.content.world.generation
{
    [CreateAssetMenu(fileName = "New Noise Settings", menuName = "world/generation/noise settings")]
    public class NoiseSettings : ScriptableObject
    {
        public FastNoiseLite.NoiseType noiseType;
        public FastNoiseLite.RotationType3D rotationType3D;
        public int seed;
        public float frequency;

        [Space]
        public FastNoiseLite.FractalType fractalType;
        public int fractalOctaves;
        public float fractalLacunarity;
        public float fractalGain;
        public float fractalWeightedStrength;

        [Space]
        public FastNoiseLite.DomainWarpType domainWarpType;
        public FastNoiseLite.RotationType3D domainRotationType3D;
        public float domainWarpAmplitude;

        public FastNoiseLite Get()
        {
            FastNoiseLite n = new();
            n.SetNoiseType(noiseType);
            n.SetRotationType3D(rotationType3D);
            n.SetSeed(seed);
            n.SetFrequency(frequency);
            n.SetFractalType(fractalType);
            n.SetFractalOctaves(fractalOctaves);
            n.SetFractalLacunarity(fractalLacunarity);
            n.SetFractalGain(fractalGain);
            n.SetFractalWeightedStrength(fractalWeightedStrength);
            n.SetDomainWarpType(domainWarpType);
            n.SetDomainWarpAmp(domainWarpAmplitude);

            return n;
        }
    }
}