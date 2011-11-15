#region Using

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Forms;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Design;
using Microsoft.Xna.Framework.Graphics;
using Willcraftia.Content.Studio.Forms;
using Willcraftia.Content.Studio.Models;
using Willcraftia.Content.Studio.Plugin.Foundation.ViewModels;
using Willcraftia.Xna.Foundation.Mock.Physics;
using Willcraftia.Xna.Foundation.Scenes;
using Willcraftia.Xna.Foundation.Scenes.Renders;
using Willcraftia.Xna.Framework;
using Willcraftia.Xna.Framework.Graphics;
using Willcraftia.Xna.Framework.Physics;
using Willcraftia.Win.Xna.Framework;
using Willcraftia.Win.Xna.Framework.Forms;

#endregion

namespace Willcraftia.Content.Studio.Plugin.Foundation.Forms
{
    public sealed class SceneConfigFormControl : GraphicsDeviceControl, ISceneContext
    {
        SpriteBatch spriteBatch;

        Grid grid;

        bool gridVisible = true;

        /// <summary>
        /// グリッドを描画するかどうかを示す値。
        /// </summary>
        /// <value>
        /// true (グリッドを描画する場合)、false (それ以外の場合)。
        /// </value>
        public bool GridVisible
        {
            get { return gridVisible; }
            set
            {
                if (gridVisible == value) return;

                gridVisible = value;
                Invalidate();
            }
        }

        CameraActor activeCamera = new CameraActor();

        /// <summary>
        /// カメラの位置ベクトル。
        /// </summary>
        [TypeConverter(typeof(Vector3Converter))]
        public Vector3 CameraPosition
        {
            get { return activeCamera.Position; }
            set
            {
                activeCamera.Position = value;
                Invalidate();
            }
        }

        /// <summary>
        /// 近くのビュー プレーンとの距離。
        /// </summary>
        public float NearPlaneDistance
        {
            get { return activeCamera.Pov.NearPlaneDistance; }
            set
            {
                activeCamera.Pov.NearPlaneDistance = value;
                Invalidate();
            }
        }

        /// <summary>
        ///  遠くのビュー プレーンとの距離。
        /// </summary>
        public float FarPlaneDistance
        {
            get { return activeCamera.Pov.FarPlaneDistance; }
            set
            {
                activeCamera.Pov.FarPlaneDistance = value;
                Invalidate();
            }
        }

        float dragCameraRotationScale = 0.01f;

        /// <summary>
        /// マウス ドラッグによる Model 回転時のスケール係数。
        /// </summary>
        public float DragCameraRotationScale
        {
            get { return dragCameraRotationScale; }
            set { dragCameraRotationScale = value; }
        }

        float dragCameraTranslationScale = 0.01f;

        /// <summary>
        /// マウス ドラッグによる POV 移動時のスケール係数。
        /// </summary>
        public float DragCameraTranslationScale
        {
            get { return dragCameraTranslationScale; }
            set { dragCameraTranslationScale = value; }
        }

        float wheelMoveScale = 0.01f;

        /// <summary>
        /// マウス ホイールによるズーム時のスケール係数。
        /// </summary>
        public float WheelMoveScale
        {
            get { return wheelMoveScale; }
            set { wheelMoveScale = value; }
        }

        GameTime gameTime = new GameTime(new TimeSpan(DateTime.Now.Ticks), new TimeSpan());

        bool isCameraTranslation;
        bool isCameraRotation;
        System.Drawing.Point dragOffset;

        public SceneConfigViewModel SceneConfigViewModel { get; set; }

        ContentManager contentManager;

        SceneRenderer sceneRenderer;

        bool contentLoaded;
        bool contentLoadFailed;

        public void LoadContent()
        {
            if (SceneConfigViewModel == null) return;
            if (contentLoaded) return;

            if (!SceneConfigViewModel.SceneConfigEdit.ContentLoaded)
            {
                try
                {
                    SceneConfigViewModel.SceneConfigEdit.SceneContext = this;
                    SceneConfigViewModel.SceneConfigEdit.LoadContent();
                    SceneConfigViewModel.SceneConfigEdit.ActiveCamera = activeCamera;
                }
                catch (ContentLoadException e)
                {
                    Tracer.TraceSource.TraceEvent(TraceEventType.Critical, 0, e.Message + "\n" + e.StackTrace);
                    contentLoadFailed = true;
                }
            }

            if (!contentLoadFailed && SceneConfigViewModel.SceneConfigEdit.ContentLoaded)
            {
                sceneRenderer = new SceneRenderer(SceneConfigViewModel.SceneConfigEdit);

                try
                {
                    sceneRenderer.LoadContent();
                }
                catch (ContentLoadException e)
                {
                    Tracer.TraceSource.TraceEvent(TraceEventType.Critical, 0, e.Message + "\n" + e.StackTrace);
                    contentLoadFailed = true;
                }
            }

            contentLoaded = true;
        }

