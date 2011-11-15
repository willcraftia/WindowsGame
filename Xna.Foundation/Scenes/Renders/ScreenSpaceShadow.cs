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
using Willcraftia.Xna.Foundation.Graphics;

#endregion

namespace Willcraftia.Xna.Foundation.Scenes.Renders
{
    public abstract class ScreenSpaceShadow : RenderComponent
    {
        #region Inner classes

        public sealed class ScreenSpaceShadowEffect : Effect
        {
            #region Fields

            public EffectParameter ShadowColorParameter;
            public EffectParameter ShadowSceneMapParameter;
            public EffectParameter SceneMapParameter;

            #endregion

            #region Constructors

            public ScreenSpaceShadowEffect(Effect cloneSource)
                : base(cloneSource)
            {
                ShadowColorParameter = Parameters["ShadowColor"];
                ShadowSceneMapParameter = Parameters["ShadowSceneMap"];
                SceneMapParameter = Parameters["SceneMap"];
            }

            #endregion
        }

        #endregion

        #region Fields and Properties

        protected ShadowSettings ShadowSettings
        {
            get { return Scene.SceneSettings.ShadowSettings; }
        }

        BackBuffer shadowSceneMapBackBuffer;
        protected BackBuffer ShadowSceneMapBackBuffer
        {
            get { return shadowSceneMapBackBuffer; }
        }

        ScreenSpaceShadowEffect screenSpaceShadowEffect;
        GaussianBlur screenSpaceBlur;

        Texture2D shadowSceneMap;
        protected Texture2D ShadowSceneMap
        {
            get { return shadowSceneMap; }
            set { shadowSceneMap = value; }
        }

        #endregion

        #region Constructors

        public ScreenSpaceShadow(IRenderContext renderContext)
            : base(renderContext)
        {
        }

        #endregion

        #region Abstract methods

        public abstract void PrepareShadowMap(GameTime gameTime);
        public abstract void PrepareShadowSceneMap(GameTime gameTime);

        #endregion

        protected void BlurShadowSceneMap()
        {
            screenSpaceBlur.Enabled = ShadowSettings.ScreenSpaceShadow.BlurEnabled;
            screenSpaceBlur.Radius = ShadowSettings.ScreenSpaceShadow.BlurRadius;
            screenSpaceBlur.Amount = ShadowSettings.ScreenSpaceShadow.BlurAmount;
            if (screenSpaceBlur.Enabled)
            {
                screenSpaceBlur.Filter(ShadowSceneMapBackBuffer.GetRenderTarget(), ShadowSceneMapBackBuffer);
            }
        }

        protected virtual void EnableBackBuffers(bool enabled)
        {
            ShadowSceneMapBackBuffer.Enabled = enabled;
            screenSpaceBlur.Enabled = enabled && ShadowSettings.ScreenSpaceShadow.BlurEnabled;
        }

        public void Filter(Texture2D source, BackBuffer destination)
        {
            if (!Enabled)
            {
                throw new InvalidOperationException("Screen space shadow is disabled.");
            }

            #region Prepare effects

            screenSpaceShadowEffect.ShadowSceneMapParameter.SetValue(shadowSceneMap);
            screenSpaceShadowEffect.ShadowColorParameter.SetValue(Scene.SceneSettings.DirectionalLight0.ShadowColor);

            #endregion

            #region Draw

            GraphicsDevice.DepthStencilState = DepthStencilState.None;
            GraphicsDevice.BlendState = BlendState.Opaque;

            destination.Begin();
            {
                SpriteBatch.Render(screenSpaceShadowEffect, source, destination.Bounds);
            }
            destination.End();

            #endregion
        }

        #region LoadContent/UnloadContent

        protected override void LoadContent()
        {
            #region Effects

            screenSpaceShadowEffect = EffectManager.Load<ScreenSpaceShadowEffect>();

            #endregion

            #region Back buffers

            var pp = GraphicsDevice.PresentationParameters;
            var mapScale = ShadowSettings.ScreenSpaceShadow.MapScale;
            var width = (int) (pp.BackBufferWidth * mapScale);
            var height = (int) (pp.BackBufferHeight * mapScale);
            var format = SurfaceFormat.Color;

            shadowSceneMapBackBuffer = BackBufferManager.Load("ShadowSceneMap");
            shadowSceneMapBackBuffer.Width = width;
            shadowSceneMapBackBuffer.Height = height;
            shadowSceneMapBackBuffer.MipMap = false;
            shadowSceneMapBackBuffer.SurfaceFormat = format;
            shadowSceneMapBackBuffer.DepthFormat = DepthFormat.Depth24Stencil8;
            shadowSceneMapBackBuffer.MultiSampleCount = 0;
            shadowSceneMapBackBuffer.Enabled = false;

            #endregion

            #region Blur

            screenSpaceBlur = new GaussianBlur(RenderContext, width, height, format);
            screenSpaceBlur.LoadContent();

            #endregion

            base.LoadContent();
        }

        #endregion

        protected override void OnEnabledChanged()
        {
            if (ContentLoaded)
            {
                EnableBackBuffers(Enabled);
            }

            base.OnEnabledChanged();
        }
    }
}
