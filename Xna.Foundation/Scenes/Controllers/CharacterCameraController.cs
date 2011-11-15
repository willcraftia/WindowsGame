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
using Willcraftia.Xna.Framework.Audio;
using Willcraftia.Xna.Foundation.Scenes.Renders;
using Willcraftia.Xna.Framework.Physics;
using Willcraftia.Xna.Framework;

#endregion

namespace Willcraftia.Xna.Foundation.Scenes.Controllers
{
    public class CharacterCameraController : Controller, IExternalForce
    {
        #region Fields and Properties

        CameraActor camera;
        public CameraActor Camera
        {
            get { return camera; }
            set { camera = value; }
        }

        CharacterActor character;
        public CharacterActor Character
        {
            get { return character; }
            set { character = value; }
        }

        #region Camera configuration

        public CameraViewType CameraViewType;
        public bool CanSwitchCameraViewType;
        public float MinCameraDistance;
        public float MaxCameraDistance;
        public float InitialOffsetDistance;

        // Z value equals InitialOffsetDistance.
        Vector3 initialOffsetVector;
        public Vector3 InitialOffsetVector
        {
            get { return initialOffsetVector; }
            set
            {
                if (value.IsZero())
                {
                    throw new ArgumentException(
                        "The initial offset vector must be not a zero vector.",
                        "value");
                }
                initialOffsetVector = value;
                initialOffsetVector.Normalize();
            }
        }

        public float ManualZoomVelocity;
        public float AutoZoomVelocity;
        public float ZoomMinCameraDistance;
        public float CameraCollisionRadius;

        #endregion

        #region Character configuration

        public float MoveVelocity;
        public float DashVelocity;
        public float JumpVelocity;
        public float SlowlyMoveVelocityFactor;

        #endregion

        protected bool IsFirstPersonView { get; set; }

        Vector3 offsetVector;
        float cameraDistance;
        float oldAdjustedCameraDistance;

        float firstPersonCameraDeltaYaw;
        float firstPersonCameraDeltaPitch;

        bool autorunActivated;
        public bool AutorunActivated
        {
            get { return autorunActivated; }
            set { autorunActivated = value; }
        }

        Vector3 desiredVelocity;

        float accelerationInterval = 0.1f;

        bool jumping;
        protected bool IsJumping
        {
            get { return jumping; }
        }

        float jumpTime;
        float jumpInterval = 10.0f / 60.0f;

        #endregion

        #region LoadContent

        public override void LoadContent()
        {
            IsFirstPersonView = (CameraViewType == CameraViewType.FirstPersonView);
            cameraDistance = InitialOffsetDistance;
            oldAdjustedCameraDistance = cameraDistance;

            // calculate the rotated offset vector
            offsetVector = InitialOffsetVector;
            Vector3.Transform(ref offsetVector, ref character.Orientation, out offsetVector);
            offsetVector.Normalize();

            if (IsFirstPersonView)
            {
                camera.Position = character.Position;
                camera.Orientation = character.Orientation;
            }
            else
            {
                // decide the camera position
                camera.Position = character.Position + offsetVector * cameraDistance;

                // decide the camera orientation
                LookAtActor(ref character.Position);
            }

            Character.RigidBody.ExternalForces.Add(this);

            base.LoadContent();
        }

        #endregion

        #region UnloadContent

        public override void UnloadContent()
        {
            Character.RigidBody.ExternalForces.Remove(this);

            base.UnloadContent();
        }

        #endregion

        #region Update

        public override void Update(GameTime gameTime)
        {
            if (!camera.Active) return;

            SwitchPointOfView(gameTime);

            if (IsFirstPersonView)
            {
                UpdateActorByFirstPersonPov(gameTime);
                UpdateCameraByFirstPersonPov(gameTime);
            }
            else
            {
                UpdateActorByThirdPersonPov(gameTime);
                UpdateCameraByThirdPersonPov(gameTime);
            }
        }

