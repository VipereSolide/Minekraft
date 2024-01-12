using com.rose.content.world.generation;
using com.rose.fundation;
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
            player.input.onUse += OnUse;
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

        private void OnUse()
        {
            CameraUtility.Raycast(transform.position, player.camera.transform.forward, checkCount, checkInterval, (hitResult) =>
            {
                Vector3Int dir = Vector3Int.RoundToInt(hitResult.direction);
                Debug.Log(dir);
                Vector3Int positionRounded = Vector3Int.RoundToInt(hitResult.position) + dir;
                WorldGenerationEngine.Instance.RegisterWorldChange(positionRounded, WorldGenerationEngine.Instance.blocks.GetEntryByName("stone").GetDefaultBlockState());
            });
        }

        private void OnAttack()
        {
            CameraUtility.Raycast(transform.position, player.camera.transform.forward, checkCount, checkInterval, (hitResult) =>
            {
                Vector3Int positionRounded = Vector3Int.RoundToInt(hitResult.position);
                WorldGenerationEngine.Instance.RegisterWorldChange(positionRounded, WorldGenerationEngine.Instance.blocks.GetEntryByName("air").GetDefaultBlockState());
            });
        }

    }
}