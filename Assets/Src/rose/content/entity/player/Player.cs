using com.rose.content.world.generation;
using UnityEngine;

namespace com.rose.content.world.entity.player
{
    public class Player : MonoBehaviour
    {
        public static Player Instance { get; private set; }

        public PlayerInput input;
        public PlayerInteractivity interactivity;
        public PlayerGui gui;
        public Camera playerCamera;

        private void Awake()
        {
            Instance = this;
        }

        public Vector3Int GetGlobalPosition()
        {
            return Vector3Int.RoundToInt(transform.position);
        }

        public Vector3Int GetChunkCoordinate()
        {
            return WorldGenerationEngine.GetChunkCoordinateFromGlobalPosition(GetGlobalPosition());
        }
    }
}