        void SwitchPointOfView(GameTime gameTime)
        {
            if (!CanSwitchCameraViewType) return;

            var keyboard = InputDevice.GetKeybord(PlayerIndex);
            var pad = InputDevice.GetGamePad(PlayerIndex);

            if (keyboard.IsKeyPressed(Keys.V) ||
                keyboard.IsKeyPressed(Keys.NumPad5) ||
                pad.IsButtonPressed(Buttons.RightStick))
            {
                if (!IsFirstPersonView)
                {
                    //camera.Orientation = orientation;
                }
                else
                {
                    Vector3.Transform(ref initialOffsetVector, ref character.Orientation, out offsetVector);
                    character.Visible = true;
                }

                IsFirstPersonView = !IsFirstPersonView;
            }
        }

        void UpdateActorByFirstPersonPov(GameTime gameTime)
        {
            float deltaYaw = 0;

            #region Control the orientation for the keyboard

            var keyboard = InputDevice.GetKeybord(PlayerIndex);
            if (keyboard.IsKeyDown(Keys.A))
            {
                deltaYaw += 1.0f;
            }
            if (keyboard.IsKeyDown(Keys.D))
            {
                deltaYaw += -1.0f;
            }
            
            #endregion

            #region Control the orientation for the gamepad

            var pad = InputDevice.GetGamePad(PlayerIndex);
            {
                var stickPosition = pad.CurrentState.ThumbSticks.Left;
                deltaYaw += -stickPosition.X;
            }

            #endregion

            #region Decide the orientation

            // calculate and set the new orientation
            if (deltaYaw != 0)
            {
                float newYaw = character.Orientation.GetYaw() + deltaYaw * gameTime.GetDeltaTime();
                Matrix.CreateRotationY(newYaw, out character.Orientation);
            }

            #endregion

            // 重力加速度。
            Vector3 gravity;
            character.RigidBody.GetGravity(out gravity);

            // 進行方向ベクトル。
            var moveDirection = Vector3.Zero;
            float moveVelocityFactor = 1.0f;

            #region Control the direction for the keyboard

            if (keyboard.IsKeyDown(Keys.W))
            {
                moveDirection.Z += -1;
            }
            if (keyboard.IsKeyDown(Keys.S))
            {
                moveDirection.Z += 1;
            }

            #endregion

            #region Control the direction for the gamepad

            {
                var stickPosition = pad.CurrentState.ThumbSticks.Left;
                moveDirection.Z += -stickPosition.Y;
                var degree = Math.Abs(stickPosition.Y);
                if (degree != 0)
                {
                    moveVelocityFactor = degree;
                }
            }

            #endregion

            // rotate the move direction
            Vector3.Transform(ref moveDirection, ref character.Orientation, out moveDirection);
            Vector3Extension.NormalizeSafe(ref moveDirection, out moveDirection);

            // 重力方向に対して衝突があるかどうかを判定します。
            bool isStanding = character.CollisionBounds.IsCollidedForDirection(ref gravity);

            #region Control the autorun

            if (keyboard.IsKeyPressed(Keys.R) ||
                keyboard.IsKeyPressed(Keys.NumPad7) ||
                pad.IsButtonPressed(Buttons.LeftShoulder))
            {
                if (!autorunActivated)
                {
                    // try to activate autorun
                    if (!moveDirection.IsZero() && isStanding)
                    {
                        autorunActivated = true;
                    }
                }
                else
                {
                    autorunActivated = false;
                }
            }

            if (autorunActivated)
            {
                if (moveDirection.IsZero())
                {
                    moveDirection = character.Orientation.Forward;
                }
                else
                {
                    // decide either to stop or hold autorun
                    if (Vector3.Dot(character.Orientation.Forward, moveDirection) < -0.75f)
                    {
                        autorunActivated = false;
                    }
                }
            }

            #endregion

            #region Control the dash

            bool dashEnabled = false;
            if (keyboard.IsKeyDown(Keys.LeftShift) ||
                pad.IsButtonDown(Buttons.RightShoulder))
            {
                dashEnabled = true;
            }

            #endregion

            #region Control the slowdown (keyboard only)

            // keyboard only
            if (keyboard.IsKeyDown(Keys.RightShift))
            {
                moveVelocityFactor = SlowlyMoveVelocityFactor;
            }

            #endregion

            #region Decide the velocity

            if (dashEnabled)
            {
                desiredVelocity = moveDirection * DashVelocity;
            }
            else
            {
                desiredVelocity = moveDirection * (MoveVelocity * moveVelocityFactor);
            }

            #endregion

            #region Control jumping

            var totalSeconds = (float) gameTime.TotalGameTime.TotalSeconds;

            if (keyboard.IsKeyDown(Keys.Space) || pad.IsButtonDown(Buttons.A))
            {
                if (isStanding)
                {
                    desiredVelocity.Y = JumpVelocity;

                    jumpTime = totalSeconds;
                    jumping = true;
                }
                else if (jumping && totalSeconds < jumpTime + jumpInterval)
                {
                    desiredVelocity.Y = JumpVelocity;
                }
            }

            if (jumping && jumpTime + jumpInterval <= totalSeconds)
            {
                jumping = false;
            }

            #endregion

            Character.RigidBody.Activate();
        }