        public void UnloadContent()
        {
            if (SceneConfigViewModel == null) return;

            if (sceneRenderer != null)
            {
                sceneRenderer.UnloadContent();
            }

            SceneConfigViewModel.SceneConfigEdit.ActiveCamera = null;
            SceneConfigViewModel.SceneConfigEdit.UnloadContent();
            SceneConfigViewModel.SceneConfigEdit.SceneContext = null;
        }

        protected override void Initialize()
        {
            if (!DesignMode)
            {
                activeCamera.Pov.AspectRatio = AspectRatio;
                activeCamera.Enabled = true;
                activeCamera.Active = true;

                GraphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;
                GraphicsDevice.BlendState = BlendState.AlphaBlend;

                spriteBatch = new SpriteBatch(GraphicsDevice);
                grid = new Grid(GraphicsDevice);

                contentManager = Workspace.Current.CreateContentManager(this);

                Services.AddService<IPhysicsService>(new MockPhysicsService());
            }

            base.Initialize();
        }

        protected override void Dispose(bool disposing)
        {
            if (!DesignMode)
            {
                UnloadContent();
                contentManager.Dispose();
                spriteBatch.Dispose();
            }

            base.Dispose(disposing);
        }

        protected override void Draw()
        {
            var bgColor = BackColor.ToXnaColor();
            GraphicsDevice.Clear(bgColor);

            // GameTime を計算します。
            CalculateGameTime();

            // カメラを更新します。
            activeCamera.Update(gameTime);

            // グリッドを描画します。
            if (gridVisible)
            {
                DrawGrid();
            }

            if (contentLoaded && !contentLoadFailed)
            {
                // シーンを更新します
                SceneConfigViewModel.SceneConfigEdit.Update(gameTime);

                // シーンを描画します
                sceneRenderer.Draw(gameTime);
            }
        }

        void CalculateGameTime()
        {
            var prevTotalTime = gameTime.TotalGameTime;
            var curTotalTime = new TimeSpan(DateTime.Now.Ticks);
            gameTime = new GameTime(curTotalTime, curTotalTime - prevTotalTime);
        }

        void DrawGrid()
        {
            grid.View = activeCamera.Pov.View;
            grid.Projection = activeCamera.Pov.Projection;
            grid.Draw();
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {

            if (e.Button == MouseButtons.Left)
            {
                isCameraRotation = true;
                dragOffset = e.Location;
            }
            else if (e.Button == MouseButtons.Right)
            {
                isCameraTranslation = true;
                dragOffset = e.Location;
            }

            base.OnMouseDown(e);
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                isCameraRotation = false;
            }
            else if (e.Button == MouseButtons.Right)
            {
                isCameraTranslation = false;
            }

            base.OnMouseUp(e);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            Focus();

            if (isCameraRotation)
            {
                RotateCamera(e);

                dragOffset = e.Location;
            }
            else if (isCameraTranslation)
            {
                TranslateCamera(e);

                dragOffset = e.Location;
            }

            base.OnMouseMove(e);
        }

        void RotateCamera(MouseEventArgs e)
        {
            var point = e.Location;
            var dx = (float) (point.X - dragOffset.X);
            var dy = (float) (point.Y - dragOffset.Y);
            dx *= dragCameraRotationScale;
            dy *= dragCameraRotationScale;

            float yaw;
            float pitch;
            float roll;
            activeCamera.Orientation.ToYawPitchRoll(out yaw, out pitch, out roll);

            yaw -= dx % (2 * MathHelper.Pi);
            pitch -= dy;

            if (MathHelper.PiOver2 < pitch)
            {
                pitch = MathHelper.PiOver2;
            }
            else if (pitch < -MathHelper.PiOver2)
            {
                pitch = -MathHelper.PiOver2;
            }

            activeCamera.Orientation = Matrix.CreateFromYawPitchRoll(yaw, pitch, roll);

            Invalidate();
        }

        void TranslateCamera(MouseEventArgs e)
        {
            var point = e.Location;
            var dx = (float) (point.X - dragOffset.X);
            var dy = (float) (point.Y - dragOffset.Y);
            dx *= dragCameraTranslationScale;
            dy *= dragCameraTranslationScale;

            activeCamera.Position += new Vector3(-dx, dy, 0);

            Invalidate();
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            var wheel = (float) e.Delta;
            wheel *= wheelMoveScale;

            var forward = activeCamera.Orientation.Forward;

            activeCamera.Position += forward * wheel;

            Invalidate();

            base.OnMouseWheel(e);
        }

        protected override void OnClientSizeChanged(EventArgs e)
        {
            activeCamera.Pov.AspectRatio = AspectRatio;

            base.OnClientSizeChanged(e);
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            activeCamera.Pov.AspectRatio = AspectRatio;

            base.OnSizeChanged(e);
        }

        #region ISceneContext

        public ContentManager Content
        {
            get { return contentManager; }
        }

        public ContentManager LocalContent
        {
            get { return contentManager; }
        }

        #endregion

        #region IGameContext

        public SpriteBatch SpriteBatch
        {
            get { return spriteBatch; }
        }

        public void ResetElapsedTime()
        {
        }

        #endregion
    }
}
