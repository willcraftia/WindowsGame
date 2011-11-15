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
using Willcraftia.Xna.Framework.Audio;
using Willcraftia.Xna.Framework.Input;
using Willcraftia.Xna.Framework.Physics;
using Willcraftia.Xna.Foundation;
using Willcraftia.Xna.Foundation.Debugs;
using Willcraftia.Xna.Foundation.Graphics;
using Willcraftia.Xna.Foundation.Scenes.Renders;
using Willcraftia.Xna.Foundation.Scenes;
using Willcraftia.Xna.Foundation.Scenes.Mediators;
using Willcraftia.Xna.Foundation.Screens;

using Willcraftia.Xna.Foundation.Cube.Scenes.Mediators;

using WindowsGame8.Inputs;

using JigLibX.Collision;
using JigLibX.Geometry;
using JigLibX.Physics;

using Willcraftia.Xna.Foundation.JigLib.Cube;
using Willcraftia.Xna.Foundation.JigLib;

#endregion

namespace WindowsGame8.Screens
{
    class Test1Screen : Screen
    {
        #region Inner classes

        class SceneContext : ISceneContext
        {
            Test1Screen test1Screen;

            internal SceneContext(Test1Screen test1Screen)
            {
                this.test1Screen = test1Screen;
            }

            #region ISceneContext

            public ContentManager Content
            {
                get { return test1Screen.Content; }
            }

            public ContentManager LocalContent
            {
                get { return test1Screen.LocalContent; }
            }

            #endregion

            #region IGameContext

            public GraphicsDevice GraphicsDevice
            {
                get { return test1Screen.ScreenContext.GraphicsDevice; }
            }

            public void ResetElapsedTime()
            {
                test1Screen.ScreenContext.ResetElapsedTime();
            }

            #endregion

            #region IServiceProvider

            public object GetService(Type serviceType)
            {
                return test1Screen.ScreenContext.GetService(serviceType);
            }

            #endregion
        }

        #endregion

        #region Fields

        SceneContext sceneContext;
        Scene scene;
        SceneRenderer sceneRenderer;

        #endregion

        #region Constructors

        public Test1Screen()
        {
            sceneContext = new SceneContext(this);
            TransitionOnTime = TimeSpan.FromSeconds(1.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);
        }

        #endregion

        #region LoadContent

        public override void LoadContent()
        {
            base.LoadContent();

            scene = new Scene(sceneContext, "Scenes/Test1Scene");

            sceneRenderer = new SceneRenderer(scene);

            #region Start-up mediators

            #region CameraMeditor [free]
            {
                var mediator = new CameraMediator();
                mediator.CameraName = "free";
                mediator.MoveVelocity = 30.0f;
                scene.StartupMediators.Add(mediator);
            }
            #endregion

            #region CameraMediator [satellite]
            {
                var mediator = new CameraMediator();
                mediator.CameraName = "satellite";
                mediator.MoveVelocity = 60.0f;
                scene.StartupMediators.Add(mediator);
            }
            #endregion

            #region CubeCharacterCameraMediator [char/Player]
            {
                var mediator = new CubeCharacterCameraMediator();
                mediator.CameraName = "char";
                mediator.CharacterActorName = "Player";
                mediator.DashVelocity = 20.0f;
                scene.StartupMediators.Add(mediator);
            }
            #endregion

            #region WorldTimeMediator
            {
                var mediator = new WorldTimeMediator();
                scene.StartupMediators.Add(mediator);
            }
            #endregion

            #region SunLightMediator
            {
                var mediator = new SunLightMediator();
                scene.StartupMediators.Add(mediator);
            }
            #endregion

            #endregion

            #region Player

            scene.Player.ControllingPlayer = ControllingPlayer.Value;

            #endregion

            scene.LoadContent();
            sceneRenderer.LoadContent();

            LoadDebugContent();

            // 読み込みが完了したら、ResetElapsedTime() を使用して、非常に長い
            // フレームを完了したことと、キャッチアップしようとする必要がないことを、
            // ゲームのタイミング メカニズムに指示します。
            ScreenContext.ResetElapsedTime();
        }

        #endregion

        #region UnloadContent

        public override void UnloadContent()
        {
            scene.UnloadContent();
            sceneRenderer.UnloadContent();

            UnloadDebugContent();
        }

        #endregion

        #region Update

