using UnityEngine;
using UnityEngine.InputSystem;

namespace Assets.Scripts.Player
{
    public class PlayerInput : MonoBehaviour
    {
        #region Input State

        /// <summary>
        /// Current movement input (WASD / left stick).
        /// </summary>
        [HideInInspector] public Vector2 moveInput;

        /// <summary>
        /// True on the frame the player presses jump.
        /// </summary>
        [HideInInspector] public bool jump;

        /// <summary>
        /// True while the player holds sprint.
        /// </summary>
        [HideInInspector] public bool sprint;

        /// <summary>
        /// True on the frame the player presses attack.
        /// </summary>
        [HideInInspector] public bool attack;

        #endregion

        #region Private

        private Keyboard keyboard;

        #endregion

        #region Unity Callbacks

        private void Start()
        {
            keyboard = Keyboard.current;
        }

        private void Update()
        {
            keyboard = Keyboard.current;
            Gamepad gamepad = Gamepad.current;

            ReadMovement(keyboard, gamepad);
            ReadActions(keyboard, gamepad);
        }

        #endregion

        #region Input Reading

        /// <summary>
        /// Reads directional movement from keyboard and gamepad.
        /// </summary>
        private void ReadMovement(Keyboard kb, Gamepad gp)
        {
            float horizontal = 0f;
            float vertical = 0f;

            if (kb != null)
            {
                if (kb.wKey.isPressed || kb.upArrowKey.isPressed) vertical++;
                if (kb.sKey.isPressed || kb.downArrowKey.isPressed) vertical--;
                if (kb.dKey.isPressed || kb.rightArrowKey.isPressed) horizontal++;
                if (kb.aKey.isPressed || kb.leftArrowKey.isPressed) horizontal--;
            }

            if (gp != null)
            {
                Vector2 leftStick = gp.leftStick.ReadValue();
                horizontal += leftStick.x;
                vertical += leftStick.y;
            }

            moveInput = new Vector2(horizontal, vertical);
            if (moveInput.magnitude > 1f)
            {
                moveInput.Normalize();
            }
        }

        /// <summary>
        /// Reads action inputs (jump, sprint, attack) from keyboard and gamepad.
        /// </summary>
        private void ReadActions(Keyboard kb, Gamepad gp)
        {
            jump = (kb?.spaceKey.wasPressedThisFrame is true) ||
                   (gp?.buttonSouth.wasPressedThisFrame is true);

            sprint = (kb?.leftShiftKey.isPressed is true) ||
                     (gp?.rightTrigger.isPressed is true);

            attack = (kb?.qKey.wasPressedThisFrame is true) ||
                     (gp?.buttonWest.wasPressedThisFrame is true);
        }

        #endregion
    }
}