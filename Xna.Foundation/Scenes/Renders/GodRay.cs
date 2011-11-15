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
    public sealed class GodRay : RenderComponent
    {
        #region Inner classes

        public sealed class GodRayEffect : Effect
        {
            EffectParameter viewParameter;
            EffectParameter viewProjectionParameter;
            EffectParameter lightDirectionParameter;
            EffectParameter cameraPositionParameter;
            EffectParameter sceneMapParameter;
            EffectParameter occlusionMapParameter;

            EffectParameter exposureParameter;
            EffectParameter sampleCountParameter;
            EffectParameter sampleDensityInverseParameter;
            EffectParameter illuminationDecaysParameter;

            public GodRayEffect(Effect cloneSource)
                : base(cloneSource)
            {
                viewParameter = Parameters["View"];
                viewProjectionParameter = Parameters["ViewProjection"];
                lightDirectionParameter = Parameters["LightDirection"];
                cameraPositionParameter = Parameters["CameraPosition"];
                sceneMapParameter = Parameters["SceneMap"];
                occlusionMapParameter = Parameters["OcclusionMap"];

                exposureParameter = Parameters["Exposure"];
                sampleCountParameter = Parameters["SampleCount"];
                sampleDensityInverseParameter = Parameters["SampleDensityInverse"];
                illuminationDecaysParameter = Parameters["IlluminationDecays"];
            }

            public void SetView(ref Matrix value)
            {
                viewParameter.SetValue(value);
            }

            public void SetViewProjection(ref Matrix value)
            {
                viewProjectionParameter.SetValue(value);
            }

            public void SetLightDirection(ref Vector3 value)
            {
                lightDirectionParameter.SetValue(value);
            }

            public void SetCameraPosition(ref Vector3 value)
            {
                cameraPositionParameter.SetValue(value);
            }

            public void SetSceneMap(Texture2D value)
            {
                sceneMapParameter.SetValue(value);
            }

            public void SetOcclusionMap(Texture2D value)
            {
                occlusionMapParameter.SetValue(value);
            }

            public void SetExposure(float value)
            {
                exposureParameter.SetValue(value);
            }

            public void SetSampleCount(int value)
            {
                sampleCountParameter.SetValue(value);
            }

            public void SetSampleDensityInverse(float value)
            {
                sampleDensityInverseParameter.SetValue(value);
            }

            public void SetIlluminationDecays(float[] value)
            {
                illuminationDecaysParameter.SetValue(value);
            }
        }

        public sealed class GodRayOcclusionMapEffect : Effect, IEffectMatrices
        {
            #region IEffectMatrices

            EffectParameter projection;
            public Matrix Projection
            {
                get { return projection.GetValueMatrix(); }
                set { projection.SetValue(value); }
            }

            EffectParameter view;
            public Matrix View
            {
                get { return view.GetValueMatrix(); }
                set { view.SetValue(value); }
            }

            EffectParameter world;
            public Matrix World
            {
                get { return world.GetValueMatrix(); }
                set { world.SetValue(value); }
            }

            #endregion

            public GodRayOcclusionMapEffect(Effect cloneSource)
                : base(cloneSource)
            {
                world = Parameters["World"];
                view = Parameters["View"];
                projection = Parameters["Projection"];
            }
        }

        #endregion

        #region Fields and Properties

        public const int MaxSampleCount = 2;

        GodRayOcclusionMapEffect occlusionMapEffect;
        GodRayEffect godRayEffect;

        BackBuffer occlusionMapBackBuffer;

        public GodRaySettings Settings;
        public bool OcclusionMapEnabled;

        float[] illuminationDecays;

        #endregion

        #region Constructors

        public GodRay(IRenderContext renderContext)
            : base(renderContext)
        {
            Settings = GodRaySettings.Default;
            OcclusionMapEnabled = true;
            illuminationDecays = new float[MaxSampleCount];
        }

        #endregion

        public void PrepareOcclusionMap(GameTime gameTime)
        {
            if (!Enabled)
            {
                throw new InvalidOperationException("God ray disabled");
            }

            if (!OcclusionMapEnabled)
            {
                return;
            }

            #region Prepare effects

            var pov = Scene.ActiveCamera.Pov;
            occlusionMapEffect.View = pov.View;
            occlusionMapEffect.Projection = pov.Projection;

            #endregion

            #region Prepare back buffers

            var pp = GraphicsDevice.PresentationParameters;
            occlusionMapBackBuffer.Width = (int) (pp.BackBufferWidth * Settings.MapScale);
            occlusionMapBackBuffer.Height = (int) (pp.BackBufferHeight * Settings.MapScale);

            #endregion

            #region Draw

            GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            GraphicsDevice.BlendState = BlendState.Opaque;

            occlusionMapBackBuffer.Begin();
            {
                GraphicsDevice.Clear(
                    ClearOptions.Target | ClearOptions.DepthBuffer,
                    Color.White,
                    1.0f,
                    0);

                RenderContext.VisibleTerrains.ForEach(
                    actor => actor.ActorModel.Draw(gameTime, occlusionMapEffect));
                RenderContext.VisibleStaticMeshes.ForEach(
                    actor => actor.ActorModel.Draw(gameTime, occlusionMapEffect));
                RenderContext.VisibleCharacters.ForEach(
                    actor => actor.ActorModel.Draw(gameTime, occlusionMapEffect));
            }
            occlusionMapBackBuffer.End();

            #endregion

            #region Nofity intermadiate maps

            if (DebugMap.Instance != null)
            {
                DebugMap.Instance.Maps.Add(occlusionMapBackBuffer.GetRenderTarget());
            }

            #endregion
        }

        public void Filter(Texture2D source, RenderTarget2D destination)
        {
            if (!Enabled)
            {
                throw new InvalidOperationException("God ray disabled");
            }

            #region Prepare effects

            var pov = Scene.ActiveCamera.Pov;
            Vector3 povPosition;
            pov.GetPosition(out povPosition);

            godRayEffect.SetView(ref pov.View);
            godRayEffect.SetViewProjection(ref pov.ViewProjection);
            godRayEffect.SetLightDirection(ref Scene.SceneSettings.DirectionalLight0.Direction);
            godRayEffect.SetCameraPosition(ref povPosition);
            
            if (OcclusionMapEnabled)
            {
                godRayEffect.SetOcclusionMap(occlusionMapBackBuffer.GetRenderTarget());
            }
            else
            {
                godRayEffect.SetOcclusionMap(source);
            }
            //godRayEffect.SetDensity(Settings.Density);
            //godRayEffect.SetWeight(Settings.Weight);
            //godRayEffect.SetDecay(Settings.Decay);
            godRayEffect.SetExposure(Settings.Exposure);
            godRayEffect.SetSampleCount(Settings.SampleCount);

            var sampleDensity = (float) Settings.SampleCount * Settings.Density;
            var sampleDensityInverse = 1.0f / sampleDensity;
            godRayEffect.SetSampleDensityInverse(sampleDensityInverse);

            float decay = 1.0f;
            for (int i = 0; i < Settings.SampleCount; i++)
            {
                illuminationDecays[i] = decay * Settings.Weight;
                decay *= Settings.Decay;
            }
            godRayEffect.SetIlluminationDecays(illuminationDecays);

            #endregion

            #region Draw

            GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            GraphicsDevice.BlendState = BlendState.Opaque;

            GraphicsDevice.SetRenderTarget(destination);
            {
                SpriteBatch.Render(godRayEffect, source, destination.Bounds);
            }
            GraphicsDevice.SetRenderTarget(null);

            #endregion
        }

        #region LoadContent

        protected override void LoadContent()
        {
            #region Effects

            occlusionMapEffect = EffectManager.Load<GodRayOcclusionMapEffect>();
            godRayEffect = EffectManager.Load<GodRayEffect>();

            #endregion

            #region Back buffers

            var pp = GraphicsDevice.PresentationParameters;
            occlusionMapBackBuffer = BackBufferManager.Load("GodyRayOcclusionMap");
            occlusionMapBackBuffer.Width = (int) (pp.BackBufferWidth * Settings.MapScale);
            occlusionMapBackBuffer.Height = (int) (pp.BackBufferHeight * Settings.MapScale);
            occlusionMapBackBuffer.MipMap = true;
            occlusionMapBackBuffer.SurfaceFormat = SurfaceFormat.Color;
            occlusionMapBackBuffer.MultiSampleCount = 0;
            occlusionMapBackBuffer.Enabled = Enabled;

            #endregion

            base.LoadContent();
        }

        #endregion

        protected override void OnEnabledChanged()
        {
            if (ContentLoaded)
            {
                occlusionMapBackBuffer.Enabled = Enabled;
            }

            base.OnEnabledChanged();
        }
    }
}
