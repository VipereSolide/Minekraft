using UnityEngine;

namespace com.rose.content.world.entity.player
{
    public class SpectatorController : MonoBehaviour
    {
        public Player player;
        public float sensitivity;
        public float movementSpeed;
        public Camera camera;
        public Transform cameraContainer;

        private float mouseY = 0;

        private void Update()
        {
            void CameraRotation()
            {
                float x = Input.GetAxis("Mouse X") * sensitivity;
                float y = Input.GetAxis("Mouse Y") * sensitivity;

                mouseY -= y;
                mouseY = Mathf.Clamp(mouseY, -90, 90);
                cameraContainer.localEulerAngles += new Vector3(0, x, 0);
                cameraContainer.localEulerAngles = new Vector3(mouseY, cameraContainer.localEulerAngles.y, cameraContainer.localEulerAngles.z);
            }

            void CameraMovement()
            {
                transform.localPosition += camera.transform.forward * Time.deltaTime * movementSpeed * player.input.Movement.y;
                transform.localPosition += camera.transform.right * Time.deltaTime * movementSpeed * player.input.Movement.x;
            }

            CameraRotation();
            CameraMovement();
        }
    }
}