        void UpdateActorByThirdPersonPov(GameTime gameTime)
        {
            var moveDirection = Vector3.Zero;
            float moveVelocityFactor = 1.0f;

            #region Control the orientation for the keyboard

            var keyboard = InputDevice.GetKeybord(PlayerIndex);
            if (keyboard.IsKeyDown(Keys.W))
            {
                moveDirection.Z += -1;
            }
            if (keyboard.IsKeyDown(Keys.S))
            {
                moveDirection.Z += 1;
            }
            if (keyboard.IsKeyDown(Keys.D))
            {
                moveDirection.X += 1;
            }
            if (keyboard.IsKeyDown(Keys.A))
            {
                moveDirection.X += -1;
            }

            #endregion

            #region Control the orientation for the gamepad

            var pad = InputDevice.GetGamePad(PlayerIndex);
            {
                var delta = pad.CurrentState.ThumbSticks.Left;
                moveDirection.X += delta.X;
                moveDirection.Z += -delta.Y;
                var degree = Math.Max(Math.Abs(delta.X), Math.Abs(delta.Y));
                if (degree != 0)
                {
                    moveVelocityFactor = degree;
                }
            }

            #endregion

            #region Decide the orientation

            Matrix rotation;
            Matrix.CreateRotationY(camera.Orientation.GetYaw(), out rotation);

            Vector3.Transform(ref moveDirection, ref rotation, out moveDirection);
            moveDirection.Y = 0;

            Vector3Extension.NormalizeSafe(ref moveDirection, out moveDirection);

            #endregion

            // 重力加速度。
            Vector3 gravity;
            character.RigidBody.GetGravity(out gravity);

            // 重力方向に対して衝突があるかどうかを判定します。
            bool isStanding = character.CollisionBounds.IsCollidedForDirection(ref gravity);

            #region Control the autorun

            if (keyboard.IsKeyPressed(Keys.R) ||
                keyboard.IsKeyPressed(Keys.NumPad7) ||
                pad.IsButtonPressed(Buttons.LeftShoulder))
            {
                if (!autorunActivated)
                {
                    // try to activate autorun
                    if (!moveDirection.IsZero() && isStanding)
                    {
                        autorunActivated = true;
                    }
                }
                else
                {
                    autorunActivated = false;
                }
            }

            if (autorunActivated)
            {
                if (moveDirection.IsZero())
                {
                    moveDirection = character.Orientation.Forward;
                }
                else
                {
                    // decide either to stop or hold autorun
                    if (Vector3.Dot(character.Orientation.Forward, moveDirection) < 0.25f)
                    {
                        autorunActivated = false;
                    }
                }
            }

            #endregion

            if (!moveDirection.IsZero())
            {
                var newOrientation = Matrix.Identity;
                newOrientation.Forward = moveDirection;
                newOrientation.Up = Vector3.Up;
                newOrientation.Right = Vector3.Cross(moveDirection, Vector3.Up);
                character.Orientation = newOrientation;
            }

            #region Control the dash

            bool dashEnabled = false;
            if (keyboard.IsKeyDown(Keys.LeftShift) ||
                pad.IsButtonDown(Buttons.RightShoulder))
            {
                dashEnabled = true;
            }

            #endregion

            #region Control the slowdown (keyboard only)

            // keyboard only
            if (keyboard.IsKeyDown(Keys.RightShift))
            {
                moveVelocityFactor = SlowlyMoveVelocityFactor;
            }

            #endregion

            #region Decide the velocity

            if (dashEnabled)
            {
                desiredVelocity = moveDirection * DashVelocity;
            }
            else
            {
                desiredVelocity = moveDirection * (MoveVelocity * moveVelocityFactor);
            }

            #endregion

            #region Jump

            var totalSeconds = (float) gameTime.TotalGameTime.TotalSeconds;

            if (keyboard.IsKeyDown(Keys.Space) || pad.IsButtonDown(Buttons.A))
            {
                if (isStanding)
                {
                    desiredVelocity.Y = JumpVelocity;

                    jumpTime = totalSeconds;
                    jumping = true;
                }
                else if (jumping && totalSeconds < jumpTime + jumpInterval)
                {
                    desiredVelocity.Y = JumpVelocity;
                }
            }

            if (jumping && jumpTime + jumpInterval <= totalSeconds)
            {
                jumping = false;
            }

            #endregion

            Character.RigidBody.Activate();
        }

