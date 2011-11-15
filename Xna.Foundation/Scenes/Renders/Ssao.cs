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
    public sealed class Ssao : RenderComponent
    {
        #region Inner classes

        public sealed class SsaoEffect : Effect
        {
            #region Fields

            EffectParameter shadowColorParameter;
            EffectParameter ssaoMapParameter;
            EffectParameter sceneMapParameter;

            #endregion

            #region Accessors

            public void SetShadowColor(ref Vector3 value)
            {
                shadowColorParameter.SetValue(value);
            }

            public void SetSsaoMap(Texture2D value)
            {
                ssaoMapParameter.SetValue(value);
            }

            public void SetSceneMap(Texture2D value)
            {
                sceneMapParameter.SetValue(value);
            }

            #endregion

            #region Constructors

            public SsaoEffect(Effect cloneSource)
                : base(cloneSource)
            {
                shadowColorParameter = Parameters["ShadowColor"];
                ssaoMapParameter = Parameters["SsaoMap"];
                sceneMapParameter = Parameters["SceneMap"];
            }

            #endregion
        }

        public sealed class SsaoMapEffect : Effect
        {
            EffectParameter totalStrengthParameter;
            float totalStrength;
            public void SetTotalStrength(float value)
            {
                if (totalStrength != value)
                {
                    totalStrength = value;
                    totalStrengthParameter.SetValue(value);
                }
            }

            EffectParameter strengthParameter;
            float strength;
            public void SetStrength(float value)
            {
                if (strength != value)
                {
                    strength = value;
                    strengthParameter.SetValue(value);
                }
            }

            EffectParameter randomOffsetParameter;
            float randomOffset;
            public void SetRandomOffset(float value)
            {
                if (randomOffset != value)
                {
                    randomOffset = value;
                    randomOffsetParameter.SetValue(value);
                }
            }

            EffectParameter falloffParameter;
            float falloff;
            public void SetFalloff(float value)
            {
                if (falloff != value)
                {
                    falloff = value;
                    falloffParameter.SetValue(value);
                }
            }

            EffectParameter radiusParameter;
            float radius;
            public void SetRadius(float value)
            {
                if (radius != value)
                {
                    radius = value;
                    radiusParameter.SetValue(value);
                }
            }

            EffectParameter randomNormalMapParameter;
            public void SetRandomNormalMap(Texture2D value)
            {
                randomNormalMapParameter.SetValue(value);
            }

            EffectParameter normalDepthMapParameter;
            public void SetNormalDepthMap(Texture2D value)
            {
                normalDepthMapParameter.SetValue(value);
            }

            public SsaoMapEffect(Effect cloneSource)
                : base(cloneSource)
            {
                totalStrengthParameter = Parameters["TotalStrength"];
                strengthParameter = Parameters["Strength"];
                randomOffsetParameter = Parameters["RandomOffset"];
                falloffParameter = Parameters["Falloff"];
                radiusParameter = Parameters["Radius"];

                randomNormalMapParameter = Parameters["RandomNormalMap"];
                normalDepthMapParameter = Parameters["NormalDepthMap"];

                totalStrength = 2.0f;
                strength = 1.0f;
                randomOffset = 18.0f;
                falloff = 0.000002f;
                radius = 1.0f;
            }
        }

        public sealed class SsaoMapBlurEffect : Effect
        {
            public EffectTechnique HorizontalBlurTechnique { get; private set; }
            public EffectTechnique VerticalBlurTechnique { get; private set; }

            EffectParameter kernelSizeParameter;
            EffectParameter weightsParameter;
            EffectParameter offsetHParameter;
            EffectParameter offsetVParameter;
            EffectParameter ssaoMapParameter;

            int width;
            int height;
            int radius;
            float amount;

            public void SetSsaoMap(Texture2D value)
            {
                ssaoMapParameter.SetValue(value);
            }

            public SsaoMapBlurEffect(Effect cloneSource)
                : base(cloneSource)
            {
                kernelSizeParameter = Parameters["KernelSize"];
                weightsParameter = Parameters["Weights"];
                offsetHParameter = Parameters["OffsetsH"];
                offsetVParameter = Parameters["OffsetsV"];
                ssaoMapParameter = Parameters["SsaoMap"];

                HorizontalBlurTechnique = Techniques["HorizontalBlur"];
                VerticalBlurTechnique = Techniques["VerticalBlur"];
            }

            public void Configure(
                int width,
                int height,
                int radius,
                float amount)
            {
                if (width <= 0)
                {
                    throw new ArgumentOutOfRangeException(
                        "width",
                        "Width must be a positive value.");
                }
                if (height <= 0)
                {
                    throw new ArgumentOutOfRangeException(
                        "height",
                        "Height must be a positive value.");
                }
                if (radius <= 0)
                {
                    throw new ArgumentOutOfRangeException(
                        "radius",
                        "Radius must be a positive value.");
                }
                if (amount <= 0)
                {
                    throw new ArgumentOutOfRangeException(
                        "amount",
                        "Amount must be a positive value.");
                }

                bool changed = false;

                if (this.width != width)
                {
                    this.width = width;
                    changed = true;
                }
                if (this.height != height)
                {
                    this.height = height;
                    changed = true;
                }
                if (this.radius != radius)
                {
                    this.radius = radius;
                    changed = true;
                }
                if (this.amount != amount)
                {
                    this.amount = amount;
                    changed = true;
                }

                if (changed)
                {
                    kernelSizeParameter.SetValue(radius * 2 + 1);
                    PopulateWeights();
                    PopulateOffsetsH();
                    PopulateOffsetsV();
                }
            }

            /// <summary>
            /// Calculates the kernel and populates them into the shader. 
            /// </summary>
            void PopulateWeights()
            {
                var weights = new float[radius * 2 + 1];
                var totalWeight = 0.0f;
                var sigma = radius / amount;

                int index = 0;
                for (int i = -radius; i <= radius; i++)
                {
                    weights[index] = CalculateGaussian(sigma, i);
                    totalWeight += weights[index];
                    index++;
                }

                // Normalize
                for (int i = 0; i < weights.Length; i++)
                {
                    weights[i] /= totalWeight;
                }

                weightsParameter.SetValue(weights);
            }

            float CalculateGaussian(float sigma, float n)
            {
                var twoSigmaSquare = 2.0f * sigma * sigma;
                //
                // REFERENCE: (float) Math.Sqrt(2.0f * Math.PI * sigma * sigma)
                //
                var sigmaRoot = (float) Math.Sqrt(Math.PI * twoSigmaSquare);

                return (float) Math.Exp(-(n * n) / twoSigmaSquare) / sigmaRoot;
            }

            void PopulateOffsetsH()
            {
                offsetHParameter.SetValue(CalculateOffsets(1.0f / (float) width, 0));
            }

            void PopulateOffsetsV()
            {
                offsetVParameter.SetValue(CalculateOffsets(0, 1.0f / (float) height));
            }

            Vector2[] CalculateOffsets(float dx, float dy)
            {
                var offsets = new Vector2[radius * 2 + 1];

                int index = 0;
                for (int i = -radius; i <= radius; i++)
                {
                    offsets[index] = new Vector2(i * dx, i * dy);
                    index++;
                }

                return offsets;
            }
        }

        #endregion

        #region Fields and Properties

        NormalDepthMapEffect normalDepthMapEffect;
        SsaoMapEffect ssaoMapEffect;
        SsaoMapBlurEffect ssaoMapBlurEffect;
        SsaoEffect ssaoEffect;
        BackBuffer normalDepthMapBackBuffer;
        BackBuffer ssaoMapBackBuffer;
        BackBuffer ssaoMapBlurBackBuffer;

        SurfaceFormat normalDepthMapFormat = SurfaceFormat.Vector4;
        public SurfaceFormat NormalDepthMapFormat
        {
            get { return normalDepthMapFormat; }
            set
            {
                if (normalDepthMapFormat != value)
                {
                    normalDepthMapFormat = value;
                }
            }
        }

        public SsaoSettings Settings = SsaoSettings.Default;

        BoundingFrustum cameraFrustum = new BoundingFrustum(Matrix.Identity);

        #endregion

        #region Constructors

        public Ssao(IRenderContext renderContext)
            : base(renderContext)
        {
        }

        #endregion

        public void PrepareNormalDepthMap(GameTime gameTime)
        {
            AssertEnabled();

            GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            GraphicsDevice.BlendState = BlendState.Opaque;

            #region Prepare the reference camera

            var pov = Scene.ActiveCamera.Pov;
            Matrix projection;
            Matrix.CreatePerspectiveFieldOfView(
                pov.Fov, pov.AspectRatio, pov.NearPlaneDistance, Settings.FarPlaneDistance, out projection);
            cameraFrustum.Matrix = pov.View * projection;

            #endregion

            #region Prepare effects

            normalDepthMapEffect.View = pov.View;
            normalDepthMapEffect.Projection = projection;

            #endregion

            #region Prepare back buffers

            var pp = GraphicsDevice.PresentationParameters;
            normalDepthMapBackBuffer.Width = (int) (pp.BackBufferWidth * Settings.MapScale);
            normalDepthMapBackBuffer.Height = (int) (pp.BackBufferHeight * Settings.MapScale);
            normalDepthMapBackBuffer.SurfaceFormat = normalDepthMapFormat;

            #endregion

            #region Draw

            normalDepthMapBackBuffer.Begin();
            {
                GraphicsDevice.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.White, 1.0f, 0);

                RenderContext.VisibleTerrains.ForEach(
                    actor => actor.ActorModel.Draw(gameTime, normalDepthMapEffect));

                var cullingDistanceSquared =
                    Settings.FarPlaneDistance * Settings.FarPlaneDistance;

                Vector3 povPosition;
                pov.GetPosition(out povPosition);

                foreach (var actor in RenderContext.VisibleStaticMeshes)
                {
                    var cameraToCenter = povPosition - actor.Position;
                    var distanceSquared = cameraToCenter.LengthSquared();

                    if (distanceSquared <= cullingDistanceSquared)
                    {
                        if (cameraFrustum.Intersects(actor.ActorModel.WorldBoundingBox))
                        {
                            actor.ActorModel.Draw(gameTime, normalDepthMapEffect);
                        }
                    }
                }

                foreach (var actor in RenderContext.VisibleCharacters)
                {
                    var cameraToCenter = povPosition - actor.Position;
                    var distanceSquared = cameraToCenter.LengthSquared();

                    if (distanceSquared <= cullingDistanceSquared)
                    {
                        if (cameraFrustum.Intersects(actor.ActorModel.WorldBoundingBox))
                        {
                            actor.ActorModel.Draw(gameTime, normalDepthMapEffect);
                        }
                    }
                }
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

        public void PrepareSsaoMap(GameTime gameTime)
        {
            AssertEnabled();

            GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            GraphicsDevice.BlendState = BlendState.Opaque;

            #region Prepare effects

            ssaoMapEffect.SetTotalStrength(Settings.TotalStrength);
            ssaoMapEffect.SetStrength(Settings.Strength);
            ssaoMapEffect.SetFalloff(Settings.Falloff);
            ssaoMapEffect.SetRadius(Settings.Radius);

            #endregion

            #region Prepare back buffers

            var pp = GraphicsDevice.PresentationParameters;
            ssaoMapBackBuffer.Width = (int) (pp.BackBufferWidth * Settings.MapScale);
            ssaoMapBackBuffer.Height = (int) (pp.BackBufferHeight * Settings.MapScale);

            #endregion

            #region Draw

            ssaoMapBackBuffer.Begin();
            {
                GraphicsDevice.Clear(
                    ClearOptions.Target | ClearOptions.DepthBuffer,
                    Color.White,
                    1.0f,
                    0);
                SpriteBatch.Render(ssaoMapEffect, normalDepthMapBackBuffer.GetRenderTarget(), ssaoMapBackBuffer.Bounds);
            }
            ssaoMapBackBuffer.End();

            #endregion
        }

        public void PrepareBluredSsaoMap(GameTime gameTime)
        {
            AssertEnabled();

            ssaoMapBlurEffect.Configure(
                ssaoMapBackBuffer.Width,
                ssaoMapBackBuffer.Height,
                Settings.BlurRadius,
                Settings.BlurAmount);

            #region 1st pass

            //
            // NOTE: Depth test is necessary because of drawing by quad vertices
            //
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            GraphicsDevice.BlendState = BlendState.Opaque;

            ssaoMapBlurEffect.CurrentTechnique = ssaoMapBlurEffect.HorizontalBlurTechnique;
            ssaoMapBlurBackBuffer.Begin();
            {
                SpriteBatch.Render(ssaoMapBlurEffect, ssaoMapBackBuffer.GetRenderTarget(), ssaoMapBlurBackBuffer.Bounds);
            }
            ssaoMapBlurBackBuffer.End();

            #endregion

            #region 2nd pass

            ssaoMapBlurEffect.CurrentTechnique = ssaoMapBlurEffect.VerticalBlurTechnique;
            ssaoMapBackBuffer.Begin();
            {
                SpriteBatch.Render(ssaoMapBlurEffect, ssaoMapBlurBackBuffer.GetRenderTarget(), ssaoMapBackBuffer.Bounds);
            }
            ssaoMapBackBuffer.End();

            #endregion

            #region Notify intermadiate maps

            if (DebugMap.Instance != null)
            {
                DebugMap.Instance.Maps.Add(ssaoMapBackBuffer.GetRenderTarget());
            }

            #endregion
        }

        public void Filter(Texture2D source, BackBuffer destination)
        {
            AssertEnabled();

            #region Prepare effects

            ssaoEffect.SetSsaoMap(ssaoMapBackBuffer.GetRenderTarget());

            #endregion

            #region Draw

            GraphicsDevice.DepthStencilState = DepthStencilState.None;
            GraphicsDevice.BlendState = BlendState.Opaque;

            destination.Begin();
            {
                SpriteBatch.Render(ssaoEffect, source, destination.Bounds);
            }
            destination.End();

            #endregion
        }

        void AssertEnabled()
        {
            if (!Enabled)
            {
                throw new InvalidOperationException("SSAO is disabled.");
            }
        }

        #region LoadContent

        protected override void LoadContent()
        {
            #region Effects

            normalDepthMapEffect = EffectManager.Load<NormalDepthMapEffect>();
            ssaoMapEffect = EffectManager.Load<SsaoMapEffect>();
            ssaoMapBlurEffect = EffectManager.Load<SsaoMapBlurEffect>();
            ssaoEffect = EffectManager.Load<SsaoEffect>();

            var randomNormalMap = Content.Load<Texture2D>("Textures/RandomNormal");
            ssaoMapEffect.SetRandomNormalMap(randomNormalMap);

            #endregion

            #region Back buffers

            const int normalDepthMapSampleQuality = 0;

            var pp = GraphicsDevice.PresentationParameters;
            var width = (int) (pp.BackBufferWidth * Settings.MapScale);
            var height = (int) (pp.BackBufferHeight * Settings.MapScale);

            normalDepthMapBackBuffer = BackBufferManager.Load("NormalDepthmap");
            normalDepthMapBackBuffer.Width = width;
            normalDepthMapBackBuffer.Height = height;
            normalDepthMapBackBuffer.MipMap = false;
            normalDepthMapBackBuffer.SurfaceFormat = normalDepthMapFormat;
            normalDepthMapBackBuffer.DepthFormat = DepthFormat.Depth24Stencil8;
            normalDepthMapBackBuffer.MultiSampleCount = normalDepthMapSampleQuality;
            normalDepthMapBackBuffer.Enabled = Enabled;

            ssaoMapBackBuffer = BackBufferManager.Load("SsaoMap");
            ssaoMapBackBuffer.Width = width;
            ssaoMapBackBuffer.Height = height;
            ssaoMapBackBuffer.MipMap = false;
            ssaoMapBackBuffer.SurfaceFormat = SurfaceFormat.Color;
            ssaoMapBackBuffer.DepthFormat = DepthFormat.Depth24Stencil8;
            ssaoMapBackBuffer.MultiSampleCount = 0;
            ssaoMapBackBuffer.Enabled = Enabled;

            ssaoMapBlurBackBuffer = BackBufferManager.Load("SsaoGaussianBlur");
            ssaoMapBlurBackBuffer.Width = width;
            ssaoMapBlurBackBuffer.Height = height;
            ssaoMapBlurBackBuffer.MipMap = false;
            ssaoMapBlurBackBuffer.SurfaceFormat = SurfaceFormat.Color;
            ssaoMapBlurBackBuffer.DepthFormat = DepthFormat.Depth24Stencil8;
            ssaoMapBlurBackBuffer.MultiSampleCount = 0;
            ssaoMapBlurBackBuffer.Enabled = Enabled;

            #endregion

            base.LoadContent();
        }

        #endregion

        protected override void OnEnabledChanged()
        {
            if (ContentLoaded)
            {
                normalDepthMapBackBuffer.Enabled = Enabled;
                ssaoMapBackBuffer.Enabled = Enabled;
                ssaoMapBlurBackBuffer.Enabled = Enabled;
            }

            base.OnEnabledChanged();
        }
    }
}
