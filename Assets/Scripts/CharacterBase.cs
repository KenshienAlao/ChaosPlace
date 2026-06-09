using Assets.Scripts.States;
using UnityEngine;

namespace Assets.Scripts
{
    /// <summary>
    /// Shared base for all characters (Player, NPC).
    /// </summary>
    public abstract class CharacterBase : MonoBehaviour
    {
        #region Shared Components

        protected CharacterController characterController;
        protected Animator animator;
        protected CombatController combatController;

        #endregion

        #region State

        public ActionState currentActionState = ActionState.Normal;
        protected string currentAnimation = "";
        protected Vector3 velocity;

        public bool CanMove => currentActionState switch
        {
            ActionState.Attacking or ActionState.Grabbing or ActionState.Stunned => false,
            _ => true
        };

        #endregion

        #region Inspector

        [Header("Movement")]
        [Range(0, 99)] public float speed = 2f;
        [Range(1, 5)] public float sprintMultiplier = 2f;

        [Header("Gravity")]
        [Range(-99, 99)] public float gravity = -10f;

        #endregion

        #region Unity Callbacks

        protected virtual void Awake()
        {
            characterController = GetComponent<CharacterController>();
            animator = GetComponent<Animator>();
            combatController = GetComponent<CombatController>();
        }

        #endregion

        #region State Management

        /// <summary>
        /// Checks if the current action animation has finished and returns to idle.
        /// Call this at the start of each Update.
        /// </summary>
        protected bool TryFinishAction()
        {
            if (CanMove) return false;

            var state = animator.GetCurrentAnimatorStateInfo(0);
            if (state.IsName(currentAnimation) && state.normalizedTime >= 1.0f && !animator.IsInTransition(0))
            {
                currentActionState = ActionState.Normal;
                ResetCurrentAnimation();
                PlayAnimation("idle");
                return true;
            }
            return false;
        }

        /// <summary>
        /// Shortcut to enter an action state and play its animation.
        /// Usage: PerformAction(ActionState.Attacking, "jab1");
        /// </summary>
        protected void PerformAction(ActionState state, string animation)
        {
            currentActionState = state;
            PlayAnimation(animation);
        }

        #endregion

        #region Combat

        /// <summary>
        /// Activates a limb hitbox during the active frames of an attack animation.
        /// </summary>
        protected void ProcessAttackFrame(string attackAnim, CombatController.LimbType limb, float startWindow, float endWindow)
        {
            if (currentActionState != ActionState.Attacking) return;

            var state = animator.GetCurrentAnimatorStateInfo(0);
            if (state.IsName(attackAnim))
            {
                float progress = state.normalizedTime;
                combatController.SetHitboxState(limb, progress >= startWindow && progress < endWindow);
            }
        }

        public void GetHit()
        {
            currentActionState = ActionState.Stunned;
            ResetCurrentAnimation();
            PlayAnimation("hit");
        }

        // Future: public void GetGrabbed() { PerformAction(ActionState.Grabbing, "grabbed"); }
        // Future: public void GetStunned() { PerformAction(ActionState.Stunned, "stunned"); }

        #endregion

        #region Physics

        /// <summary>
        /// Applies gravity and moves the character controller. Call at the end of each frame.
        /// </summary>
        protected void ApplyGravity()
        {
            if (characterController.isGrounded && velocity.y < 0) velocity.y = -2f;
            velocity.y += gravity * Time.deltaTime;
            characterController.Move(velocity * Time.deltaTime);
        }

        #endregion

        #region Animation

        public void PlayAnimation(string newAnimation, float crossfade = 0.1f)
        {
            if (currentAnimation == newAnimation) return;
            animator.CrossFadeInFixedTime(newAnimation, crossfade);
            currentAnimation = newAnimation;
        }

        public void ForcePlayAnimation(string newAnimation, float crossfade = 0.05f)
        {
            animator.CrossFadeInFixedTime(newAnimation, crossfade, 0, 0f);
            currentAnimation = newAnimation;
        }

        public void ResetCurrentAnimation() => currentAnimation = "";

        #endregion
    }
}
