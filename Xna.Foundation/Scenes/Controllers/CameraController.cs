#region Using

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Willcraftia.Xna.Framework;
using Willcraftia.Xna.Framework.Input;

#endregion

namespace Willcraftia.Xna.Foundation.Scenes.Controllers
{
    public sealed class CameraController : Controller
    {
        #region Fields and Properties

        public string CameraName;
        public float RotationVelocity;
        public float MoveVelocity;

        CameraActor camera;

        #endregion

        #region LoadContent

        public override void LoadContent()
        {
            camera = Scene.Cameras[CameraName];

            base.LoadContent();
        }

        #endregion

        #region Update

        public override void Update(GameTime gameTime)
        {
            if (!camera.Active) return;

            var dt = gameTime.GetDeltaTime();

            var newPosition = camera.Position;
            var newOrientation = camera.Orientation;

            var deltaRotation = new Vector2();

            var mouse = InputDevice.GetMouse();
            if (mouse.IsMouseMoved)
            {
                deltaRotation.X += mouse.DeltaX;
                deltaRotation.Y += mouse.DeltaY;
                mouse.ResetPosition();
            }

            var pad = InputDevice.GetGamePad(PlayerIndex);
            {
                const float padRotationAmount = 10.0f;

                var stickPosition = pad.CurrentState.ThumbSticks.Right;
                deltaRotation.X += stickPosition.X * padRotationAmount;
                deltaRotation.Y += -stickPosition.Y * padRotationAmount;
            }

            {
                float yaw;
                float pitch;
                float roll;
                camera.Orientation.ToYawPitchRoll(out yaw, out pitch, out roll);

                yaw -= RotationVelocity * deltaRotation.X * dt % (2 * MathHelper.Pi);
                pitch -= RotationVelocity * deltaRotation.Y * dt;

                if (MathHelper.PiOver2 < pitch)
                {
                    pitch = MathHelper.PiOver2;
                }
                else if (pitch < -MathHelper.PiOver2)
                {
                    pitch = -MathHelper.PiOver2;
                }

                newOrientation = Matrix.CreateFromYawPitchRoll(yaw, pitch, roll);
            }

            var moveDirection = new Vector3(0, 0, 0);

            var keyboard = InputDevice.GetKeybord(PlayerIndex);
            if (keyboard.IsKeyDown(Keys.W))
            {
                moveDirection.Z = -1;
            }
            if (keyboard.IsKeyDown(Keys.S))
            {
                moveDirection.Z = 1;
            }
            if (keyboard.IsKeyDown(Keys.D))
            {
                moveDirection.X = 1;
            }
            if (keyboard.IsKeyDown(Keys.A))
            {
                moveDirection.X = -1;
            }
            if (keyboard.IsKeyDown(Keys.Q))
            {
                moveDirection.Y = 1;
            }
            if (keyboard.IsKeyDown(Keys.Z))
            {
                moveDirection.Y = -1;
            }

            {
                var delta = pad.CurrentState.ThumbSticks.Left;
                moveDirection.X += delta.X;
                moveDirection.Z += -delta.Y;

                if (pad.IsButtonDown(Buttons.LeftShoulder))
                {
                    moveDirection.Y += 1;
                }
                if (pad.IsButtonDown(Buttons.RightShoulder))
                {
                    moveDirection.Y += -1;
                }
            }

            if (!moveDirection.IsZero())
            {
                moveDirection.Normalize();
                newPosition += MoveVelocity * dt * Vector3.Transform(moveDirection, newOrientation);
            }

            camera.Position = newPosition;
            camera.Orientation = newOrientation;
        }

        #endregion
    }
}
