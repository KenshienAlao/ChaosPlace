using Assets.Scripts.States;
using UnityEngine;

namespace Assets.Scripts.Player
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

        public ActionState currentActionState = ActionState.Normal;

        private string currentAnimation = "";

        private Vector3 velocity;

        // temp
        [Header("Movement Settings")]
        [Range(0, 99)]
        public float speed;

        [Header("Sprint Settings")]
        [Range(1, 5)]
        public float sprintMultiplier = 2f;

        [Header("Gravity")]
        [Range(-99, 99)]
        public float gravity = -10f;

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
            if (IsPlayingAction()) return;

            Vector3 inputDirection = cameraController.GetCameraRelativeInput();

            // Play animations
            if (inputDirection == Vector3.zero)
            {
                currentActionState = ActionState.Normal;
                PlayAnimation("idle");
            }
            else if (!characterController.isGrounded)
            {
                currentActionState = ActionState.Jumping;
                PlayAnimation("jump");
            }
            else if (playerInput.sprint)
            {
                currentActionState = ActionState.Running;
                PlayAnimation("run");
            }
            else
            {
                currentActionState = ActionState.Walking;
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
            // ==== SPRINT ====
            currentSpeed = playerInput.sprint ? speed * sprintMultiplier : speed;

            Vector3 inputDirection = cameraController.GetCameraRelativeInput();

            // Set horizontal velocity
            if (CanMove)
            {
                velocity.x = currentSpeed * inputDirection.x; // left or right
                velocity.z = currentSpeed * inputDirection.z; // forward or backward
            }
            else
            {
                velocity.x = 0f;
                velocity.z = 0f;
            }

            // Reset downward velocity if grounded
            if (characterController.isGrounded && velocity.y < 0)
            {
                velocity.y = -2f; // downward force
            }

            // ==== JUMP ====
            if (playerInput.jump && characterController.isGrounded && CanMove)
            {
                velocity.y = Mathf.Sqrt(currentSpeed * -2f * gravity);
            }

            // ==== ATTACK ====
            // attack while on ground and not running
            if (playerInput.attack && characterController.isGrounded && !playerInput.sprint && CanMove)
            {
                currentActionState = ActionState.Attacking;
                PlayAnimation("jab1");
            }

            // Apply gravity
            velocity.y += gravity * Time.deltaTime; // up or down

            // Move the character
            characterController.Move(velocity * Time.deltaTime);
        }



        // ======================================
        // HELPERS
        // ======================================

        /// <summary>
        /// Gets whether the player is currently allowed to move or take new inputs.
        /// </summary>
        private bool CanMove => currentActionState switch
        {
            ActionState.Attacking or ActionState.Grabbing or ActionState.Stunned => false,
            _ => true
        };

        /// <summary>
        /// Checks if the player is performing an action that shouldn't be interrupted.
        /// </summary>
        private bool IsPlayingAction()
        {
            if (CanMove) return false;

            AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
            bool isPlaying = stateInfo.IsName(currentAnimation) && stateInfo.normalizedTime < 1.0f;
            bool isTransitioning = animator.IsInTransition(0) && animator.GetNextAnimatorStateInfo(0).IsName(currentAnimation);

            if (isPlaying || isTransitioning) return true;

            currentActionState = ActionState.Normal;
            return false;
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