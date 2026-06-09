using Assets.Scripts.Player;
using Assets.Settings;
using UnityEngine;

namespace Assets.Scripts
{
    public class CameraController : MonoBehaviour
    {
        #region References

        private PlayerInput playerInput;
        private Transform target;

        #endregion

        #region Inspector Settings

        /// <summary>
        /// Camera behavior settings (offset, smoothing, rotation).
        /// </summary>
        public CameraSettings cameraSettings;

        #endregion

        #region Unity Callbacks

        /// <summary>
        /// Finds the player and caches required references.
        /// </summary>
        private void Start()
        {
            if (target == null)
            {
                GameObject player = GameObject.FindGameObjectWithTag("Player");
                if (player != null)
                {
                    target = player.transform;
                }
                else
                {
                    Debug.LogError("Player is not found or put the Tag \"Player\" in the player.");
                    return;
                }
            }

            if (target != null)
            {
                playerInput = target.GetComponent<PlayerInput>();
            }
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
            if (playerInput == null)
            {
                TryResolvePlayerInput();
            }

            if (playerInput == null) return Vector3.zero;
            if (playerInput.moveInput.sqrMagnitude <= 0.01f) return Vector3.zero;

            Transform cam = Camera.main.transform;
            Vector3 forward = new Vector3(cam.forward.x, 0f, cam.forward.z).normalized;
            Vector3 right = new Vector3(cam.right.x, 0f, cam.right.z).normalized;

            return ((forward * playerInput.moveInput.y) + (right * playerInput.moveInput.x)).normalized;
        }

        #endregion

        #region Helpers

        /// <summary>
        /// Attempts to find and cache the PlayerInput component from the player.
        /// </summary>
        private void TryResolvePlayerInput()
        {
            if (target != null)
            {
                playerInput = target.GetComponent<PlayerInput>();
                return;
            }

            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                target = player.transform;
                playerInput = target.GetComponent<PlayerInput>();
            }
        }

        #endregion
    }
}