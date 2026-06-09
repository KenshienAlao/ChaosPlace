using UnityEngine;
namespace Assets.Settings
{
    [System.Serializable]
    public class CameraSettings
    {
        public Vector3 offset = new(0f, 1.5f, -3f);
        [Range(0, 10)]
        public float smoothTime = 1f;
        [Range(0, 10)]
        public float rotationSmoothSpeed = 10f;
        [HideInInspector] public Vector3 currentVelocity = Vector3.zero;
    }
}