using com.rose.content.world.generation;
using com.rose.fundation;
using UnityEngine;

namespace com.rose.content.world.entity.player
{
    public class PlayerInteractivity : MonoBehaviour
    {
        [Header("Settings")]
        public int checkCount;
        public float checkInterval;

        [Header("Resources")]
        public Player player;

        [Space]

        public Mesh previewingBlockMesh;
        public Material previewingDestroyBlockMaterial;
        public Material previewingPlaceBlockMaterial;

        [Header("Runtime Data")]
        public bool isHitting;
        public BlockHitResult currentHitResult;

        private void Awake()
        {
            player.input.onAttack += OnAttack;
            player.input.onUse += OnUse;
        }

        private void Update()
        {
            isHitting = CameraUtility.Raycast(transform.position, player.playerCamera.transform.forward, checkCount, checkInterval, (hitResult) =>
            {
                currentHitResult = hitResult;
            });

            if (isHitting)
            {
                Matrix4x4 matrice = new();

                if (WorldData.activateBlockPlaceIndicator)
                {
                    matrice.SetTRS(currentHitResult.GetRoundedPosition() + currentHitResult.GetRoundedDirection(), Quaternion.identity, Vector3.one);
                    Graphics.DrawMesh(previewingBlockMesh, matrice, previewingPlaceBlockMaterial, 0);
                }

                matrice.SetTRS(currentHitResult.GetRoundedPosition(), Quaternion.identity, Vector3.one);
                Graphics.DrawMesh(previewingBlockMesh, matrice, previewingDestroyBlockMaterial, 0);
            }
        }

        private void OnDrawGizmosSelected()
        {
            float nextCheckLength = 0;
            for (int i = 0; i < checkCount; i++)
            {
                nextCheckLength += checkInterval;
                Vector3 pos = transform.position + player.playerCamera.transform.forward * nextCheckLength;
                Gizmos.DrawLine(transform.position, pos);
                Gizmos.DrawSphere(pos, 0.05F);
            }
        }

        private void OnUse()
        {
            if (isHitting)
            {
                Vector3Int positionRounded = currentHitResult.GetRoundedPosition() + currentHitResult.GetRoundedDirection();
                WorldGenerationEngine.Instance.RegisterWorldChange(positionRounded, WorldGenerationEngine.Instance.blocks.GetEntryByName("glass").GetDefaultBlockState());
            }
        }

        private void OnAttack()
        {
            if (isHitting)
                WorldGenerationEngine.Instance.RegisterWorldChange(currentHitResult.GetRoundedPosition(), WorldGenerationEngine.Instance.blocks.GetEntryByName("air").GetDefaultBlockState());
        }

    }
}