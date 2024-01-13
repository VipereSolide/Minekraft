using System;
using UnityEngine;

namespace com.rose.content.world.entity.player
{
    public class PlayerInput : MonoBehaviour
    {
        [Header("Interact")]
        public KeyCode attack = KeyCode.Mouse0;
        public KeyCode use = KeyCode.Mouse1;

        [Header("Movement")]
        public KeyCode moveRight = KeyCode.D;
        public KeyCode moveLeft = KeyCode.Q;
        public KeyCode moveForward = KeyCode.Z;
        public KeyCode moveBackward = KeyCode.S;

        [Space]
        [SerializeField] private float movementX;
        [SerializeField] private float movementZ;

        public Action onAttack;
        public Action onUse;

        public bool isAttackHeld;
        public bool isUseHeld;

        public Vector2 Movement
        {
            get { return new(movementX, movementZ); }
        }

        private void Update()
        {
            void Movement()
            {
                if (Input.GetKey(moveRight))
                    movementX = 1F;
                if (Input.GetKey(moveLeft))
                    movementX = -1F;
                if (Input.GetKey(moveRight) && Input.GetKey(moveLeft))
                    movementX = 0F;
                if (!Input.GetKey(moveRight) && !Input.GetKey(moveLeft))
                    movementX = 0F;

                if (Input.GetKey(moveForward))
                    movementZ = 1F;
                if (Input.GetKey(moveBackward))
                    movementZ = -1F;
                if (Input.GetKey(moveForward) && Input.GetKey(moveBackward))
                    movementZ = 0F;
                if (!Input.GetKey(moveForward) && !Input.GetKey(moveBackward))
                    movementZ = 0F;
            }
            void Interact()
            {
                if (Input.GetKeyDown(attack))
                    onAttack?.Invoke();
                if (Input.GetKeyDown(use))
                    onUse?.Invoke();

                isAttackHeld = Input.GetKey(attack);
                isUseHeld = Input.GetKey(use);
            }

            Movement();
            Interact();
        }
    }
}