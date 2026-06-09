using Assets.Scripts.States;
using UnityEngine;

namespace Assets.Scripts.Player
{
    public class PlayerController : MonoBehaviour
    {
        #region Components

        private CharacterController characterController;
        private PlayerInput playerInput;
        private CameraController cameraController;
        private Animator animator;

        #endregion

        #region State

        public ActionState currentActionState = ActionState.Normal;
        private string currentAnimation = "";
        private Vector3 velocity;
        private float currentSpeed;

        #endregion

        #region Inspector Settings

        [Header("Movement Settings")]
        [Range(0, 99)]
        public float speed;

        [Header("Sprint Settings")]
        [Range(1, 5)]
        public float sprintMultiplier = 2f;

        [Header("Gravity")]
        [Range(-99, 99)]
        public float gravity = -10f;

        #endregion

        #region Unity Callbacks

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
        private void Update()
        {
            Controls();
            CameraRotate();
        }

        #endregion

        #region Controls

        /// <summary>
        /// Handles player movement, actions (like attacks), and locomotion animations.
        /// </summary>
        private void Controls()
        {
            // Check if the current action has completed
            if (!CanMove)
            {
                AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
                if (stateInfo.IsName(currentAnimation) && stateInfo.normalizedTime >= 1.0f && !animator.IsInTransition(0))
                {
                    currentActionState = ActionState.Normal;
                }
            }

            // Process Action Inputs
            if (CanMove && characterController.isGrounded)
            {
                if (playerInput.attack && !playerInput.sprint)
                {
                    currentActionState = ActionState.Attacking;
                    PlayAnimation("jab1");
                }
            }

            // Process Locomotion Animations
            if (CanMove)
            {
                Vector3 inputDirection = cameraController.GetCameraRelativeInput();

                if (!characterController.isGrounded)
                {
                    currentActionState = ActionState.Jumping;
                    PlayAnimation("jump");
                }
                else if (inputDirection == Vector3.zero)
                {
                    currentActionState = ActionState.Normal;
                    PlayAnimation("idle");
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

            // Calculate Physics & Movement
            currentSpeed = playerInput.sprint ? speed * sprintMultiplier : speed;
            Vector3 movementInput = cameraController.GetCameraRelativeInput();

            if (CanMove)
            {
                velocity.x = currentSpeed * movementInput.x;
                velocity.z = currentSpeed * movementInput.z;
            }
            else
            {
                velocity.x = 0f;
                velocity.z = 0f;
            }

            // Reset downward velocity if grounded
            if (characterController.isGrounded && velocity.y < 0)
            {
                velocity.y = -2f;
            }

            // Jump
            if (playerInput.jump && characterController.isGrounded && CanMove)
            {
                velocity.y = Mathf.Sqrt(currentSpeed * -2f * gravity);
            }

            // Gravity
            velocity.y += gravity * Time.deltaTime;

            // Move character controller
            characterController.Move(velocity * Time.deltaTime);
        }

        /// <summary>
        /// Rotates the player towards the movement direction.
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

            if (inputDirection != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(inputDirection);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, speed * Time.deltaTime);
            }
        }

        #endregion

        #region Helpers

        /// <summary>
        /// Whether the player is allowed to move or take new inputs.
        /// </summary>
        private bool CanMove => currentActionState switch
        {
            ActionState.Attacking or ActionState.Grabbing or ActionState.Stunned => false,
            _ => true
        };

        #endregion

        #region Animation

        /// <summary>
        /// Plays an animation with crossfade. Skips if the same animation is already playing.
        /// </summary>
        public void PlayAnimation(string newAnimation, float crossfade = 0.1f)
        {
            if (currentAnimation == newAnimation) return;

            animator.CrossFadeInFixedTime(newAnimation, crossfade);
            currentAnimation = newAnimation;
        }

        /// <summary>
        /// Forces an animation to play from the start, even if it's already playing.
        /// </summary>
        public void ForcePlayAnimation(string newAnimation, float crossfade = 0.05f)
        {
            animator.CrossFadeInFixedTime(newAnimation, crossfade, 0, 0f);
            currentAnimation = newAnimation;
        }

        /// <summary>
        /// Resets the current animation tracker, allowing any animation to be played next.
        /// </summary>
        public void ResetCurrentAnimation()
        {
            currentAnimation = "";
        }

        #endregion
    }
}