        void UpdateCameraByFirstPersonPov(GameTime gameTime)
        {
            float deltaYaw = 0;
            float deltaPitch = 0;

            //
            // Decide the offset vector from inputs
            //
            var keyboard = InputDevice.GetKeybord(PlayerIndex);
            if (keyboard.IsKeyDown(Keys.Up))
            {
                deltaPitch += 1;
            }
            if (keyboard.IsKeyDown(Keys.Down))
            {
                deltaPitch -= 1;
            }
            if (keyboard.IsKeyDown(Keys.Left))
            {
                deltaYaw += 1;
            }
            if (keyboard.IsKeyDown(Keys.Right))
            {
                deltaYaw -= 1;
            }

            var pad = InputDevice.GetGamePad(PlayerIndex);
            {
                var delta = pad.CurrentState.ThumbSticks.Right;
                deltaYaw += -delta.X;
                deltaPitch += delta.Y;
            }

            float deltaTime = gameTime.GetDeltaTime();

            const float amount = 5.0f;
            if (deltaYaw != 0)
            {
                deltaYaw *= amount * deltaTime;
                firstPersonCameraDeltaYaw += deltaYaw;
                firstPersonCameraDeltaYaw = MathExtension.Clamp(
                    firstPersonCameraDeltaYaw,
                    -MathHelper.PiOver2,
                    MathHelper.PiOver2);
            }
            else
            {
                if (firstPersonCameraDeltaYaw < 0)
                {
                    firstPersonCameraDeltaYaw += amount * deltaTime;
                    firstPersonCameraDeltaYaw = MathHelper.Min(firstPersonCameraDeltaYaw, 0);
                }
                else if (0 < firstPersonCameraDeltaYaw)
                {
                    firstPersonCameraDeltaYaw -= amount * deltaTime;
                    firstPersonCameraDeltaYaw = MathHelper.Max(0, firstPersonCameraDeltaYaw);
                }
            }

            if (deltaPitch != 0)
            {
                deltaPitch *= amount * deltaTime;
                firstPersonCameraDeltaPitch += deltaPitch;
                firstPersonCameraDeltaPitch = MathExtension.Clamp(
                    firstPersonCameraDeltaPitch,
                    -MathHelper.PiOver2,
                    MathHelper.PiOver2);
            }
            else
            {
                if (firstPersonCameraDeltaPitch < 0)
                {
                    firstPersonCameraDeltaPitch += amount * deltaTime;
                    firstPersonCameraDeltaPitch = MathHelper.Min(firstPersonCameraDeltaPitch, 0);
                }
                else if (0 < firstPersonCameraDeltaPitch)
                {
                    firstPersonCameraDeltaPitch -= amount * deltaTime;
                    firstPersonCameraDeltaPitch = MathHelper.Max(0, firstPersonCameraDeltaPitch);
                }
            }

            // calculate the relative orientation from the actor orientation
            float actorYaw = character.Orientation.GetYaw();
            float actorPitch = 0;
            float yaw = actorYaw + firstPersonCameraDeltaYaw;
            float pitch = actorPitch + firstPersonCameraDeltaPitch;

            // set the camera orientation
            Matrix.CreateFromYawPitchRoll(yaw, pitch, 0, out camera.Orientation);

            float adjustedOffsetDistance = 0;
            if (0 < oldAdjustedCameraDistance)
            {
                adjustedOffsetDistance = oldAdjustedCameraDistance - AutoZoomVelocity * deltaTime;
                adjustedOffsetDistance = MathHelper.Max(adjustedOffsetDistance, 0);
                oldAdjustedCameraDistance = adjustedOffsetDistance;
            }
            else
            {
                character.Visible = false;
            }

            //
            // decide and set the camera position
            //
            camera.Position = character.Position + offsetVector * adjustedOffsetDistance;

            //
            // adjust focus distance to the default
            //
            camera.Pov.FocusDistance = Pov.DefaultFocusDistance;
        }

