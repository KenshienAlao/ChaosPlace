using Assets.Scripts.States;
using UnityEngine;

namespace Assets.Scripts.Npc
{
    [RequireComponent(typeof(CharacterController))]
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(CombatController))]

    public class NpcController : CharacterBase
    {
        private Transform playerTarget;

        [Header("Attack Settings")]
        [Range(0, 99)] public float attackRange = 2f;

        private void Start()
        {
            var playerObj = GameObject.FindWithTag("Player");
            if (playerObj != null) playerTarget = playerObj.transform;
        }

        private void Update()
        {
            if (playerTarget == null) return;
            TryFinishAction();
            HandleAI();
            ProcessAttackFrame("jab1", CombatController.LimbType.HandR, 0.566f, 0.70f);
        }

        private void HandleAI()
        {
            float distance = Vector3.Distance(transform.position, playerTarget.position);
            Vector3 direction = (playerTarget.position - transform.position).normalized;
            direction.y = 0;

            if (CanMove)
            {
                // Face the player
                if (direction != Vector3.zero)
                    transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), 10f * Time.deltaTime);

                // TODO: Attack when in range
                if (distance <= attackRange)
                {
                    // PerformAction(ActionState.Attacking, "jab1");
                }
                // TODO: Chase when out of range
                else
                {
                    // velocity.x = direction.x * speed;
                    // velocity.z = direction.z * speed;
                    // PerformAction(ActionState.Walking, "walk");
                }
            }
            else
            {
                velocity.x = 0;
                velocity.z = 0;
            }

            ApplyGravity();
        }
    }
}