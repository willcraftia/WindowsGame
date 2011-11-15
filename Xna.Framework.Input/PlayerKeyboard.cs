#region Using

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

#endregion

namespace Willcraftia.Xna.Framework.Input
{
    public sealed class PlayerKeyboard
    {
        public IInputDevice InputDevice { get; private set; }
        public PlayerIndex PlayerIndex { get; private set; }
        public KeyboardState CurrentState { get; private set; }
        public KeyboardState PreviousState { get; private set; }

        public PlayerKeyboard(IInputDevice inputDevice, PlayerIndex playerIndex)
        {
            if (inputDevice == null) throw new ArgumentNullException("inputDevice");

            InputDevice = inputDevice;
            PlayerIndex = playerIndex;
        }

        public void Initialize()
        {
            PreviousState = Keyboard.GetState(PlayerIndex);
        }

        public void CaptureState()
        {
            if (!InputDevice.Enabled) return;

            PreviousState = CurrentState;
            CurrentState = Keyboard.GetState(PlayerIndex);
        }

        public bool IsKeyUp(Keys key)
        {
            if (!InputDevice.Enabled) return false;
            return CurrentState.IsKeyUp(key);
        }

        public bool IsKeyDown(Keys key)
        {
            if (!InputDevice.Enabled) return false;
            return CurrentState.IsKeyDown(key);
        }

        public bool IsKeyPressed(Keys key)
        {
            if (!InputDevice.Enabled) return false;
            return CurrentState.IsKeyDown(key) && PreviousState.IsKeyUp(key);
        }

        public bool IsKeyReleased(Keys key)
        {
            if (!InputDevice.Enabled) return false;
            return CurrentState.IsKeyUp(key) && PreviousState.IsKeyDown(key);
        }
    }
}