        void UpdateCameraByThirdPersonPov(GameTime gameTime)
        {
            //
            // Decide the offset vector from inputs
            //
            float deltaYaw = 0;
            float deltaPitch = 0;

            var keyboard = InputDevice.GetKeybord(PlayerIndex);
            if (keyboard.IsKeyDown(Keys.Up))
            {
                deltaPitch += 1;
            }
            if (keyboard.IsKeyDown(Keys.Down))
            {
                deltaPitch -= 1;
            }
            if (keyboard.IsKeyDown(Keys.Left))
            {
                deltaYaw += 1;
            }
            if (keyboard.IsKeyDown(Keys.Right))
            {
                deltaYaw -= 1;
            }

            var pad = InputDevice.GetGamePad(PlayerIndex);
            {
                var stickPosition = pad.CurrentState.ThumbSticks.Right;
                deltaYaw += -stickPosition.X;
                deltaPitch += stickPosition.Y;
            }

            float deltaTime = gameTime.GetDeltaTime();

            //const float amount = 0.05f;
            const float amount = 1.0f;
            deltaYaw *= amount * deltaTime;
            deltaPitch *= amount * deltaTime;

            // rotate the offset vector around Y axis
            if (deltaYaw != 0)
            {
                Matrix rotation;
                Matrix.CreateRotationY(deltaYaw, out rotation);
                Vector3.Transform(ref offsetVector, ref rotation, out offsetVector);
            }
            // rotate the offset vector around the right vector of the camera
            if (deltaPitch != 0)
            {
                Vector3 pitchAxis;
                Vector3 up = Vector3.Up;
                Vector3.Cross(ref offsetVector, ref up, out pitchAxis);

                Matrix rotation;
                Matrix.CreateFromAxisAngle(ref pitchAxis, deltaPitch, out rotation);
                Vector3.Transform(ref offsetVector, ref rotation, out offsetVector);
            }

            //
            // Manual zoom
            //
            if (keyboard.IsKeyDown(Keys.PageUp) && keyboard.IsKeyDown(Keys.PageDown))
            {
                cameraDistance = InitialOffsetDistance;
            }
            else
            {
                if (keyboard.IsKeyDown(Keys.PageUp))
                {
                    cameraDistance -= ManualZoomVelocity * gameTime.GetDeltaTime();
                    // reset auto zoom
                    oldAdjustedCameraDistance = cameraDistance;
                }
                if (keyboard.IsKeyDown(Keys.PageDown))
                {
                    cameraDistance += ManualZoomVelocity * gameTime.GetDeltaTime();
                }
            }
            // clamp [ZoomMinCameraDistance, MaxCameraDistance]
            cameraDistance = MathExtension.Clamp(cameraDistance, ZoomMinCameraDistance, MaxCameraDistance);

            //
            // try to adjust the offset distance with models blocking the ray from the camera to the actor
            //
            float adjustedOffsetDistance = CalculateAdjustedOffsetDistance(ref character.Position);

            //
            // Auto zoom
            //
            // interpolate from the adjusted offset distance to the desired offset one
            //
            if (cameraDistance <= adjustedOffsetDistance)
            {
                if (oldAdjustedCameraDistance < cameraDistance)
                {
                    adjustedOffsetDistance = oldAdjustedCameraDistance + AutoZoomVelocity * deltaTime;
                    adjustedOffsetDistance = MathHelper.Min(adjustedOffsetDistance, cameraDistance);
                    oldAdjustedCameraDistance = adjustedOffsetDistance;
                }
                else
                {
                    oldAdjustedCameraDistance = cameraDistance;
                }
            }
            else
            {
                oldAdjustedCameraDistance = adjustedOffsetDistance;
            }

            //
            // decide and set the camera position
            //
            camera.Position = character.Position + offsetVector * adjustedOffsetDistance;

            // update the camera orientation and the view matrix
            LookAtActor(ref character.Position);

            //
            // adjust focus distance to focus the actor
            //
            Vector3 focusPosition;
            Vector3.Transform(ref character.Position, ref camera.Pov.View, out focusPosition);
            camera.Pov.FocusDistance = focusPosition.Length();
        }

