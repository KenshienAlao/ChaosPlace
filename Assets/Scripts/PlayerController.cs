using UnityEngine;

namespace Assets.Scripts
{
    public class PlayerController : MonoBehaviour
    {
        /// <summary>
        /// Components that are required for the player controller.
        /// </summary>
        private CharacterController characterController;
        private PlayerInput playerInput;
        private CameraController cameraController;

        /// <summary>
        /// Gets the required components for the player controller.
        /// </summary>
        private void Awake()
        {
            characterController = GetComponent<CharacterController>();
            playerInput = GetComponent<PlayerInput>();
            cameraController = GetComponent<CameraController>();
        }
        private void Start()
        {
        }

        void Update()
        {

        }
    }
}