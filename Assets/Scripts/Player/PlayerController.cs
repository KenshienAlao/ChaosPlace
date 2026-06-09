using Assets.Scripts.Npc;
using Assets.Scripts.States;
using UnityEngine;

namespace Assets.Scripts.Player
{
    [RequireComponent(typeof(CharacterController))]
    [RequireComponent(typeof(PlayerInput))]
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(CombatController))]
    [RequireComponent(typeof(CharacterIdentityController))]
    [RequireComponent(typeof(NpcController))]

    public class PlayerController : CharacterBase
    {
        private PlayerInput playerInput;
        private CameraController cameraController;

        protected override void Awake()
        {
            base.Awake();
            playerInput = GetComponent<PlayerInput>();
            cameraController = Camera.main.GetComponent<CameraController>();
        }

        private void Update()
        {
            TryFinishAction();
            HandleInput();
            HandleLocomotion();
            ProcessAttackFrame("jab1", CombatController.LimbType.HandL, 0.45f, 0.60f);
            HandleCameraRotation();
        }

        private void HandleInput()
        {
            if (CanMove && characterController.isGrounded && playerInput.attack && !playerInput.sprint)
                PerformAction(ActionState.Attacking, "jab1");
        }

        private void HandleLocomotion()
        {
            float currentSpeed = playerInput.sprint ? speed * sprintMultiplier : speed;
            Vector3 input = cameraController.GetCameraRelativeInput();

            if (CanMove)
            {
                if (!characterController.isGrounded)
                    PerformAction(ActionState.Jumping, "jump");
                else if (input == Vector3.zero)
                    PerformAction(ActionState.Normal, "idle");
                else if (playerInput.sprint)
                    PerformAction(ActionState.Running, "run");
                else
                    PerformAction(ActionState.Walking, "walk");

                velocity.x = currentSpeed * input.x;
                velocity.z = currentSpeed * input.z;

                if (playerInput.jump && characterController.isGrounded)
                    velocity.y = Mathf.Sqrt(currentSpeed * -2f * gravity);
            }
            else
            {
                velocity.x = 0f;
                velocity.z = 0f;
            }

            ApplyGravity();
        }

        private void HandleCameraRotation()
        {
            if (Camera.main != null) cameraController = Camera.main.GetComponent<CameraController>();
            if (cameraController == null) return;

            Vector3 input = cameraController.GetCameraRelativeInput();
            if (input != Vector3.zero)
            {
                Quaternion target = Quaternion.LookRotation(input);
                transform.rotation = Quaternion.Slerp(transform.rotation, target, speed * Time.deltaTime);
            }
        }
    }
}