        float CalculateAdjustedOffsetDistance(ref Vector3 actorPosition)
        {
            // up
            Vector3 upSegumentVector = offsetVector * cameraDistance + Vector3.Up * CameraCollisionRadius;
            upSegumentVector.Normalize();
            upSegumentVector *= cameraDistance;
            // down
            Vector3 downSegumentVector = offsetVector * cameraDistance + Vector3.Down * CameraCollisionRadius;
            downSegumentVector.Normalize();
            downSegumentVector *= cameraDistance;
            // left
            Vector3 leftSegumentVector = offsetVector * cameraDistance + Vector3.Left * CameraCollisionRadius;
            leftSegumentVector.Normalize();
            leftSegumentVector *= cameraDistance;
            // right
            Vector3 rightSegumentVector = offsetVector * cameraDistance + Vector3.Right * CameraCollisionRadius;
            rightSegumentVector.Normalize();
            rightSegumentVector *= cameraDistance;

            Vector3 intersectPosition;
            Vector3 intersectNormal;
            float intersectFraction;
            float minIntersectFraction = float.MaxValue;
            bool intersected = false;
            foreach (var actor in Scene.Terrains)
            {
                // up
                if (actor.ActorModel.Intersects(ref actorPosition, ref upSegumentVector,
                    out intersectPosition, out intersectNormal, out intersectFraction))
                {
                    minIntersectFraction = MathHelper.Min(minIntersectFraction, intersectFraction);
                    intersected = true;
                }
                // down
                if (actor.ActorModel.Intersects(ref actorPosition, ref downSegumentVector,
                    out intersectPosition, out intersectNormal, out intersectFraction))
                {
                    minIntersectFraction = MathHelper.Min(minIntersectFraction, intersectFraction);
                    intersected = true;
                }
                // left
                if (actor.ActorModel.Intersects(ref actorPosition, ref leftSegumentVector,
                    out intersectPosition, out intersectNormal, out intersectFraction))
                {
                    minIntersectFraction = MathHelper.Min(minIntersectFraction, intersectFraction);
                    intersected = true;
                }
                // right
                if (actor.ActorModel.Intersects(ref actorPosition, ref rightSegumentVector,
                    out intersectPosition, out intersectNormal, out intersectFraction))
                {
                    minIntersectFraction = MathHelper.Min(minIntersectFraction, intersectFraction);
                    intersected = true;
                }
            }

            float adjustedOffsetDistance = intersected ? cameraDistance * minIntersectFraction - CameraCollisionRadius : cameraDistance;
            return MathExtension.Clamp(adjustedOffsetDistance, MinCameraDistance, MaxCameraDistance);
        }

        void UpdateCameraWithInterpolation(GameTime gameTime)
        {
            Vector3 startVector = offsetVector;
            startVector.Normalize();

            Vector3 endVector;
            Vector3.Transform(ref initialOffsetVector, ref character.Orientation, out endVector);
            endVector.Normalize();

            if (0.9999f < Vector3.Dot(startVector, endVector))
            {
                offsetVector = endVector;
            }
            else
            {
                float amount = 0.01f;
                CalculateSphereLenear(ref startVector, ref endVector, amount, out offsetVector);
            }

            camera.Position = character.Position + offsetVector * cameraDistance;

            LookAtActor(ref character.Position);
        }

        void CalculateSphereLenear(ref Vector3 start, ref Vector3 end, float amount, out Vector3 result)
        {
            Vector3 startUnit;
            Vector3Extension.NormalizeSafe(ref start, out startUnit);
            Vector3 endUnit;
            Vector3Extension.NormalizeSafe(ref end, out endUnit);

            float dot;
            Vector3.Dot(ref endUnit, ref startUnit, out dot);
            float angle = (float) Math.Acos(dot);
            float sin = (float) Math.Sin(angle);

            if (sin == 0)
            {
                result = endUnit;
            }
            else
            {
                float startCoeff = (float) Math.Sin(angle * (1 - amount));
                float endCoeff = (float) Math.Sin(angle * amount);

                result = (startCoeff * startUnit + endCoeff * endUnit) / sin;
                Vector3Extension.NormalizeSafe(ref result, out result);
            }
        }

