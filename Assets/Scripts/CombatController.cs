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
        [Header("Hitbox References")]
        [SerializeField] private GameObject HandLeft;
        [SerializeField] private GameObject HandRight;
        [SerializeField] private GameObject LegLeft;
        [SerializeField] private GameObject LegRight;
        #endregion

        #region References
        /// <summary>
        /// Dictionary to store hitbox references.
        /// </summary>
        private Dictionary<LimbType, GameObject> hitboxMap;
        #endregion

        #region Unity Callbacks
        /// <summary>
        /// Cache hitboxes for fast access.
        /// </summary>
        private void Awake()
        {
            hitboxMap = new Dictionary<LimbType, GameObject>
            {
                { LimbType.HandL, HandLeft },
                { LimbType.HandR, HandRight },
                { LimbType.LegL, LegLeft },
                { LimbType.LegR, LegRight }
            };

            foreach (var hitbox in hitboxMap.Values)
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