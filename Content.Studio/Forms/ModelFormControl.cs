#region Using

using System;
using System.ComponentModel;
using System.Windows.Forms;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Design;
using Microsoft.Xna.Framework.Graphics;
using Willcraftia.Xna.Framework;
using Willcraftia.Xna.Framework.Graphics;
using Willcraftia.Win.Xna.Framework;
using Willcraftia.Win.Xna.Framework.Design;

#endregion

namespace Willcraftia.Content.Studio.Forms
{
    public class ModelFormControl : RuntimeContentFormControl
    {
        protected Model Model { get; set; }

        Pov pov = new Pov();
        Grid grid;

        bool lightingEnabled = true;

        /// <summary>
        /// ライティングを行うかどうかを示す値。
        /// </summary>
        /// <value>
        /// true (ライティングを行う場合)、 false (それ以外の場合)。
        /// </value>
        public bool LightingEnabled
        {
            get { return lightingEnabled; }
            set
            {
                if (lightingEnabled != value)
                {
                    lightingEnabled = value;
                    Invalidate();
                }
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
        /// POV の位置ベクトル。
        /// </summary>
        [TypeConverter(typeof(Vector3Converter))]
        public Vector3 PovPosition
        {
            get
            {
                Vector3 position;
                pov.GetPosition(out position);
                return position;
            }
            set
            {
                pov.SetPosition(ref value);
                Invalidate();
            }
        }

        /// <summary>
        /// Model の姿勢ベクトル (x:Pitch, y:Yaw, z:Roll)。
        /// </summary>
        [TypeConverter(typeof(Vector3Converter))]
        public Vector3 ModelOrientation
        {
            get
            {
                Vector3 orientation;
                modelOrientation.ToYawPitchRoll(out orientation.Y, out orientation.X, out orientation.Z);
                return orientation;
            }
            set
            {
                Matrix.CreateFromYawPitchRoll(value.Y, value.X, value.Z, out modelOrientation);
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
            get { return pov.NearPlaneDistance; }
            set
            {
                pov.NearPlaneDistance = value;
                Invalidate();
            }
        }

        /// <summary>
        ///  遠くのビュー プレーンとの距離。
        /// </summary>
        public float FarPlaneDistance
        {
            get { return pov.FarPlaneDistance; }
            set
            {
                pov.FarPlaneDistance = value;
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
        Matrix lastModelOrientation = Matrix.Identity;
        Matrix modelOrientation = Matrix.Identity;

        bool isModelRotation;
        bool isPovTranslation;
        System.Drawing.Point dragOffset;

        public override void LoadContent()
        {
            if (RuntimeContent == null) return;

            base.LoadContent();

            Model = RuntimeContent.LoadContent(ContentManager) as Model;
        }

        public override void UnloadContent()
        {
            Model = null;

            base.UnloadContent();
        }

        protected override void Initialize()
        {
            if (!DesignMode)
            {
                pov.AspectRatio = AspectRatio;
                grid = new Grid(GraphicsDevice);

                GraphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;
                GraphicsDevice.BlendState = BlendState.AlphaBlend;
            }
        }

        protected override void Draw()
        {
            var bgColor = BackColor.ToXnaColor();
            GraphicsDevice.Clear(bgColor);

            // POV を更新します。
            pov.Update();

            // グリッドを描画します。
            if (gridVisible)
            {
                DrawGrid();
            }

            // Model を描画します。
            if (Model != null)
            {
                DrawModel();
            }
        }

        void DrawGrid()
        {
            grid.View = pov.View;
            grid.Projection = pov.Projection;
            grid.Draw();
        }

        void DrawModel()
        {
            foreach (var mesh in Model.Meshes)
            {
                foreach (var effect in mesh.Effects)
                {
                    var effectMatrices = effect as IEffectMatrices;
                    if (effectMatrices != null)
                    {
                        effectMatrices.World = modelOrientation;
                        effectMatrices.View = pov.View;
                        effectMatrices.Projection = pov.Projection;
                    }

                    var effectLights = effect as IEffectLights;
                    if (effectLights != null)
                    {
                        effectLights.EnableDefaultLighting();
                        if (lightingEnabled)
                        {
                            effectLights.LightingEnabled = true;
                        }
                        else
                        {
                            effectLights.LightingEnabled = false;
                        }
                    }

                    var basicEffect = effect as BasicEffect;
                    if (basicEffect != null)
                    {
                        if (lightingEnabled)
                        {
                            basicEffect.PreferPerPixelLighting = true;
                        }
                        else
                        {
                            basicEffect.PreferPerPixelLighting = true;
                        }
                    }
                }

                mesh.Draw();
            }
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                isModelRotation = true;
                dragOffset = e.Location;
                lastModelOrientation = modelOrientation;
            }
            else if (e.Button == MouseButtons.Right)
            {
                isPovTranslation = true;
                dragOffset = e.Location;
                pov.GetPosition(out lastPovPosition);
            }

            base.OnMouseDown(e);
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                isModelRotation = false;
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

            if (isModelRotation)
            {
                RotateModel(e);
            }
            else if (isPovTranslation)
            {
                TranslatePov(e);
            }

            base.OnMouseMove(e);
        }

        void RotateModel(MouseEventArgs e)
        {
            var point = e.Location;
            var dx = (float) (point.X - dragOffset.X);
            var dy = (float) (point.Y - dragOffset.Y);
            dx *= dragModelRotationScale;
            dy *= dragModelRotationScale;

            Matrix rotationY;
            Matrix.CreateRotationY(dx, out rotationY);
            Matrix rotationX;
            Matrix.CreateRotationX(dy, out rotationX);
            Matrix rotation;
            Matrix.Multiply(ref rotationY, ref rotationX, out rotation);

            Matrix.Multiply(ref lastModelOrientation, ref rotation, out modelOrientation);

            Invalidate();
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
            pov.SetPosition(ref position);

            Invalidate();
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            var wheel = (float) e.Delta;
            wheel *= wheelZoomScale;

            Vector3 position = Vector3.Zero;
            pov.GetPosition(out position);
            position.Z = Math.Max(minZoomDistance, position.Z + wheel);
            pov.SetPosition(ref position);

            Invalidate();

            base.OnMouseWheel(e);
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            pov.AspectRatio = AspectRatio;

            base.OnSizeChanged(e);
        }
    }
}
