using com.rose.content.world.content.block;
using com.rose.content.world.generation;
using System;
using UnityEngine;

namespace com.rose.fundation
{
    public struct BlockHitResult
    {
        public Vector3 direction;
        public Vector3 position;

        public Vector3Int GetRoundedPosition()
        {
            return Vector3Int.RoundToInt(position);
        }

        public Vector3Int GetRoundedDirection()
        {
            return Vector3Int.RoundToInt(direction);
        }
    }

    public static class CameraUtility
    {
        public static bool Raycast(Vector3 origin, Vector3 direction, int checkCount, float checkInterval, Action<BlockHitResult> onHit)
        {
            float nextCheckLength = 0;
            for (int i = 0; i < checkCount; i++)
            {
                nextCheckLength += checkInterval;
                Vector3 position = origin + direction * nextCheckLength;
                Vector3Int positionRounded = Vector3Int.RoundToInt(position);

                BlockEntry blockAtPosition = WorldGenerationEngine.Instance.GetBlockState(positionRounded).entry;
                if (blockAtPosition.name == "air")
                    continue;

                onHit?.Invoke(new()
                {
                    direction = (position - positionRounded).normalized,
                    position = position
                });

                return true;
            }

            return false;
        }
    }
}