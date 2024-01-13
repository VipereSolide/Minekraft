using UnityEngine;

namespace com.rose.content.world.content.block
{
    [CreateAssetMenu(fileName = "New Block Entry", menuName = "world/content/new block entry")]
    public class BlockEntry : ScriptableObject
    {
        public new string name;

        [Tooltip("Whether the block is seethrough or not. Checking this will allow neighbour blocks to show their faces against this " +
            "block's faces.")]
        public bool isTransparent;

        [Tooltip("Whether the block occludes faces with other transparent blocks.")]
        public bool occludeFacesWithSameTypeNeighbours;

        [Header("Textures")]
        public bool useMainMaterial = true;
        public Material main;

        [Space]

        public Material[] textures;

        public Material GetModifiedMaterial(Material material, byte face)
        {
            return useMainMaterial ? (main == null ? material : main) : textures[face];
        }

        public BlockState GetDefaultBlockState()
        {
            return new(this);
        }
    }
}