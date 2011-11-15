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
    public sealed class SingleScreenSpaceShadow : ScreenSpaceShadow
    {
        #region Inner classes

        public abstract class ShadowSceneEffect : Effect, IEffectMatrices
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

            EffectParameter lightViewProjection;
            public Matrix LightViewProjection
            {
                get { return lightViewProjection.GetValueMatrix(); }
                set { lightViewProjection.SetValue(value); }
            }

            EffectParameter depthBias;
            public float DepthBias
            {
                get { return depthBias.GetValueSingle(); }
                set { depthBias.SetValue(value); }
            }

            EffectParameter shadowMap;
            public Texture2D ShadowMap
            {
                get { return shadowMap.GetValueTexture2D(); }
                set { shadowMap.SetValue(value); }
            }

            #region Constructors

            protected ShadowSceneEffect(Effect cloneSource)
                : base(cloneSource)
            {
                world = Parameters["World"];
                view = Parameters["View"];
                projection = Parameters["Projection"];

                lightViewProjection = Parameters["LightViewProjection"];
                depthBias = Parameters["DepthBias"];
                shadowMap = Parameters["ShadowMap"];
            }

            #endregion
        }

        public sealed class VsmSceneEffect : ShadowSceneEffect
        {
            public VsmSceneEffect(Effect cloneSource)
                : base(cloneSource)
            {
            }
        }

        public sealed class PcfSceneEffect : ShadowSceneEffect
        {
            #region Fields and Properties

            EffectParameter tapCountParameter;
            EffectParameter offsetsParameter;

            int shadowMapSize;
            int kernelSize;

            #endregion

            #region Constructors

            public PcfSceneEffect(Effect cloneSource)
                : base(cloneSource)
            {
                tapCountParameter = Parameters["TapCount"];
                offsetsParameter = Parameters["Offsets"];
            }

            #endregion

            public void Configure(int shadowMapSize, int kernelSize)
            {
                if (shadowMapSize <= 0)
                {
                    throw new ArgumentOutOfRangeException(
                        "shadowMapSize",
                        "The shadow map size must be a positive value.");
                }
                if (kernelSize <= 0)
                {
                    throw new ArgumentOutOfRangeException(
                        "kernelSize",
                        "The kernel size must be a positive value.");
                }

                bool dirty = false;
                if (this.shadowMapSize != shadowMapSize)
                {
                    this.shadowMapSize = shadowMapSize;
                    dirty = true;
                }
                if (this.kernelSize != kernelSize)
                {
                    this.kernelSize = kernelSize;
                    dirty = true;
                }
                if (dirty)
                {
                    PopulateKernel();
                }
            }

            void PopulateKernel()
            {
                float texelSize = 1.0f / (float) shadowMapSize;

                int start;
                if (kernelSize % 2 == 0)
                {
                    start = -(kernelSize / 2) + 1;
                }
                else
                {
                    start = -(kernelSize - 1) / 2;
                }
                int end = start + kernelSize;

                int tapCount = kernelSize * kernelSize;
                var offsets = new Vector2[tapCount];

                int i = 0;
                for (int y = start; y < end; y++)
                {
                    for (int x = start; x < end; x++)
                    {
                        offsets[i++] = new Vector2(x, y) * texelSize;
                    }
                }

                tapCountParameter.SetValue(tapCount);
                offsetsParameter.SetValue(offsets);
            }
        }

        public sealed class ClassicShadowSceneEffect : ShadowSceneEffect
        {
            public ClassicShadowSceneEffect(Effect cloneSource)
                : base(cloneSource)
            {
            }
        }

        #endregion

        #region Fields and Properties

        ShadowMapEffect basicShadowMapEffect;
        ShadowMapEffect vsmShadowMapEffect;
        PcfSceneEffect pcfSceneEffect;
        VsmSceneEffect vsmSceneEffect;
        ClassicShadowSceneEffect classicShadowSceneEffect;

        BackBuffer shadowMapBackBuffer;
        GaussianBlur vsmShadowMapBlur;

        LspsmLightCamera lspsmLightCamera;
        public LspsmLightCamera LspsmLightCamera
        {
            get { return lspsmLightCamera; }
        }

        BoundingFrustum lightCameraFrustum;
        List<Actor> shadowCasters;

        #endregion

        #region Constructors

        public SingleScreenSpaceShadow(IRenderContext context)
            : base(context)
        {
            lspsmLightCamera = new LspsmLightCamera();
            lightCameraFrustum = new BoundingFrustum(Matrix.Identity);
            shadowCasters = new List<Actor>();
        }

        #endregion

        protected override void EnableBackBuffers(bool enabled)
        {
            shadowMapBackBuffer.Enabled = enabled;
            vsmShadowMapBlur.Enabled =
                enabled &&
                ShadowSettings.Test == ShadowTest.Vsm &&
                ShadowSettings.Vsm.BlurEnabled;
            
            base.EnableBackBuffers(enabled);
        }

        #region LoadContent

        protected override void LoadContent()
        {
            if (ContentLoaded)
            {
                return;
            }

            basicShadowMapEffect = EffectManager.Load<ShadowMapEffect>();
            vsmShadowMapEffect = EffectManager.Load<ShadowMapEffect>(RenderConstants.VsmShadowMapEffectAssetName);
            vsmShadowMapBlur = new GaussianBlur(RenderContext, ShadowSettings.Size, ShadowSettings.Size, ShadowSettings.Format);
            vsmShadowMapBlur.LoadContent();

            vsmSceneEffect = EffectManager.Load<VsmSceneEffect>();
            pcfSceneEffect = EffectManager.Load<PcfSceneEffect>();
            classicShadowSceneEffect = EffectManager.Load<ClassicShadowSceneEffect>();

            shadowMapBackBuffer = BackBufferManager.Load("ShadowMap");
            shadowMapBackBuffer.Width = ShadowSettings.Size;
            shadowMapBackBuffer.Height = ShadowSettings.Size;
            shadowMapBackBuffer.MipMap = false;
            shadowMapBackBuffer.SurfaceFormat = ShadowSettings.Format;
            shadowMapBackBuffer.DepthFormat = DepthFormat.Depth24Stencil8;
            shadowMapBackBuffer.MultiSampleCount = 0;
            shadowMapBackBuffer.Enabled = false;

            base.LoadContent();
        }

        #endregion

        #region UnloadContent

        protected override void UnloadContent()
        {
            if (vsmShadowMapBlur != null)
            {
                vsmShadowMapBlur.Dispose();
                vsmShadowMapBlur = null;
            }

            base.UnloadContent();
        }

        #endregion

        void PrepareLspsmShadowCamera(GameTime gameTime)
        {
            lightCameraFrustum.Matrix = LspsmLightCamera.Pov.ViewProjection;

            //
            // Collect shadow casters and create the light volume.
            //
            CollectShadowCasters();
            //var ray = new Ray(Vector3.Zero, LspsmLightCamera.LightDirection);
            //foreach (CharacterActor actor in Context.ActorManager.Characters)
            //{
            //    var model = actor.Model;

            //    if (!model.CastShadowEnabled)
            //    {
            //        continue;
            //    }

            //    if (0 < model.MaxDrawDistance)
            //    {
            //        var maxDistanceSquared =
            //            actor.Model.MaxDrawDistance * actor.Model.MaxDrawDistance;
            //        var cameraToActor = LspsmLightCamera.ReferenceCamera.Position - actor.Position;
            //        var distanceSquared = cameraToActor.LengthSquared();

            //        if (maxDistanceSquared < distanceSquared)
            //        {
            //            continue;
            //        }
            //    }

            //    bool intersected;
            //    lightCameraFrustum.Intersects(ref model.WorldBoundingBox, out intersected);
            //    if (intersected)
            //    {
            //        LspsmLightCamera.AddLightVolume(ref model.WorldBoundingBox);
            //        shadowCasters.Add(actor);
            //        continue;
            //    }

            //    var corners = model.WorldBoundingBox.GetCorners();
            //    for (int cornerIndex = 0; cornerIndex < corners.Length; cornerIndex++)
            //    {
            //        ray.Position = corners[cornerIndex];
            //        if (ray.Intersects(lightCameraFrustum) != null)
            //        {
            //            LspsmLightCamera.AddLightVolumePoints(corners);
            //            shadowCasters.Add(actor);
            //            break;
            //        }
            //    }
            //}

            // Calculate the light volume on the backward space of the referenceCamera.
            // This volume must be needed to draw the terrains shadow.

            var pov = LspsmLightCamera.Pov;
            Vector3 povPosition;
            Matrix povOrientation;
            pov.GetPosition(out povPosition);
            pov.GetOrientation(out povOrientation);

            var backPosition = povPosition + povOrientation.Backward * ShadowSettings.BackwardLightVolumeRadius;
            LspsmLightCamera.AddLightVolumePoint(ref backPosition);
            var leftPosition = povPosition + povOrientation.Left * ShadowSettings.BackwardLightVolumeRadius;
            LspsmLightCamera.AddLightVolumePoint(ref leftPosition);
            var rightPosition = povPosition + povOrientation.Right * ShadowSettings.BackwardLightVolumeRadius;
            LspsmLightCamera.AddLightVolumePoint(ref rightPosition);
            var upPosition = povPosition + povOrientation.Up * ShadowSettings.BackwardLightVolumeRadius;
            LspsmLightCamera.AddLightVolumePoint(ref upPosition);

            // setup LspsmLightCamera
            LspsmLightCamera.ShadowMapSize = ShadowSettings.Size;
            LspsmLightCamera.LspsmSettings = ShadowSettings.Lspsm;

            // PrepareSplitShadowCameras light view/projection
            LspsmLightCamera.Prepare();
        }

        void CollectShadowCasters()
        {
            var ray = new Ray(Vector3.Zero, LspsmLightCamera.LightDirection);

            Scene.StaticMeshes.ForEach(
                actor =>
                {
                    TryAddShadowCaster(ref ray, actor);
                });

            Scene.Characters.ForEach(
                actor =>
                {
                    TryAddShadowCaster(ref ray, actor);
                });
        }

        void TryAddShadowCaster(ref Ray lightRay, Actor actor)
        {
            if (IsShadowCaster(ref lightRay, actor))
            {
                LspsmLightCamera.AddLightVolumePoints(
                    actor.ActorModel.WorldBoundingBox.GetCorners());
                shadowCasters.Add(actor);
            }
        }

        bool IsShadowCaster(ref Ray lightRay, Actor actor)
        {
            var model = actor.ActorModel;

            if (!model.CastShadowEnabled)
            {
                // sweep
                return false;
            }

            if (0 < model.MaxDrawDistance)
            {
                var maxDistanceSquared = actor.ActorModel.MaxDrawDistance * actor.ActorModel.MaxDrawDistance;
                Vector3 povPosition;
                LspsmLightCamera.Pov.GetPosition(out povPosition);
                var cameraToActor = povPosition - actor.Position;
                var distanceSquared = cameraToActor.LengthSquared();

                if (maxDistanceSquared < distanceSquared)
                {
                    return false;
                }
            }

            bool intersected;
            lightCameraFrustum.Intersects(ref model.WorldBoundingBox, out intersected);
            if (intersected)
            {
                return false;
            }

            var corners = model.WorldBoundingBox.GetCorners();
            for (int cornerIndex = 0; cornerIndex < corners.Length; cornerIndex++)
            {
                lightRay.Position = corners[cornerIndex];
                if (lightRay.Intersects(lightCameraFrustum) != null)
                {
                    return true;
                }
            }

            return false;
        }

        public override void PrepareShadowMap(GameTime gameTime)
        {
            PrepareLspsmShadowCamera(gameTime);

            GraphicsDevice.RasterizerState = RasterizerState.CullNone;
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            GraphicsDevice.BlendState = BlendState.Opaque;

            ShadowMapEffect currentShadowMapEffect;
            switch (ShadowSettings.Test)
            {
                case ShadowTest.Vsm:
                    currentShadowMapEffect = vsmShadowMapEffect;
                    break;
                case ShadowTest.Pcf:
                case ShadowTest.Classic:
                default:
                    currentShadowMapEffect = basicShadowMapEffect;
                    break;
                    
            }

            //
            // adjust
            //
            shadowMapBackBuffer.Width = ShadowSettings.Size;
            shadowMapBackBuffer.Height = ShadowSettings.Size;
            shadowMapBackBuffer.SurfaceFormat = ShadowSettings.Format;

            currentShadowMapEffect.View = LspsmLightCamera.LightView;
            currentShadowMapEffect.Projection = LspsmLightCamera.LightProjection;

            shadowMapBackBuffer.Begin();
            {
                GraphicsDevice.Clear(
                    ClearOptions.Target | ClearOptions.DepthBuffer,
                    Color.White,
                    1.0f,
                    0);

                RenderContext.VisibleTerrains.ForEach(
                    actor =>
                    {
                        if (actor.ActorModel.CastShadowEnabled)
                        {
                            actor.ActorModel.Draw(gameTime, currentShadowMapEffect);
                        }
                    });

                shadowCasters.ForEach(actor => actor.ActorModel.Draw(gameTime, currentShadowMapEffect));
            }
            shadowMapBackBuffer.End();

            LspsmLightCamera.ClearLightVolumePoints();
            shadowCasters.Clear();

            vsmShadowMapBlur.Enabled =
                (ShadowSettings.Test == ShadowTest.Vsm &&
                ShadowSettings.Vsm.BlurEnabled);
            if (vsmShadowMapBlur.Enabled)
            {
                vsmShadowMapBlur.Radius = ShadowSettings.Vsm.BlurRadius;
                vsmShadowMapBlur.Amount = ShadowSettings.Vsm.BlurAmount;
                vsmShadowMapBlur.Filter(shadowMapBackBuffer.GetRenderTarget(), shadowMapBackBuffer);
            }

            if (DebugMap.Instance != null)
            {
                DebugMap.Instance.Maps.Add(shadowMapBackBuffer.GetRenderTarget());
            }

            GraphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;
        }

        public override void PrepareShadowSceneMap(GameTime gameTime)
        {
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            GraphicsDevice.BlendState = BlendState.Opaque;

            #region Select a shadow map technique

            ShadowSceneEffect currentEffect;
            switch (ShadowSettings.Test)
            {
                case ShadowTest.Vsm:
                    currentEffect = vsmSceneEffect;
                    break;
                case ShadowTest.Pcf:
                    pcfSceneEffect.Configure(
                        ShadowSettings.Size,
                        ShadowSettings.Pcf.KernelSize);
                    currentEffect = pcfSceneEffect;
                    break;
                case ShadowTest.Classic:
                default:
                    currentEffect = classicShadowSceneEffect;
                    break;
            }

            #endregion

            #region Prepare the effect

            var pov = Scene.ActiveCamera.Pov;
            currentEffect.View = pov.View;
            currentEffect.Projection = pov.Projection;
            currentEffect.LightViewProjection = LspsmLightCamera.LightViewProjection;
            currentEffect.DepthBias = ShadowSettings.DepthBias;
            currentEffect.ShadowMap = shadowMapBackBuffer.GetRenderTarget();

            #endregion

            #region Prepare Back buffers

            var pp = GraphicsDevice.PresentationParameters;
            var mapScale = ShadowSettings.ScreenSpaceShadow.MapScale;

            ShadowSceneMapBackBuffer.Width = (int) (pp.BackBufferWidth * mapScale);
            ShadowSceneMapBackBuffer.Height = (int) (pp.BackBufferHeight * mapScale);

            #endregion

            #region Draw the shadow scene map

            ShadowSceneMapBackBuffer.Begin();
            {
                GraphicsDevice.Clear(
                    ClearOptions.Target | ClearOptions.DepthBuffer,
                    Color.White,
                    1.0f,
                    0);

                RenderContext.VisibleTerrains.ForEach(
                    actor => actor.ActorModel.Draw(gameTime, currentEffect));
                RenderContext.VisibleStaticMeshes.ForEach(
                    actor => actor.ActorModel.Draw(gameTime, currentEffect));
                RenderContext.VisibleCharacters.ForEach(
                    actor => actor.ActorModel.Draw(gameTime, currentEffect));
            }
            ShadowSceneMapBackBuffer.End();

            BlurShadowSceneMap();

            #endregion

            // 生成した Shadow Scene Map をプロパティに設定します。
            ShadowSceneMap = ShadowSceneMapBackBuffer.GetRenderTarget();

            if (DebugMap.Instance != null)
            {
                DebugMap.Instance.Maps.Add(ShadowSceneMapBackBuffer.GetRenderTarget());
            }
        }
    }
}
