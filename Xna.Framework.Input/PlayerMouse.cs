#region Using

using System;
using Microsoft.Xna.Framework.Input;

#endregion

namespace Willcraftia.Xna.Framework.Input
{
    public sealed class PlayerMouse
    {
        public IInputDevice InputDevice { get; private set; }
        public MouseState InitialState { get; private set; }
        public MouseState CurrentState { get; private set; }

        public bool IsMouseMoved
        {
            get
            {
                if (!InputDevice.Enabled) return false;
                return CurrentState != InitialState;
            }
        }

        public int DeltaX
        {
            get
            {
                if (!InputDevice.Enabled) return 0;
                return CurrentState.X - InitialState.X;
            }
        }

        public int DeltaY
        {
            get
            {
                if (!InputDevice.Enabled) return 0;
                return CurrentState.Y - InitialState.Y;
            }
        }

        public PlayerMouse(IInputDevice inputDevice)
        {
            if (inputDevice == null) throw new ArgumentNullException("inputDevice");

            InputDevice = inputDevice;
        }

        public void Initialize(int x, int y)
        {
            Mouse.SetPosition(x, y);
            InitialState = Mouse.GetState();
            CurrentState = Mouse.GetState();
        }

        public void CaptureState()
        {
            if (!InputDevice.Enabled) return;
            CurrentState = Mouse.GetState();
        }

        public void ResetPosition()
        {
            if (!InputDevice.Enabled) return;
            Mouse.SetPosition(InitialState.X, InitialState.Y);
        }
    }
}