        void LookAtActor(ref Vector3 actorPosition)
        {
            //
            // the forward vector of the camera is equals to the negated offset vector,
            // so we can update them without deciding the camera position.
            //
            Vector3 forward = -offsetVector;
            //Vector3.Subtract(ref actorPosition, ref camera.Position, out forward);
            forward.Normalize();

            Vector3 up = Vector3.Up;
            if (forward == up)
            {
                up = Vector3.Forward;
            }

            Vector3 right;
            Vector3.Cross(ref forward, ref up, out right);
            right.Normalize();

            Vector3.Cross(ref right, ref forward, out up);
            up.Normalize();

            camera.Orientation.Forward = forward;
            camera.Orientation.Up = up;
            camera.Orientation.Right = right;
        }

        #endregion

        #region IExternalForce

        public void ApplyExternalForce(IRigidBody rigidBody, float timeStep)
        {
            Vector3 velocity;
            rigidBody.GetVeclocity(out velocity);
            
            // 重力方向に対して衝突があるかどうかを判定します。
            Vector3 gravity;
            character.RigidBody.GetGravity(out gravity);
            bool isStanding = character.CollisionBounds.IsCollidedForDirection(ref gravity);

            if (isStanding)
            {
                // 現在の速度から desiredVelocity までの速度の変化量。
                var dv = desiredVelocity - velocity;

                // 時間 accelerationInterval で速度 dv に到達するために必要な加速度。
                var acceleration = new Vector3();
                acceleration.X = dv.X / accelerationInterval;
                acceleration.Y = dv.Y / accelerationInterval;
                acceleration.Z = dv.Z / accelerationInterval;

                // 加速度と質量によりかかる力。
                var targetForce = rigidBody.Mass * acceleration;

                rigidBody.ApplyForce(targetForce);
            }
            else
            {
                if (!desiredVelocity.IsZero())
                {
                    // delta velocity
                    var dv = desiredVelocity - velocity;
                    // acceleration
                    var acceleration = new Vector3();
                    acceleration.X = dv.X / accelerationInterval;
                    acceleration.Z = dv.Z / accelerationInterval;

                    //
                    // 速度ベクトルが同じ方向であるほどに通常の加速度
                    // 反対方法であるほどに若干のブレーキをかける
                    //
                    Vector3 tryMoveVelocityUnit;
                    Vector3Extension.NormalizeSafe(ref desiredVelocity, out tryMoveVelocityUnit);
                    Vector3 currentVelocity = velocity;
                    Vector3 currentVelocityUnit;
                    Vector3Extension.NormalizeSafe(ref currentVelocity, out currentVelocityUnit);

                    float dot;
                    Vector3.Dot(ref tryMoveVelocityUnit, ref currentVelocityUnit, out dot);

                    //
                    // dot [-1, 1] -> factor [brake, 1] (brake < 1)
                    //
                    const float brake = 0.01f;
                    //
                    // dot の範囲を長さ 1 へ変更
                    //      dot / (1 - (-1))
                    // それを factor の範囲の長さへ変更
                    //      (dot / 2) * (1 - brake)
                    // その原点を factor の中心へ移動
                    //      (dot / 2) * (1 - brake) + (1 - (1 - brake) / 2)
                    // ゆえに
                    //      (dot * (1 - brake) + (1 + brake)) / 2
                    //
                    var factor = (dot * (1 - brake) + (1 + brake)) * 0.5f;
                    var targetForce = rigidBody.Mass * acceleration * factor;

                    // for jump
                    if (0 < desiredVelocity.Y)
                    {
                        var accelerationY = (desiredVelocity.Y - velocity.Y) / accelerationInterval;
                        targetForce.Y = rigidBody.Mass * accelerationY;
                    }

                    rigidBody.ApplyForce(targetForce);
                }
            }
        }

        #endregion
    }
}
