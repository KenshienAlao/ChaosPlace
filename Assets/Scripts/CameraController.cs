using Assets.Scripts.Player;
using Assets.Settings;
using UnityEngine;

namespace Assets.Scripts
{
    public class CameraController : MonoBehaviour
    {
        #region References
        private PlayerInput playerInput;
        #endregion

        #region Inspector Settings

        [Header("Target Tracking")]
        [SerializeField] private Transform target;

        /// <summary>
        /// Camera behavior settings (offset, smoothing, rotation).
        /// </summary>
        [Header("Settings")]
        public CameraSettings cameraSettings;

        #endregion

        #region Unity Callbacks

        /// <summary>
        /// Caches references from the dragged target.
        /// </summary>
        private void Start()
        {
            ResolveTargetReferences();
        }

        /// <summary>
        /// Follows and looks at the player after all Update calls.
        /// </summary>
        private void LateUpdate()
        {
            if (target == null) return;

            Vector3 desiredPosition = target.position + (target.rotation * cameraSettings.offset);
            transform.position = Vector3.SmoothDamp(transform.position, desiredPosition, ref cameraSettings.currentVelocity, cameraSettings.smoothTime);

            Vector3 lookTarget = target.position + (Vector3.up * (cameraSettings.offset.y * 0.5f));
            Vector3 lookDirection = lookTarget - transform.position;
            if (lookDirection != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(lookDirection);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, cameraSettings.rotationSmoothSpeed * Time.deltaTime);
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Returns the player's movement input relative to the camera's facing direction.
        /// </summary>
        public Vector3 GetCameraRelativeInput()
        {
            // If there's no target or no input setup, give back a zero direction safely
            if (playerInput == null) return Vector3.zero;
            if (playerInput.moveInput.sqrMagnitude <= 0.01f) return Vector3.zero;

            Transform cam = Camera.main.transform;
            Vector3 forward = new Vector3(cam.forward.x, 0f, cam.forward.z).normalized;
            Vector3 right = new Vector3(cam.right.x, 0f, cam.right.z).normalized;

            return ((forward * playerInput.moveInput.y) + (right * playerInput.moveInput.x)).normalized;
        }

        /// <summary>
        /// Public method allowing character selection / dynamic spawn managers 
        /// to hand a newly spawned character over to the camera instantly.
        /// </summary>
        public void SetNewTarget(Transform newTarget)
        {
            target = newTarget;
            ResolveTargetReferences();
        }

        #endregion

        #region Helpers

        /// <summary>
        /// Validates the target and safely extracts necessary components once.
        /// </summary>
        private void ResolveTargetReferences()
        {
            if (target != null)
            {
                playerInput = target.GetComponent<PlayerInput>();

                if (playerInput == null)
                {
                    Debug.LogWarning($"[{gameObject.name}] Assigned target '{target.name}' is missing a PlayerInput component!");
                }
            }
            else
            {
                Debug.LogError($"[{gameObject.name}] Target is missing! Please drag a character into the 'Target' inspector slot.", this);
            }
        }

        #endregion
    }
}