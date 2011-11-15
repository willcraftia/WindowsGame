#region Using

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Willcraftia.Xna.Foundation.Graphics;

#endregion

namespace Willcraftia.Xna.Foundation.Scenes.Renders
{
    public sealed class GaussianBlur : IDisposable
    {
        #region Inner classes

        public sealed class GaussianBlurEffect : Effect
        {
            public EffectTechnique HorizontalBlurTechnique { get; private set; }
            public EffectTechnique VerticalBlurTechnique { get; private set; }

            EffectParameter kernelSizeParameter;
            EffectParameter weightsParameter;
            EffectParameter offsetHParameter;
            EffectParameter offsetVParameter;
            EffectParameter colorMapParameter;

            int width;
            int height;
            int radius;
            float amount;

            public void SetColorMap(Texture2D value)
            {
                colorMapParameter.SetValue(value);
            }

            public GaussianBlurEffect(Effect cloneSource)
                : base(cloneSource)
            {
                kernelSizeParameter = Parameters["KernelSize"];
                weightsParameter = Parameters["Weights"];
                offsetHParameter = Parameters["OffsetsH"];
                offsetVParameter = Parameters["OffsetsV"];
                colorMapParameter = Parameters["ColorMap"];

                HorizontalBlurTechnique = Techniques["HorizontalBlur"];
                VerticalBlurTechnique = Techniques["VerticalBlur"];
            }

            public void Configure(int width, int height, int radius, float amount)
            {
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
                //
                // REFERENCE: sigmaRoot = (float) Math.Sqrt(2.0f * Math.PI * sigma * sigma)
                //
                var twoSigmaSquare = 2.0f * sigma * sigma;
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

        #region Gaussian blur
#if SHADER_3_0
        public const int MaxGaussianBlurRadius = 7;
#else
        public const int MaxGaussianBlurRadius = 4;
#endif
        public const int MinGaussianBlurRadius = 1;
        public const float MinGaussianBlurAmount = 0.001f;
        public const int DefaultGaussianBlurRadius = 1;
        public const float DefaultGaussianBlurAmount = 2.0f;
        #endregion

        IRenderContext renderContext;
        EffectManager effectManager;
        GaussianBlurEffect effect;

        BackBufferManager backBufferManager;
        BackBuffer backBuffer;

        int widthHint;
        int heightHint;
        SurfaceFormat formatHint;

        int radius;
        public int Radius
        {
            get { return radius; }
            set
            {
                radius = MathExtension.Clamp(value, MinGaussianBlurRadius, MaxGaussianBlurRadius);
            }
        }

        float amount;
        public float Amount
        {
            get { return amount; }
            set
            {
                amount = Math.Max(value, MinGaussianBlurAmount);
            }
        }

        bool enabled;
        public bool Enabled
        {
            get { return enabled; }
            set
            {
                if (enabled == value) return;

                enabled = value;

                if (contentLoaded)
                {
                    backBuffer.Enabled = value;
                }
            }
        }

        bool contentLoaded;

        public GaussianBlur(IRenderContext renderContext, int widthHint, int heightHint, SurfaceFormat formatHint)
        {
            this.renderContext = renderContext;
            this.widthHint = widthHint;
            this.heightHint = heightHint;
            this.formatHint = formatHint;

            radius = DefaultGaussianBlurRadius;
            amount = DefaultGaussianBlurAmount;

            enabled = false;
            contentLoaded = false;
        }

        public void Filter(Texture2D source, BackBuffer destination)
        {
            if (!enabled) throw new InvalidOperationException("Gaussian blur filter is disabled.");
            if (!contentLoaded) throw new InvalidOperationException("Content is not loaded.");

            AdjustContent(destination.Width, destination.Height, destination.SurfaceFormat);

            FilterByHorizontalBlur(source);
            FilterByVirticalBlur(backBuffer.GetRenderTarget(), destination);
        }

        void FilterByHorizontalBlur(Texture2D source)
        {
            effect.CurrentTechnique = effect.HorizontalBlurTechnique;
            backBuffer.Begin();
            {
                renderContext.SpriteBatch.Render(
                    effect,
                    source,
                    new Rectangle(0, 0, backBuffer.Width, backBuffer.Height));
            }
            backBuffer.End();
        }

        void FilterByVirticalBlur(Texture2D source, BackBuffer destination)
        {
            renderContext.GraphicsDevice.DepthStencilState = DepthStencilState.None;

            effect.CurrentTechnique = effect.VerticalBlurTechnique;
            destination.Begin();
            {
                renderContext.SpriteBatch.Render(
                    effect,
                    backBuffer.GetRenderTarget(),
                    new Rectangle(0, 0, destination.Width, destination.Height));
            }
            destination.End();
        }

        #region LoadContent

        public void LoadContent()
        {
            if (contentLoaded) return;

            effectManager = new EffectManager(renderContext.GraphicsDevice, renderContext.Content);

            effect = effectManager.Load<GaussianBlurEffect>();
            effect.Configure(widthHint, heightHint, radius, amount);

            backBufferManager = new BackBufferManager(renderContext.GraphicsDevice);

            backBuffer = backBufferManager.Load("GaussianBlur");
            backBuffer.MipMap = false;
            backBuffer.MultiSampleCount = 0;
            backBuffer.DepthFormat = DepthFormat.None;

            AdjustContent(widthHint, heightHint, formatHint);

            contentLoaded = true;
        }

        void AdjustContent(int width, int height, SurfaceFormat format)
        {
            effect.Configure(width, height, radius, amount);

            backBuffer.Width = width;
            backBuffer.Height = height;
            backBuffer.SurfaceFormat = format;
        }

        #endregion

        #region UnloadContent

        public void UnloadContent()
        {
            if (!contentLoaded) return;

            effectManager.Unload();
            backBufferManager.Unload();

            contentLoaded = false;
        }

        #endregion

        #region IDisposable

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        bool disposed;

        ~GaussianBlur()
        {
            Dispose(false);
        }

        void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    UnloadContent();
                    effectManager.Dispose();
                    backBufferManager.Dispose();
                }
                disposed = true;
            }
        }

        #endregion
    }
}
