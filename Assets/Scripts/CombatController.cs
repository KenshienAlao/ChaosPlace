using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
    public class CombatController : MonoBehaviour
    {
        #region Enums
        /// <summary>
        /// Type of limb to enable.
        /// </summary>
        public enum LimbType { HandL, HandR, LegL, LegR }
        #endregion

        #region Inspector
        /// <summary>
        /// Array to store hitbox references.
        /// </summary>
        [Header("Animation References")]
        [SerializeField] private GameObject[] hitboxes = new GameObject[4];
        #endregion

        #region References
        /// <summary>
        /// Dictionary to store hitbox references.
        /// </summary>
        private Dictionary<LimbType, GameObject> hitboxMap = new Dictionary<LimbType, GameObject>();
        #endregion

        #region Unity Callbacks
        /// <summary>
        /// Cache hitboxes for fast access.
        /// </summary>
        private void Awake()
        {
            /// <summary>
            /// Cache hitboxes for fast access.
            /// </summary>
            hitboxMap.Add(LimbType.HandL, hitboxes[0]);
            hitboxMap.Add(LimbType.HandR, hitboxes[1]);
            hitboxMap.Add(LimbType.LegL, hitboxes[2]);
            hitboxMap.Add(LimbType.LegR, hitboxes[3]);

            foreach (var hitbox in hitboxes)
            {
                if (hitbox != null) hitbox.SetActive(false);
            }
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Call this to toggle any hitbox instantly.
        /// Example: SetHitboxState(LimbType.HandR, true);
        /// </summary>
        public void SetHitboxState(LimbType limb, bool isActive)
        {
            if (hitboxMap.TryGetValue(limb, out GameObject targetHitbox))
            {
                if (targetHitbox != null)
                {
                    targetHitbox.SetActive(isActive);
                }
            }
        }
        #endregion
    }
}