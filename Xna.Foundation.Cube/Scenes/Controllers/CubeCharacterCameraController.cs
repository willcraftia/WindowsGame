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
using Willcraftia.Xna.Foundation.Scenes.Controllers;

#endregion

namespace Willcraftia.Xna.Foundation.Cube.Scenes.Controllers
{
    public sealed class CubeCharacterCameraController : CharacterCameraController
    {
        #region Fields and Properties

        public const string UpstandAnimation = "Upstand";
        public const string WalkAnimation = "Walk";
        public const string AirAnimation = "Air";

        ISound jumpSound;
        ISound footstepSound;
        int footstepStartPartIndex;

        #endregion

        #region LoadContent

        public override void LoadContent()
        {
            base.LoadContent();

            if (!(Character.ActorModel is CubeAnimateCharacterActorModel))
            {
                throw new InvalidOperationException(
                    string.Format("CharacterActor '{0}' must have CubeAnimateCharacterActorModel.", Character.Name));
            }
        }

        #endregion

        #region Update

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (!IsFirstPersonView)
            {
                UpdateCubicCharacterActorModel(gameTime);
            }

            UpdateAudio();
        }

        void UpdateCubicCharacterActorModel(GameTime gameTime)
        {
            ResolvePartIndex(gameTime);
        }

        void ResolvePartIndex(GameTime gameTime)
        {
            var time = (float) gameTime.TotalGameTime.TotalSeconds;

            var model = Character.ActorModel as CubeAnimateCharacterActorModel;
            
            // upstand by default
            var animationName = UpstandAnimation;
            
            // for walk speed
            float degree = 1.0f;

            // 重力方向に対して衝突があるかどうかを判定します。
            Vector3 gravity;
            Character.RigidBody.GetGravity(out gravity);
            bool isStanding = Character.CollisionBounds.IsCollidedForDirection(ref gravity);

            if (isStanding)
            {
                #region Walk [keyboards]

                var keyboard = InputDevice.GetKeybord(PlayerIndex);
                if (keyboard.IsKeyDown(Keys.W) ||
                    keyboard.IsKeyDown(Keys.S) ||
                    keyboard.IsKeyDown(Keys.D) ||
                    keyboard.IsKeyDown(Keys.A))
                {
                    animationName = WalkAnimation;
                }

                #endregion

                #region Walk [gamepads]

                var pad = InputDevice.GetGamePad(PlayerIndex);
                {
                    var delta = pad.CurrentState.ThumbSticks.Left;
                    var maxDelta = Math.Max(Math.Abs(delta.X), Math.Abs(delta.Y));
                    if (0 < maxDelta)
                    {
                        animationName = WalkAnimation;
                        degree = maxDelta;
                    }
                }

                #endregion

                #region Walk [autorun]

                if (AutorunActivated)
                {
                    animationName = WalkAnimation;
                }

                #endregion

                #region Dash

                if (keyboard.IsKeyDown(Keys.LeftShift) ||
                    pad.IsButtonDown(Buttons.RightShoulder))
                {
                    degree *= 2.0f;
                }

                #endregion

                #region Slow animation speed down (keyboard only)

                // keyboard only
                if (keyboard.IsKeyDown(Keys.RightShift))
                {
                    degree *= 0.5f;
                }

                #endregion
            }
            else
            {
                #region Jump and so on

                animationName = AirAnimation;

                #endregion
            }

            #region Cacluate the current animatin frame and its part index

            var animation = model.CubeAnimateCharacter.Animations[animationName];
            var frame = (int) (time * degree * animation.FramesPerSecond) % animation.FrameCount;
            model.CurrentPartIndex = animation.ModelIndices[frame];

            #endregion
        }

        void UpdateAudio()
        {
            UpdateListener();

            if (IsJumping)
            {
                if (jumpSound == null)
                {
                    var emitter = new AudioEmitter();
                    UpdateActorEmitter(emitter);

                    var key = new ActionSoundKey("Jump", null);
                    string soundName;
                    if (Character.ActionSounds.TryGetValue(key, out soundName))
                    {
                        jumpSound = Scene.CharacterAudioManager.CreateSound(soundName, emitter);
                        jumpSound.Play();
                    }
                }
                else if (jumpSound.IsPlaying)
                {
                    UpdateActorEmitter(jumpSound.Emitter);
                }
            }
            else
            {
                if (jumpSound != null)
                {
                    if (jumpSound.IsDisposed)
                    {
                        jumpSound = null;
                    }
                    else if (jumpSound.IsPlaying)
                    {
                        UpdateActorEmitter(jumpSound.Emitter);
                    }
                }
            }

            if (footstepSound != null)
            {
                if (footstepSound.IsDisposed)
                {
                    footstepSound = null;
                    //footstepStartPartIndex = 0;
                }
                else if (footstepSound.IsPlaying)
                {
                    UpdateActorEmitter(footstepSound.Emitter);
                }
            }


            // 重力方向に対して衝突があるかどうかを判定します。
            Vector3 gravity;
            Character.RigidBody.GetGravity(out gravity);
            bool isStanding = Character.CollisionBounds.IsCollidedForDirection(ref gravity);

            if (isStanding)
            {
                Vector3 velocity;
                Character.RigidBody.GetVeclocity(out velocity);
                velocity.Y = 0;
                if (0 < velocity.LengthSquared())
                {
                    if (footstepSound == null)
                    {
                        var model = Character.ActorModel as CubeAnimateCharacterActorModel;
                        var currentPartIndex = model.CurrentPartIndex;
                        if (footstepStartPartIndex != currentPartIndex)
                        {
                            var emitter = new AudioEmitter();
                            UpdateActorEmitter(emitter);

                            var key = new ActionSoundKey("Footstep", null);
                            string soundName;
                            if (Character.ActionSounds.TryGetValue(key, out soundName))
                            {
                                footstepSound = Scene.CharacterAudioManager.CreateSound(soundName, emitter);
                                footstepSound.Play();
                                footstepStartPartIndex = currentPartIndex;
                            }
                        }
                    }
                }
            }
        }

        void UpdateListener()
        {
            var camera = Scene.ActiveCamera;
            Scene.CharacterAudioManager.Listener.Position = camera.Position;
            Scene.CharacterAudioManager.Listener.Forward = camera.Orientation.Forward;
            Scene.CharacterAudioManager.Listener.Up = camera.Orientation.Up;

            // Velocity follows the actor's rigid body
            Vector3 velocity;
            Character.RigidBody.GetVeclocity(out velocity);
            Scene.CharacterAudioManager.Listener.Velocity = velocity;
        }

        void UpdateActorEmitter(AudioEmitter emitter)
        {
            // Velocity follows the rigid body
            Vector3 velocity;
            Character.RigidBody.GetVeclocity(out velocity);

            emitter.Position = Character.Position;
            emitter.Forward = Character.Orientation.Forward;
            emitter.Up = Character.Orientation.Up;
            emitter.Velocity = velocity;
        }

        #endregion
    }
}
