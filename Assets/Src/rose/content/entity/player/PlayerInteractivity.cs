using com.rose.content.world.content.block;
using com.rose.content.world.generation;
using UnityEngine;

namespace com.rose.content.world.entity.player
{
    public class PlayerInteractivity : MonoBehaviour
    {
        public Player player;

        public int checkCount;
        public float checkInterval;

        private void Awake()
        {
            player.input.onAttack += OnAttack;
        }

        private void OnDrawGizmosSelected()
        {
            float nextCheckLength = 0;
            for (int i = 0; i < checkCount; i++)
            {
                nextCheckLength += checkInterval;
                Vector3 pos = transform.position + player.camera.transform.forward * nextCheckLength;
                Gizmos.DrawLine(transform.position, pos);
                Gizmos.DrawSphere(pos, 0.05F);
            }
        }

        private void OnAttack()
        {
            float nextCheckLength = 0;
            for (int i = 0; i < checkCount; i++)
            {
                nextCheckLength += checkInterval;
                Vector3 position = transform.position + player.camera.transform.forward * nextCheckLength;
                Vector3Int positionRounded = Vector3Int.RoundToInt(position);

                BlockEntry blockAtPosition = WorldGenerationEngine.Instance.GetBlockState(positionRounded).entry;
                if (blockAtPosition.name == "air")
                    continue;

                WorldGenerationEngine.Instance.RegisterWorldChange(positionRounded, WorldGenerationEngine.Instance.blocks.GetEntryByName("air").GetDefaultBlockState());
                break;
            }
        }
    }
}