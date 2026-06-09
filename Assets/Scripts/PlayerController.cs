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

        private Animator animator;

        private string currentAnimation = "";

        // temp
        [Header("Movement Settings")]
        [Range(0, 99)]
        public float speed;

        [Header("Sprint Settings")]
        [Range(1, 5)]
        public float sprintMultiplier = 2f;

        private float currentSpeed;

        /// <summary>
        /// Gets the required components for the player controller.
        /// </summary>
        private void Awake()
        {
            characterController = GetComponent<CharacterController>();
            playerInput = GetComponent<PlayerInput>();
            cameraController = Camera.main.GetComponent<CameraController>();
            animator = GetComponent<Animator>();

            if (characterController == null) Debug.LogError("CharacterController component not found on this GameObject.");
            if (playerInput == null) Debug.LogError("PlayerInput component not found on this GameObject.");
            if (cameraController == null) Debug.LogError("CameraController component not found on Main Camera.");
            if (animator == null) Debug.LogError("Animator component not found on this GameObject.");
        }
        private void Start()
        {
        }

        /// <summary>
        /// Handles the player controller every frame.
        /// </summary>
        void Update()
        {
            Movements();
            CameraRotate();
            Controls();
        }

        /// <summary>
        /// Handles the player's movements.
        /// </summary>
        private void Movements()
        {
            Vector3 inputDirection = cameraController.GetCameraRelativeInput();
            Vector3 moveVelocity = currentSpeed * inputDirection;
            characterController.Move(moveVelocity * Time.deltaTime);

            if (inputDirection == Vector3.zero)
            {
                PlayAnimation("idle");
            }
            else if (playerInput.sprint)
            {
                PlayAnimation("run");
            }
            else
            {
                PlayAnimation("walk");
            }
        }

        /// <summary>
        /// Handles the camera.
        /// </summary>
        private void CameraRotate()
        {
            Vector3 inputDirection = cameraController.GetCameraRelativeInput();

            if (Camera.main != null) cameraController = Camera.main.GetComponent<CameraController>();
            if (cameraController == null)
            {
                Debug.LogError("Camera controller is not found.");
                return;
            }

            // rotate towards movement direction
            if (inputDirection != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(inputDirection);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, speed * Time.deltaTime);
            }
        }

        /// <summary>
        /// Handles the player's controls.
        /// </summary>
        private void Controls()
        {
            // sprint
            currentSpeed = playerInput.sprint ? speed * sprintMultiplier : speed;
        }

        public void PlayAnimation(string newAnimation, float crossfade = 0.1f)
        {
            if (currentAnimation == newAnimation) return;

            animator.CrossFade(newAnimation, crossfade);
            currentAnimation = newAnimation;
        }

        public void ForcePlayAnimation(string newAnimation, float crossfade = 0.05f)
        {
            animator.CrossFade(newAnimation, crossfade, 0, 0f);
            currentAnimation = newAnimation;
        }

        public void ResetCurrentAnimation()
        {
            currentAnimation = "";
        }
    }
}