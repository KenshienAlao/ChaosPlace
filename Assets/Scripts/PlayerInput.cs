using UnityEngine;
using UnityEngine.InputSystem;

namespace Assets.Scripts
{
    public class PlayerInput : MonoBehaviour
    {
        /// <summary>
        /// Gets the movement input from the player.
        /// </summary>
        [HideInInspector] public Vector2 moveInput;

        /// <summary>
        /// Gets the jump input from the player.
        /// </summary>
        [HideInInspector] public bool jump;

        /// <summary>
        /// Gets the sprint input from the player.
        /// </summary>
        [HideInInspector] public bool sprint;

        /// <summary>
        /// Gets the attack input from the player.
        /// </summary>
        [HideInInspector] public bool attack;

        private Keyboard keyboard;

        void Start()
        {
            keyboard = Keyboard.current;
        }

        void Update()
        {
            keyboard = Keyboard.current;
            Gamepad gamepad = Gamepad.current;

            float horizontal = 0;
            float vertical = 0;

            if (keyboard != null)
            {
                if (keyboard.wKey.isPressed || keyboard.upArrowKey.isPressed) vertical++;
                if (keyboard.sKey.isPressed || keyboard.downArrowKey.isPressed) vertical--;
                if (keyboard.dKey.isPressed || keyboard.rightArrowKey.isPressed) horizontal++;
                if (keyboard.aKey.isPressed || keyboard.leftArrowKey.isPressed) horizontal--;
            }

            if (gamepad != null)
            {
                Vector2 leftStick = gamepad.leftStick.ReadValue();
                horizontal += leftStick.x;
                vertical += leftStick.y;
            }

            moveInput = new Vector2(horizontal, vertical);
            if (moveInput.magnitude > 1f)
            {
                moveInput.Normalize();
            }

            jump = (keyboard?.spaceKey.wasPressedThisFrame is true) ||
                          (gamepad?.buttonSouth.wasPressedThisFrame is true);
            sprint = (keyboard?.leftShiftKey.isPressed is true) ||
                           (gamepad?.rightTrigger.isPressed is true);

            attack = (keyboard?.qKey.wasPressedThisFrame is true) ||
                     (gamepad?.buttonWest.wasPressedThisFrame is true);
        }
    }
}