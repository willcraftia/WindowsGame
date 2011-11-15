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
using Willcraftia.Xna.Foundation.Debugs;
using Willcraftia.Xna.Foundation.Graphics;

#endregion

namespace Willcraftia.Xna.Foundation.Scenes.Renders
{
    public sealed class EdgeDetection : RenderComponent
    {
        #region Inner classes

        public sealed class EdgeDetectionEffect : Effect
        {
            float edgeWidth;
            float edgeIntensity;
            float normalThreshold;
            float depthThreshold;
            float normalSensitivity;
            float depthSensitivity;
            int mapWidth;
            int mapHeight;

            EffectParameter edgeIntensityParameter;
            EffectParameter normalThresholdParameter;
            EffectParameter depthThresholdParameter;
            EffectParameter normalSensitivityParameter;
            EffectParameter depthSensitivityParameter;
            EffectParameter edgeOffsetParameter;
            EffectParameter edgeColorParameter;
            EffectParameter normalDepthMapParameter;
            EffectParameter sceneMapParameter;

            public void SetEdgeWidth(float value)
            {
                if (edgeWidth != value)
                {
                    edgeWidth = value;
                    PopulateEdgeOffset();
                }
            }

            public void SetEdgeIntensity(float value)
            {
                edgeIntensityParameter.SetValue(value);
            }

            public void SetNormalThreshold(float value)
            {
                normalThresholdParameter.SetValue(value);
            }

            public void SetDepthThreshold(float value)
            {
                depthThresholdParameter.SetValue(value);
            }

            public void SetNormalSensitivity(float value)
            {
                normalSensitivityParameter.SetValue(value);
            }

            public void SetDepthSensitivity(float value)
            {
                depthSensitivityParameter.SetValue(value);
            }

            public void SetMapSize(int width, int height)
            {
                bool changed = this.mapWidth != width || this.mapHeight != height;

                this.mapWidth = width;
                this.mapHeight = height;

                if (changed)
                {
                    PopulateEdgeOffset();
                }
            }

            public void SetEdgeColor(ref Vector3 value)
            {
                edgeColorParameter.SetValue(value);
            }

            public void SetNormalDepthMap(Texture2D value)
            {
                normalDepthMapParameter.SetValue(value);
            }

            public void SetSceneMap(Texture2D value)
            {
                sceneMapParameter.SetValue(value);
            }

            //public EdgeDetectionEffect(GraphicsDevice GraphicsDevice, Effect cloneSource,
            //    int sceneMapWidth, int sceneMapHeight)
            //    : this(GraphicsDevice, cloneSource,
            //    1.0f, 1.0f,
            //    0.1f, 0.1f,
            //    1.0f, 10.0f,
            //    sceneMapWidth, sceneMapHeight)
            //{
            //    //
            //    // original (by xna)
            //    //
            //    // edgeWidth = 1.0
            //    // edgeIntensity = 1.0
            //    // normalThreshold = 0.5
            //    // depthThreshhold = 0.1
            //    // normalSensitivity = 1.0
            //    // depthSensitivity = 10.0
            //    //

            //    //
            //    // original (by me)
            //    //
            //    // edgeWidth = 1.0
            //    // edgeIntensity = 1.0
            //    // normalThreshold = 0.1
            //    // depthThreshhold = 0.1
            //    // normalSensitivity = 1.0
            //    // depthSensitivity = 10.0
            //    //
            //}

            public EdgeDetectionEffect(Effect cloneSource)
                : base(cloneSource)
            {
                edgeOffsetParameter = Parameters["EdgeOffset"];
                edgeIntensityParameter = Parameters["EdgeIntensity"];
                normalThresholdParameter = Parameters["NormalThreshold"];
                depthThresholdParameter = Parameters["DepthThreshold"];
                normalSensitivityParameter = Parameters["NormalSensitivity"];
                depthSensitivityParameter = Parameters["DepthSensitivity"];

                edgeColorParameter = Parameters["EdgeColor"];
                sceneMapParameter = Parameters["SceneMap"];
                normalDepthMapParameter = Parameters["NormalDepthMap"];

                edgeWidth = 1.0f;
                edgeIntensity = 1.0f;
                normalThreshold = 0.1f;
                depthThreshold = 0.1f;
                normalSensitivity = 1.0f;
                depthSensitivity = 10.0f;
                mapWidth = 1;
                mapHeight = 1;

                PopulateEdgeOffset();
                SetEdgeIntensity(edgeIntensity);
                SetNormalThreshold(normalThreshold);
                SetDepthThreshold(depthThreshold);
                SetNormalSensitivity(normalSensitivity);
                SetDepthSensitivity(depthSensitivity);
            }

            void PopulateEdgeOffset()
            {
                var edgeOffset = new Vector2(edgeWidth, edgeWidth);
                edgeOffset.X /= (float) mapWidth;
                edgeOffset.Y /= (float) mapHeight;
                edgeOffsetParameter.SetValue(edgeOffset);
            }
        }

        #endregion

        #region Fields and Properties

        public const SurfaceFormat DefaultNormalDepthMapFormat = SurfaceFormat.Vector4;

        NormalDepthMapEffect normalDepthMapEffect;
        EdgeDetectionEffect edgeDetectionEffect;

        BackBuffer normalDepthMapBackBuffer;

        SurfaceFormat normalDepthMapFormat;
        public SurfaceFormat NormalDepthMapFormat
        {
            get { return normalDepthMapFormat; }
            set
            {
                if (normalDepthMapFormat != value)
                {
                    normalDepthMapFormat = value;
                    if (ContentLoaded)
                    {
                        normalDepthMapBackBuffer.SurfaceFormat = value;
                    }
                }
            }
        }

