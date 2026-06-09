using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
    public class CombatController : MonoBehaviour
    {
        public enum LimbType { HandL, HandR, LegL, LegR }

        [Header("Hitbox References")]
        [SerializeField] private GameObject HandLeft;
        [SerializeField] private GameObject HandRight;
        [SerializeField] private GameObject LegLeft;
        [SerializeField] private GameObject LegRight;

        private Dictionary<LimbType, GameObject> hitboxMap;

        private void InitializeHitboxMap()
        {
            if (hitboxMap != null) return;
            hitboxMap = new Dictionary<LimbType, GameObject>
            {
                { LimbType.HandL, HandLeft },
                { LimbType.HandR, HandRight },
                { LimbType.LegL, LegLeft },
                { LimbType.LegR, LegRight }
            };
        }

        private void Awake()
        {
            InitializeHitboxMap();
            foreach (var hitbox in hitboxMap.Values)
            {
                if (hitbox == null) continue;
                hitbox.SetActive(false);
                var hb = hitbox.GetComponent<Hitbox>() ?? hitbox.AddComponent<Hitbox>();
                hb.Owner = this;
            }
        }

        public void SetHitboxState(LimbType limb, bool isActive)
        {
            InitializeHitboxMap();
            if (hitboxMap.TryGetValue(limb, out GameObject target) && target != null)
                target.SetActive(isActive);
        }

        /// <summary>
        /// Routes a hit to the active character controller on this GameObject.
        /// </summary>
        public void TakeHit(CombatController attacker)
        {
            foreach (var character in GetComponents<CharacterBase>())
            {
                if (character.enabled)
                {
                    character.GetHit();
                    return;
                }
            }
        }
    }
}