        /// <summary>
        /// ゲームの状態を更新します。このメソッドは GameScreen.IsActive プロパティを
        /// チェックして、一時停止メニューがアクティブなとき、または Alt-Tab キーを押して
        /// 別のアプリケーションに切り替えた際に、ゲームが更新を停止するようにします。
        /// </summary>
        public override void Update(GameTime gameTime)
        {
            if (TimeRuler.Instance != null)
            {
                TimeRuler.Instance.BeginMark("Update", Color.Pink);
            }

            base.Update(gameTime);

            var physicsService = ScreenContext.GetRequiredService<IPhysicsService>();

            if (HasFocus)
            {
                scene.Update(gameTime);

                scene.AmbientSoundManager.PlayOrResume();

                physicsService.Enabled = true;
            }
            else
            {
                scene.AmbientSoundManager.Pause();

                physicsService.Enabled = false;
            }

            if (TimeRuler.Instance != null)
            {
                TimeRuler.Instance.EndMark("Update");
            }
        }

        /// <summary>
        /// ゲームがプレイヤーの入力に応答できるようにします。Update メソッドと異なり、
        /// ゲームプレイ画面がアクティブな場合にのみ呼び出されます。
        /// </summary>
        public override void HandleInput(GameTime gameTime)
        {
            var gamePad = InputDevice.GetGamePad(ControllingPlayer.Value);

            PlayerIndex playerIndex;
            if (InputDevice.IsPauseGame(ControllingPlayer, out playerIndex) || gamePad.IsDisconnected)
            {
                Audio.SharedAudioManager.CreateSound(SoundConstants.MessageBoxOpenSound).Play();

                var screen = new PauseMenuScreen();
                screen.ControllingPlayer = ControllingPlayer;
                ScreenContext.AddScreen(screen);
            }
            else
            {
                scene.HandleInput(gameTime);
            }
        }

        #endregion

        #region Draw

        /// <summary>
        /// ゲームプレイ画面を描画します。
        /// </summary>
        public override void Draw(GameTime gameTime)
        {
            //
            // TODO
            //
            scene.AmbientSoundManager.AmbientSounds.ForEach(
                sound =>
                {
                    sceneRenderer.DebugRenderer.AmbientSounds.Add(sound);
                });

            if (sceneRenderer.ContentLoaded)
            {
                sceneRenderer.Draw(gameTime);
            }

            // ゲームがオンまたはオフに移行する場合、表示画面をフェード アウトします。
            if (TransitionPosition > 0)
            {
                ScreenContext.FadeBackBuffer(1.0f - TransitionAlpha, ScreenContext.BackgroundColor);
            }
        }

        #endregion

        #region LoadDebugContent/UnloadDebugContent

