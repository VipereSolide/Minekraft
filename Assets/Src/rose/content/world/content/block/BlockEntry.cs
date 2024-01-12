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

        [Header("Textures")]
        public Material main;

        public Material GetModifiedMaterial(Material material)
        {
            return main == null ? material : main;
        }

        public BlockState GetDefaultBlockState()
        {
            return new(this);
        }
    }
}