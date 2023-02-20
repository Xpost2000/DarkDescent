using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DarkDescent {
    internal static class Input {
        private static Microsoft.Xna.Framework.Input.KeyboardState m_last_key_state;
        private static Microsoft.Xna.Framework.Input.KeyboardState m_current_key_state;

        private static Microsoft.Xna.Framework.Input.GamePadState m_last_gamepad_state;
        private static Microsoft.Xna.Framework.Input.GamePadState m_current_gamepad_state;

        public static void UpdateKeyState() {
            m_last_key_state = m_current_key_state;
            m_current_key_state = Microsoft.Xna.Framework.Input.Keyboard.GetState();
        }

        public static void UpdateGamePadState() {
            m_last_gamepad_state = m_current_gamepad_state;
            m_current_gamepad_state = Microsoft.Xna.Framework.Input.GamePad.GetState(PlayerIndex.One);
        }

        public static Microsoft.Xna.Framework.Input.KeyboardState State {
            get {
                return m_current_key_state;
            }
        }
        public static Microsoft.Xna.Framework.Input.GamePadState GamePadState {
            get {
                return m_current_gamepad_state;
            }
        }

        public static bool KeyDown(Microsoft.Xna.Framework.Input.Keys key) {
            return State.IsKeyDown(key);
        }

        public static bool KeyUp(Microsoft.Xna.Framework.Input.Keys key) {
            return State.IsKeyUp(key);
        }

        public static bool KeyPressedThenReleased(Microsoft.Xna.Framework.Input.Keys key) {
            UpdateKeyState();
            bool last_down = m_last_key_state.IsKeyDown(key);
            bool current_down = m_current_key_state.IsKeyDown(key);

            return last_down && !current_down;
        }

        public static bool KeyPressed(Microsoft.Xna.Framework.Input.Keys key) {
            bool last_down = m_last_key_state.IsKeyDown(key);
            bool current_down = m_current_key_state.IsKeyDown(key);

            return !last_down && current_down;
        }
        public static bool ButtonDown(Microsoft.Xna.Framework.Input.Buttons button) {
            return GamePadState.IsButtonDown(button);
        }
        public static bool ButtonUp(Microsoft.Xna.Framework.Input.Buttons button) {
            return GamePadState.IsButtonUp(button);
        }

        public static bool ButtonPressedThenReleased(Microsoft.Xna.Framework.Input.Buttons button) {
            bool last_down = m_last_gamepad_state.IsButtonDown(button);
            bool current_down = m_current_gamepad_state.IsButtonDown(button);

            return last_down && !current_down;
        }

        public static bool ButtonPressed(Microsoft.Xna.Framework.Input.Buttons button) {
            bool last_down = m_last_gamepad_state.IsButtonDown(button);
            bool current_down = m_current_gamepad_state.IsButtonDown(button);

            return !last_down && current_down;
        }
    }
}
