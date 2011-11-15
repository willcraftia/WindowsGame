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
    public sealed class Bloom : RenderComponent
    {
        #region Inner classes

        public sealed class BloomExtractEffect : Effect
        {
            #region Fields and Properties

            EffectParameter thresholdParameter;
            float threshold;
            public void SetThreshold(float value)
            {
                if (threshold != value)
                {
                    threshold = value;
                    thresholdParameter.SetValue(value);
                }
            }

            #endregion

            #region Constructors

            public BloomExtractEffect(Effect cloneSource)
                : base(cloneSource)
            {
                thresholdParameter = Parameters["Threshold"];
            }

            #endregion
        }

        public sealed class BloomEffect : Effect
        {
            #region Fields and Properties

            public EffectParameter BloomExtractMapParameter;

            EffectParameter bloomIntensityParameter;
            float bloomIntensity;
            public void SetBloomIntensity(float value)
            {
                if (bloomIntensity != value)
                {
                    bloomIntensity = value;
                    bloomIntensityParameter.SetValue(value);
                }
            }

            EffectParameter baseIntensityParameter;
            float baseIntensity;
            public void SetBaseIntensity(float value)
            {
                if (baseIntensity != value)
                {
                    baseIntensity = value;
                    baseIntensityParameter.SetValue(value);
                }
            }

            EffectParameter bloomSaturationParameter;
            float bloomSaturation;
            public void SetBloomSaturation(float value)
            {
                if (bloomSaturation != value)
                {
                    bloomSaturation = value;
                    bloomSaturationParameter.SetValue(value);
                }
            }

            EffectParameter baseSaturationParameter;
            float baseSaturation;
            public void SetBaseSaturation(float value)
            {
                if (baseSaturation != value)
                {
                    baseSaturation = value;
                    baseSaturationParameter.SetValue(value);
                }
            }

            #endregion

            #region Constructors

            public BloomEffect(Effect cloneSource)
                : base(cloneSource)
            {
                BloomExtractMapParameter = Parameters["BloomExtractMap"];
                bloomIntensityParameter = Parameters["BloomIntensity"];
                baseIntensityParameter = Parameters["BaseIntensity"];
                bloomSaturationParameter = Parameters["BloomSaturation"];
                baseSaturationParameter = Parameters["BaseSaturation"];
            }

            #endregion
        }

        #endregion

        #region Fields and Properties

        BloomExtractEffect bloomExtractEffect;
        BloomEffect bloomEffect;

        BackBuffer bloomExtractMapBackBuffer;
        
        GaussianBlur gaussianBlur;

        public BloomSettings Settings = BloomSettings.Default;

        #endregion

        #region Constructors

        public Bloom(IRenderContext renderContext)
            : base(renderContext)
        {
        }

        #endregion

        public void Filter(Texture2D source, BackBuffer destination)
        {
            if (!Enabled) throw new InvalidOperationException("Bloom disabled.");

            #region Create bloom extract map

            #region Prepare effects

            bloomExtractEffect.SetThreshold(Settings.Threshold);

            #endregion

            #region Prepare back buffers

            var pp = GraphicsDevice.PresentationParameters;
            bloomExtractMapBackBuffer.Width = (int) (pp.BackBufferWidth * Settings.MapScale);
            bloomExtractMapBackBuffer.Height = (int) (pp.BackBufferHeight * Settings.MapScale);

            #endregion

            #region Draw

            GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            GraphicsDevice.BlendState = BlendState.Opaque;

            bloomExtractMapBackBuffer.Begin();
            {
                SpriteBatch.Render(bloomExtractEffect, source, bloomExtractMapBackBuffer.Bounds);
            }
            bloomExtractMapBackBuffer.End();

            #endregion

            #endregion

            #region Blur

            gaussianBlur.Radius = Settings.BlurRadius;
            gaussianBlur.Amount = Settings.BlurAmount;
            gaussianBlur.Filter(
                bloomExtractMapBackBuffer.GetRenderTarget(),
                bloomExtractMapBackBuffer);

            #endregion

            #region Notify intermediate maps

            if (DebugMap.Instance != null)
            {
                DebugMap.Instance.Maps.Add(bloomExtractMapBackBuffer.GetRenderTarget());
            }

            #endregion

            #region Apply bloom to the destination

            bloomEffect.BloomExtractMapParameter.SetValue(bloomExtractMapBackBuffer.GetRenderTarget());
            bloomEffect.SetBloomIntensity(Settings.BloomIntensity);
            bloomEffect.SetBaseIntensity(Settings.BaseIntensity);
            bloomEffect.SetBloomSaturation(Settings.BloomSaturation);
            bloomEffect.SetBaseSaturation(Settings.BaseSaturation);

            GraphicsDevice.DepthStencilState = DepthStencilState.None;
            GraphicsDevice.BlendState = BlendState.Opaque;

            destination.Begin();
            {
                SpriteBatch.Render(bloomEffect, source, destination.Bounds);
            }
            destination.End();

            #endregion
        }

        #region LoadContent

        protected override void LoadContent()
        {
            #region Effects

            bloomExtractEffect = EffectManager.Load<BloomExtractEffect>();
            bloomEffect = EffectManager.Load<BloomEffect>();

            #endregion

            #region Back buffers

            var pp = GraphicsDevice.PresentationParameters;
            var width = (int) (pp.BackBufferWidth * Settings.MapScale);
            var height = (int) (pp.BackBufferHeight * Settings.MapScale);

            bloomExtractMapBackBuffer = BackBufferManager.Load("BloomExtractMap");
            bloomExtractMapBackBuffer.Width = width;
            bloomExtractMapBackBuffer.Height = height;
            bloomExtractMapBackBuffer.MipMap = false;
            bloomExtractMapBackBuffer.SurfaceFormat = pp.BackBufferFormat;
            bloomExtractMapBackBuffer.DepthFormat = DepthFormat.Depth24Stencil8;
            bloomExtractMapBackBuffer.MultiSampleCount = 0;
            bloomExtractMapBackBuffer.Enabled = Enabled;

            #endregion

            #region Gaussian blur

            gaussianBlur = new GaussianBlur(RenderContext, width, height, bloomExtractMapBackBuffer.SurfaceFormat);
            gaussianBlur.LoadContent();
            gaussianBlur.Enabled = Enabled;

            #endregion

            base.LoadContent();
        }

        #endregion

        #region UnloadContent

        protected override void UnloadContent()
        {
            gaussianBlur.UnloadContent();

            base.UnloadContent();
        }

        #endregion

        protected override void OnEnabledChanged()
        {
            if (ContentLoaded)
            {
                bloomExtractMapBackBuffer.Enabled = Enabled;
                gaussianBlur.Enabled = Enabled;
            }

            base.OnEnabledChanged();
        }
    }
}
