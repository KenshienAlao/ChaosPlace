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
            Movements();
            CameraRotate();
            Controls();
        }

        #endregion

        #region Movement & Camera

        /// <summary>
        /// Handles the player's movement animations.
        /// </summary>
        private void Movements()
        {
            if (IsPlayingAction()) return;

            Vector3 inputDirection = cameraController.GetCameraRelativeInput();

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

        #region Controls

        /// <summary>
        /// Handles the player's input controls (sprint, jump, attack) and physics.
        /// </summary>
        private void Controls()
        {
            currentSpeed = playerInput.sprint ? speed * sprintMultiplier : speed;

            Vector3 inputDirection = cameraController.GetCameraRelativeInput();

            // Horizontal velocity
            if (CanMove)
            {
                velocity.x = currentSpeed * inputDirection.x;
                velocity.z = currentSpeed * inputDirection.z;
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

            // Attack
            if (playerInput.attack && characterController.isGrounded && !playerInput.sprint && CanMove)
            {
                currentActionState = ActionState.Attacking;
                PlayAnimation("jab1");
            }

            // Gravity
            velocity.y += gravity * Time.deltaTime;

            // Move
            characterController.Move(velocity * Time.deltaTime);
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

        /// <summary>
        /// Returns true if the player is performing an action that should not be interrupted.
        /// Automatically resets the state when the animation finishes.
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

        #endregion

        #region Animation

        /// <summary>
        /// Plays an animation with crossfade. Skips if the same animation is already playing.
        /// </summary>
        public void PlayAnimation(string newAnimation, float crossfade = 0.1f)
        {
            if (currentAnimation == newAnimation) return;

            animator.CrossFade(newAnimation, crossfade);
            currentAnimation = newAnimation;
        }

        /// <summary>
        /// Forces an animation to play from the start, even if it's already playing.
        /// </summary>
        public void ForcePlayAnimation(string newAnimation, float crossfade = 0.05f)
        {
            animator.CrossFade(newAnimation, crossfade, 0, 0f);
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