        public EdgeDetectionSettings Settings;

        #endregion

        #region Constructors

        public EdgeDetection(IRenderContext renderContext)
            : base(renderContext)
        {
            normalDepthMapFormat = DefaultNormalDepthMapFormat;
            Settings = EdgeDetectionSettings.Default;
        }

        #endregion

        public void PrepareNormalDepthMap(GameTime gameTime)
        {
            if (!Enabled)
            {
                throw new InvalidOperationException("Edge detection is disabled");
            }

            #region Prepare the reference camera

            var pov = Scene.ActiveCamera.Pov;
            Matrix projection;
            Matrix.CreatePerspectiveFieldOfView(
                pov.Fov, pov.AspectRatio, pov.NearPlaneDistance, Settings.FarPlaneDistance, out projection);

            #endregion

            #region Prepare effects

            normalDepthMapEffect.View = pov.View;
            normalDepthMapEffect.Projection = projection;

            #endregion

            #region Back buffers

            var pp = GraphicsDevice.PresentationParameters;
            normalDepthMapBackBuffer.Width = (int) (pp.BackBufferWidth * Settings.MapScale);
            normalDepthMapBackBuffer.Height = (int) (pp.BackBufferHeight * Settings.MapScale);

            #endregion

            #region Draw

            GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            GraphicsDevice.BlendState = BlendState.Opaque;

            normalDepthMapBackBuffer.Begin();
            {
                GraphicsDevice.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.White, 1.0f, 0);

                // NOTE: Sky domes never join into the edge detection.

                RenderContext.VisibleFluidSurfaces.ForEach(
                    actor => actor.ActorModel.Draw(gameTime, normalDepthMapEffect));
                RenderContext.VisibleTerrains.ForEach(
                    actor => actor.ActorModel.Draw(gameTime, normalDepthMapEffect));
                RenderContext.VisibleStaticMeshes.ForEach(
                    actor => actor.ActorModel.Draw(gameTime, normalDepthMapEffect));
                RenderContext.VisibleCharacters.ForEach(
                    actor => actor.ActorModel.Draw(gameTime, normalDepthMapEffect));
            }
            normalDepthMapBackBuffer.End();

            #endregion

            #region Notify intermadiate maps

            if (DebugMap.Instance != null)
            {
                DebugMap.Instance.Maps.Add(normalDepthMapBackBuffer.GetRenderTarget());
            }

            #endregion
        }

        public void Filter(Texture2D source, BackBuffer destination)
        {
            if (!Enabled)
            {
                throw new InvalidOperationException("Depth of field disabled");
            }

            #region Prepare effects

            var pp = GraphicsDevice.PresentationParameters;
            var width = (int) (pp.BackBufferWidth * Settings.MapScale);
            var height = (int) (pp.BackBufferHeight * Settings.MapScale);
            edgeDetectionEffect.SetEdgeWidth(Settings.EdgeWidth);
            edgeDetectionEffect.SetMapSize(width, height);
            edgeDetectionEffect.SetNormalDepthMap(normalDepthMapBackBuffer.GetRenderTarget());
            edgeDetectionEffect.SetEdgeIntensity(Settings.EdgeIntensity);
            edgeDetectionEffect.SetNormalThreshold(Settings.NormalThreshold);
            edgeDetectionEffect.SetDepthThreshold(Settings.DepthThreshold);
            edgeDetectionEffect.SetNormalSensitivity(Settings.NormalSensitivity);
            edgeDetectionEffect.SetDepthSensitivity(Settings.DepthSensitivity);
            edgeDetectionEffect.SetEdgeColor(ref Settings.EdgeColor);

            #endregion

            #region Draw

            GraphicsDevice.DepthStencilState = DepthStencilState.None;
            GraphicsDevice.BlendState = BlendState.Opaque;

            destination.Begin();
            {
                SpriteBatch.Render(edgeDetectionEffect, source, destination.Bounds);
            }
            destination.End();

            #endregion
        }

        #region LoadContent

        protected override void LoadContent()
        {
            var pp = GraphicsDevice.PresentationParameters;
            var width = (int) (pp.BackBufferWidth * Settings.MapScale);
            var height = (int) (pp.BackBufferHeight * Settings.MapScale);

            #region Effects

            normalDepthMapEffect = EffectManager.Load<NormalDepthMapEffect>();
            edgeDetectionEffect = EffectManager.Load<EdgeDetectionEffect>();
            edgeDetectionEffect.SetEdgeWidth(Settings.EdgeWidth);
            edgeDetectionEffect.SetMapSize(width, height);

            #endregion

            #region Back buffers

            normalDepthMapBackBuffer = BackBufferManager.Load("NormalDepthMap");
            normalDepthMapBackBuffer.Width = width;
            normalDepthMapBackBuffer.Height = height;
            normalDepthMapBackBuffer.MipMap = false;
            normalDepthMapBackBuffer.SurfaceFormat = normalDepthMapFormat;
            normalDepthMapBackBuffer.MultiSampleCount = 0;
            normalDepthMapBackBuffer.Enabled = Enabled;

            #endregion

            base.LoadContent();
        }

        #endregion

        protected override void OnEnabledChanged()
        {
            if (ContentLoaded)
            {
                normalDepthMapBackBuffer.Enabled = Enabled;
            }

            base.OnEnabledChanged();
        }
    }
}
