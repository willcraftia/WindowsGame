#region Using

using System;
using System.ComponentModel;
using System.Windows.Forms;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Design;
using Microsoft.Xna.Framework.Graphics;
using Willcraftia.Win.Xna.Framework;
using Willcraftia.Xna.Framework;
using Willcraftia.Xna.Framework.Graphics;
using Willcraftia.Xna.Foundation.Scenes;
using Willcraftia.Content.Studio.Forms;

using WDirectionalLight = Willcraftia.Xna.Foundation.Scenes.DirectionalLight;

#endregion

namespace Willcraftia.Content.Studio.Plugin.Foundation.Forms
{
    public sealed class ActorFormControl : RuntimeContentFormControl, IActorContext
    {
        Actor actor;
        Grid grid;

        /// <summary>
        /// ライティングを行うかどうかを示す値。
        /// </summary>
        /// <value>
        /// true (ライティングを行う場合)、 false (それ以外の場合)。
        /// </value>
        public bool LightingEnabled
        {
            get { return sceneSettings.DirectionalLight0.Enabled; }
            set
            {
                sceneSettings.DirectionalLight0.Enabled = value;
                Invalidate();
            }
        }

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
                if (gridVisible != value)
                {
                    gridVisible = value;
                    Invalidate();
                }
            }
        }

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

        float minZoomDistance = 5.0f;

        /// <summary>
        /// ズーム時の最近距離。
        /// </summary>
        public float MinZoomDistance
        {
            get { return minZoomDistance; }
            set { minZoomDistance = value; }
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

        float wheelZoomScale = 0.01f;

        /// <summary>
        /// マウス ホイールによるズーム時のスケール係数。
        /// </summary>
        public float WheelZoomScale
        {
            get { return wheelZoomScale; }
            set { wheelZoomScale = value; }
        }

        float dragModelRotationScale = 0.01f;

        /// <summary>
        /// マウス ドラッグによる Model 回転時のスケール係数。
        /// </summary>
        public float DragModelRotationScale
        {
            get { return dragModelRotationScale; }
            set { dragModelRotationScale = value; }
        }

        float dragPovTranslationScale = 0.01f;

        /// <summary>
        /// マウス ドラッグによる POV 移動時のスケール係数。
        /// </summary>
        public float DragPovTranslationScale
        {
            get { return dragPovTranslationScale; }
            set { dragPovTranslationScale = value; }
        }

        Vector3 lastPovPosition = Vector3.Zero;
        Matrix lastActorOrientation = Matrix.Identity;
        GameTime gameTime = new GameTime(new TimeSpan(DateTime.Now.Ticks), new TimeSpan());

        bool isPovTranslation;
        bool isActorRotation;
        System.Drawing.Point dragOffset;

        public ActorFormControl()
        {
            sceneSettings.DirectionalLight0 = WDirectionalLight.Default;
        }

        public override void LoadContent()
        {
            if (RuntimeContent == null) return;

            actor = RuntimeContent.LoadContent(ContentManager) as Actor;
            if (actor == null) return;

            actor.ActorContext = this;
            actor.LoadContent();

            Invalidate();
        }

        public override void UnloadContent()
        {
            if (actor != null)
            {
                actor.UnloadContent();
                actor.ActorContext = null;
                actor = null;
            }

            Invalidate();
        }

        protected override void Initialize()
        {
            if (!DesignMode)
            {
                activeCamera.Pov.AspectRatio = AspectRatio;
                activeCamera.Enabled = true;
                activeCamera.Active = true;

                // 表示が見辛くなるのでフォグを OFF にしておきます。
                sceneSettings.Fog.Enabled = false;

                GraphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;
                GraphicsDevice.BlendState = BlendState.AlphaBlend;

                spriteBatch = new SpriteBatch(GraphicsDevice);
                grid = new Grid(GraphicsDevice);
            }

            base.Initialize();
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

            // Actor を描画します。
            if (actor != null)
            {
                DrawActor();
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

        void DrawActor()
        {
            actor.Update(gameTime);
            actor.ActorModel.Draw(gameTime);
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {

            if (e.Button == MouseButtons.Left)
            {
                isActorRotation = true;
                dragOffset = e.Location;

                if (actor != null)
                {
                    lastActorOrientation = actor.Orientation;
                }
            }
            else if (e.Button == MouseButtons.Right)
            {
                isPovTranslation = true;
                dragOffset = e.Location;
                lastPovPosition = activeCamera.Position;
            }

            base.OnMouseDown(e);
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                isActorRotation = false;
            }
            else if (e.Button == MouseButtons.Right)
            {
                isPovTranslation = false;
            }

            base.OnMouseUp(e);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            Focus();

            if (isActorRotation)
            {
                RotateActor(e);
            }
            else if (isPovTranslation)
            {
                TranslatePov(e);
            }

            base.OnMouseMove(e);
        }

        void RotateActor(MouseEventArgs e)
        {
            var point = e.Location;
            var dx = (float) (point.X - dragOffset.X);
            var dy = (float) (point.Y - dragOffset.Y);
            dx *= dragModelRotationScale;
            dy *= dragModelRotationScale;

            if (actor != null)
            {
                actor.Orientation = lastActorOrientation * Matrix.CreateRotationY(dx) * Matrix.CreateRotationX(dy);
                actor.Update(gameTime);
                Invalidate();
            }
        }

        void TranslatePov(MouseEventArgs e)
        {
            var point = e.Location;
            var dx = (float) (point.X - dragOffset.X);
            var dy = (float) (point.Y - dragOffset.Y);
            dx *= dragPovTranslationScale;
            dy *= dragPovTranslationScale;

            Vector3 position = lastPovPosition;
            position.X -= dx;
            position.Y += dy;
            activeCamera.Position = position;

            Invalidate();
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            var wheel = (float) e.Delta;
            wheel *= wheelZoomScale;

            activeCamera.Position.Z = Math.Max(minZoomDistance, activeCamera.Position.Z + wheel);

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

        public void ResetActorOrientation(Matrix orientation)
        {
            if (actor == null) return;

            actor.Orientation = orientation;
            Invalidate();
        }

        #region IActorContext

        public ContentManager Content
        {
            get { return ContentManager; }
        }

        SceneSettings sceneSettings = new SceneSettings();
        public SceneSettings SceneSettings
        {
            get { return sceneSettings; }
        }

        CameraActor activeCamera = new CameraActor();
        public CameraActor ActiveCamera
        {
            get { return activeCamera; }
        }

        #endregion

        #region IGameContext

        SpriteBatch spriteBatch;
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
