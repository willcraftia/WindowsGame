#region Using

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

#endregion

namespace Willcraftia.Xna.Framework.Input
{
    public sealed class PlayerGamePad
    {
        public IInputDevice InputDevice { get; private set; }
        public PlayerIndex PlayerIndex { get; private set; }

        GamePadState currentState;
        public GamePadState CurrentState
        {
            get { return currentState; }
        }

        GamePadState previousState;
        public GamePadState PreviousState
        {
            get { return previousState; }
        }

        public bool Enabled { get; private set; }

        public bool IsConnected
        {
            get { return Enabled && CurrentState.IsConnected; }
        }

        bool wasConnected;
        public bool WasConnected
        {
            get { return Enabled && wasConnected; }
        }

        public bool IsDisconnected
        {
            get { return Enabled && !CurrentState.IsConnected && wasConnected; }
        }

        public PlayerGamePad(IInputDevice inputDevice, PlayerIndex playerIndex)
        {
            if (inputDevice == null) throw new ArgumentNullException("inputDevice");

            InputDevice = inputDevice;
            PlayerIndex = playerIndex;
            wasConnected = false;
            Enabled = false;
        }

        public void Initialize()
        {
            try
            {
                previousState = GamePad.GetState(PlayerIndex);
                currentState = GamePad.GetState(PlayerIndex);
                Enabled = true;
            }
            catch (InvalidOperationException e)
            {
                previousState = new GamePadState();
                currentState = new GamePadState();
                Enabled = false;
            }
        }

        public void CaptureState()
        {
            if (!InputDevice.Enabled) return;
            if (!Enabled) return;

            previousState = currentState;
            currentState = GamePad.GetState(PlayerIndex);
            if (currentState.IsConnected)
            {
                wasConnected = true;
            }
        }

        public bool IsButtonUp(Buttons button)
        {
            if (!InputDevice.Enabled) return false;
            if (!Enabled) return false;

            return CurrentState.IsButtonUp(button);
        }

        public bool IsButtonDown(Buttons button)
        {
            if (!InputDevice.Enabled) return false;
            if (!Enabled) return false;

            return CurrentState.IsButtonDown(button);
        }

        public bool IsButtonPressed(Buttons button)
        {
            if (!InputDevice.Enabled) return false;
            if (!Enabled) return false;

            return currentState.IsButtonDown(button) && previousState.IsButtonUp(button);
        }

        public bool IsButtonReleased(Buttons button)
        {
            if (!InputDevice.Enabled) return false;
            if (!Enabled) return false;

            return currentState.IsButtonUp(button) && previousState.IsButtonDown(button);
        }
    }
}
