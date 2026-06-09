using Assets.Scripts.Npc;
using Assets.Scripts.Player;
using UnityEngine;

namespace Assets.Scripts
{
    public class CharacterIdentityController : MonoBehaviour
    {
        #region Unity Callbacks

        /// <summary>
        /// Checks if the character is player or npc and enables the corresponding controller.
        /// </summary>
        private void Awake()
        {
            if (CompareTag("Player"))
            {
                // Disable npc controller if it exists
                if (TryGetComponent<NpcController>(out var npc)) npc.enabled = false;

                // Enable player controller if it doesn't exist
                if (GetComponent<PlayerController>() == null) gameObject.AddComponent<PlayerController>();

                Debug.Log($"{gameObject.name} is player");
            }
            else if (CompareTag("Npc"))
            {
                // Disable player controller if it exists
                if (TryGetComponent<PlayerController>(out var player)) player.enabled = false;

                // Enable npc controller if it doesn't exist
                if (GetComponent<NpcController>() == null) gameObject.AddComponent<NpcController>();

                Debug.Log($"{gameObject.name} is npc");
            }
        }

        #endregion
    }
}