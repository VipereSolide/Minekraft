using com.rose.content.world.content.block;
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

        [Space]

        public float blocksPlacedPerMinute;
        public float blocksBrokenPerMinute;

        [Header("Resources")]
        public Player player;

        [Space]

        public Mesh previewingBlockMesh;
        public Material previewingDestroyBlockMaterial;
        public Material previewingPlaceBlockMaterial;

        [Header("Runtime Data")]
        public bool isHitting;
        public BlockHitResult currentHitResult;

        private float nextTimeToAttack;
        private float nextTimeToUse;

        private void Awake()
        {
            player.input.onAttack += OnAttack;
            player.input.onUse += OnUse;

            player.input.onAttack += () => nextTimeToAttack = Time.time + 60F / blocksBrokenPerMinute;
            player.input.onUse += () => nextTimeToUse = Time.time + 60F / blocksPlacedPerMinute;
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

            if (player.input.isUseHeld && Time.time > nextTimeToUse)
            {
                nextTimeToUse = Time.time + 60F / blocksPlacedPerMinute;
                OnUse();
            }

            if (player.input.isAttackHeld && Time.time > nextTimeToAttack)
            {
                nextTimeToAttack = Time.time + 60F / blocksBrokenPerMinute;
                OnAttack();
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
                BlockEntry entry = player.gui.hotbar.GetSelectedSlot().content;
                if (entry == null)
                    return;

                Vector3Int positionRounded = currentHitResult.GetRoundedPosition() + currentHitResult.GetRoundedDirection();
                WorldGenerationEngine.Instance.RegisterWorldChange(positionRounded, entry.GetDefaultBlockState());
            }
        }

        private void OnAttack()
        {
            if (isHitting)
                WorldGenerationEngine.Instance.RegisterWorldChange(currentHitResult.GetRoundedPosition(), WorldGenerationEngine.Instance.blocks.GetEntryByName("air").GetDefaultBlockState());
        }

    }
}