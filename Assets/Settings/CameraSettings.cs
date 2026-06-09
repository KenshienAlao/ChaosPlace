using UnityEngine;
namespace Assets.Settings
{
    [System.Serializable]
    public class CameraSettings
    {
        /// <summary>
        /// Target to apply to the camera's position relative to the target.
        /// </summary>
        [Range(0, 10)]
        public Vector3 offset = new(0f, 1.5f, -3f);

        /// <summary>
        /// Smoothest speed to apply to the camera's position relative to the target.
        /// </summary>
        [Range(0, 10)]
        public float smoothTime = 1f;

        /// <summary>
        /// Smoothest speed to apply to the camera's rotation relative to the target.
        /// </summary>
        [Range(0, 10)]
        public float rotationSmoothSpeed = 10f;

        /// <summary>
        /// Current velocity of the camera.
        /// </summary>
        [HideInInspector] public Vector3 currentVelocity = Vector3.zero;
    }
}