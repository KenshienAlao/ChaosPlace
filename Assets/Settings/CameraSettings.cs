using UnityEngine;

namespace Assets.Settings
{
    [System.Serializable]
    public class CameraSettings
    {
        #region Position

        /// <summary>
        /// Offset applied to the camera's position relative to the target.
        /// </summary>
        [Range(0, 10)]
        public Vector3 offset = new(0f, 1.5f, -3f);

        /// <summary>
        /// How smoothly the camera follows the target's position.
        /// </summary>
        [Range(0, 10)]
        public float smoothTime = 1f;

        #endregion

        #region Rotation

        /// <summary>
        /// How smoothly the camera rotates to look at the target.
        /// </summary>
        [Range(0, 10)]
        public float rotationSmoothSpeed = 10f;

        #endregion

        #region Runtime

        /// <summary>
        /// Internal velocity used by SmoothDamp. Do not modify.
        /// </summary>
        [HideInInspector] public Vector3 currentVelocity = Vector3.zero;

        #endregion
    }
}