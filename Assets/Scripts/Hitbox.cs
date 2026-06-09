using UnityEngine;

namespace Assets.Scripts
{
    public class Hitbox : MonoBehaviour
    {
        [HideInInspector] public CombatController Owner;

        private void OnTriggerEnter(Collider other)
        {
            CombatController target = other.GetComponentInParent<CombatController>();

            if (target != null && target != Owner)
            {
                target.TakeHit(Owner);
            }
        }
    }
}