        void LoadDebugContent()
        {
            if (DebugConsole.Instance != null)
            {
                #region Command [player]

                DebugConsole.Instance.RegisterDebugCommand("player", "Configure player",
                    delegate(IDebugCommandHost host, string command, IList<string> arguments)
                    {
                        var camera = scene.ActiveCamera;
                        var actor = scene.Player.Actor;

                        Matrix orientation;
                        actor.RigidBody.GetOrientation(out orientation);

                        host.Echo("Position: " + actor.Position);
                        host.Echo("Model Forward: " + actor.Orientation.Forward);
                        host.Echo("Body Forward: " + orientation.Forward);

                        var forwardInView = Vector3.Transform(
                            actor.Position + actor.Orientation.Forward,
                            camera.Pov.View);
                        host.Echo("Model Forward in View: " + forwardInView);

                        host.Echo("Yaw/Pitch/Roll: " +
                            actor.Orientation.GetYaw() + "/" +
                            actor.Orientation.GetPitch() + "/" +
                            actor.Orientation.GetRoll());
                    });

                #endregion

                #region Command[cam]

                DebugConsole.Instance.RegisterDebugCommand("cam", "Configure camera",
                    delegate(IDebugCommandHost host, string command, IList<string> arguments)
                    {
                        if (arguments.Count == 0)
                        {
                            host.Echo("Active camera: " + scene.ActiveCamera.Name);
                            host.Echo("Cameras:");
                            foreach (var camera in scene.Cameras)
                            {
                                host.Echo("    " + camera.Name);
                            }
                        }
                        else
                        {
                            var sub = arguments[0];

                            if (sub == "set")
                            {
                                if (arguments.Count == 1)
                                {
                                    host.Echo("Active camera: " + scene.ActiveCamera.Name);
                                }
                                else
                                {
                                    var name = arguments[1];
                                    if (scene.Cameras.Contains(name))
                                    {
                                        scene.Cameras[name].Active = true;
                                    }
                                    else
                                    {
                                        host.Echo("Invalid camera name: " + name);
                                    }
                                }
                            }
                            else if (sub == "info")
                            {
                                host.Echo("Name: " + scene.ActiveCamera.Name);
                                var pov = scene.ActiveCamera.Pov;
                                Vector3 position;
                                Matrix orientation;
                                pov.GetPosition(out position);
                                pov.GetOrientation(out orientation);
                                host.Echo("Position: " + position);
                                host.Echo("Forward: " + orientation.Forward);
                                host.Echo("Up: " + orientation.Up);
                                host.Echo("FoV: " + pov.Fov);
                                host.Echo("Aspect ratio: " + pov.AspectRatio);
                                host.Echo("Near: " + pov.NearPlaneDistance);
                                host.Echo("Far: " + pov.FarPlaneDistance);
                                host.Echo("Focus range: " + pov.FocusRange);
                                host.Echo("Focus distance: " + pov.FocusDistance);
                            }
                            else if (sub == "near")
                            {
                                if (arguments.Count == 1)
                                {
                                    host.Echo("Near plane distance: " + scene.ActiveCamera.Pov.NearPlaneDistance);
                                }
                                else
                                {
                                    try
                                    {
                                        scene.ActiveCamera.Pov.NearPlaneDistance = float.Parse(arguments[1]);
                                    }
                                    catch (Exception e)
                                    {
                                        host.EchoError(e.Message);
                                    }
                                }
                            }
                            else if (sub == "far")
                            {
                                if (arguments.Count == 1)
                                {
                                    host.Echo("Far plane distance: " + scene.ActiveCamera.Pov.FarPlaneDistance);
                                }
                                else
                                {
                                    try
                                    {
                                        scene.ActiveCamera.Pov.FarPlaneDistance = float.Parse(arguments[1]);
                                    }
                                    catch (Exception e)
                                    {
                                        host.EchoError(e.Message);
                                    }
                                }
                            }
                            else if (sub == "focusrange")
                            {
                                if (arguments.Count == 1)
                                {
                                    host.Echo("Focus range: " + scene.ActiveCamera.Pov.FocusRange);
                                }
                                else
                                {
                                    try
                                    {
                                        scene.ActiveCamera.Pov.FocusRange = float.Parse(arguments[1]);
                                    }
                                    catch (Exception e)
                                    {
                                        host.EchoError(e.Message);
                                    }
                                }
                            }
                            else if (sub == "focusdistance")
                            {
                                if (arguments.Count == 1)
                                {
                                    host.Echo("Focus distance: " + scene.ActiveCamera.Pov.FocusDistance);
                                }
                                else
                                {
                                    try
                                    {
                                        scene.ActiveCamera.Pov.FocusDistance = float.Parse(arguments[1]);
                                    }
                                    catch (Exception e)
                                    {
                                        host.EchoError(e.Message);
                                    }
                                }
                            }
                        }
                    });
                #endregion
            }

            #region TimeRulerDrawCallback の登録

            int trIndex = 1;
            sceneRenderer.DrawCallbacks.Add(
                new TimeRulerDrawCallback("Draw", Color.Yellow, trIndex++));
            sceneRenderer.RenderTargetPreparedDrawCallbacks.Add(
                new TimeRulerDrawCallback("RenderTargetPrepareds", Color.LightSkyBlue, trIndex++));
            sceneRenderer.ShadowMapDrawCallbacks.Add(
                new TimeRulerDrawCallback("Shadow", Color.Gray, trIndex++));
            sceneRenderer.ShadowSceneMapDrawCallbacks.Add(
                new TimeRulerDrawCallback("ShadowScene", Color.Gray, trIndex++));
            sceneRenderer.SceneMapDrawCallbacks.Add(
                new TimeRulerDrawCallback("Scene", Color.Yellow, trIndex++));
            sceneRenderer.ShadowPostProcessDrawCallbacks.Add(
                new TimeRulerDrawCallback("ShadowPostProcess", Color.Gray, trIndex++));
            sceneRenderer.SsaoDrawCallbacks.Add(
                new TimeRulerDrawCallback("Ssao", Color.Yellow, trIndex++));
            sceneRenderer.DofDrawCallbacks.Add(
                new TimeRulerDrawCallback("Dof", Color.Yellow, trIndex++));
            sceneRenderer.FinalScreenDrawCallbacks.Add(
                new TimeRulerDrawCallback("FinalScreen", Color.Yellow, trIndex++));

            #endregion
        }

        void UnloadDebugContent()
        {
            if (DebugConsole.Instance != null)
            {
                DebugConsole.Instance.UnregisterDebugCommand("player");
                DebugConsole.Instance.UnregisterDebugCommand("cam");
            }
        }

        #endregion
    }
}
