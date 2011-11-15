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
    public sealed class PssmScreenSpaceShadow : ScreenSpaceShadow
    {
        #region Inner classes

        public abstract class PssmSceneEffect : Effect, IEffectMatrices
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

            EffectParameter depthBias;
            public float DepthBias
            {
                get { return depthBias.GetValueSingle(); }
                set { depthBias.SetValue(value); }
            }

            EffectParameter splitCount;
            public int SplitCount
            {
                get { return splitCount.GetValueInt32(); }
                set { splitCount.SetValue(value); }
            }

            EffectParameter splitDistances;
            public float[] SplitDistances
            {
                get { return splitDistances.GetValueSingleArray(Pssm.MaxSplitCount); }
                set { splitDistances.SetValue(value); }
            }

            EffectParameter splitViewProjections;
            public Matrix[] SplitViewProjections
            {
                get { return splitViewProjections.GetValueMatrixArray(Pssm.MaxSplitCount); }
                set { splitViewProjections.SetValue(value); }
            }

            EffectParameter[] shadowMaps;
            public void SetShadowMap(int index, Texture2D shadowMap)
            {
                shadowMaps[index].SetValue(shadowMap);
            }

            #region Constructors

            protected PssmSceneEffect(Effect cloneSource)
                : base(cloneSource)
            {
                world = Parameters["World"];
                view = Parameters["View"];
                projection = Parameters["Projection"];

                depthBias = Parameters["DepthBias"];
                splitCount = Parameters["SplitCount"];
                splitDistances = Parameters["SplitDistances"];
                splitViewProjections = Parameters["SplitViewProjections"];

                shadowMaps = new EffectParameter[Pssm.MaxSplitCount];
                for (int i = 0; i < shadowMaps.Length; i++)
                {
                    shadowMaps[i] = Parameters["ShadowMap" + i];
                }
            }

            #endregion
        }

        public sealed class PssmVsmSceneEffect : PssmSceneEffect
        {
            public PssmVsmSceneEffect(Effect cloneSource)
                : base(cloneSource)
            {
            }
        }

        public sealed class PssmPcfSceneEffect : PssmSceneEffect
        {
            #region Fields and Properties

            EffectParameter tapCountParameter;
            EffectParameter offsetsParameter;

            int shadowMapSize;
            int kernelSize;

            #endregion

            #region Constructors

            public PssmPcfSceneEffect(Effect cloneSource)
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

        public sealed class PssmClassicShadowSceneEffect : PssmSceneEffect
        {
            public PssmClassicShadowSceneEffect(Effect cloneSource)
                : base(cloneSource)
            {
            }
        }

        #endregion

        #region Fields and Properties

        ShadowMapEffect basicShadowMapEffect;
        ShadowMapEffect vsmShadowMapEffect;
        PssmVsmSceneEffect pssmVsmSceneEffect;
        PssmPcfSceneEffect pssmPcfSceneEffect;
        PssmClassicShadowSceneEffect pssmClassicShadowSceneEffect;

        BackBuffer shadowMapBackBuffer;

        GaussianBlur vsmShadowMapBlur;

        Pssm pssm = new Pssm(3);
        public Pssm Pssm
        {
            get { return pssm; }
        }

        BoundingFrustum lightCameraFrustum = new BoundingFrustum(Matrix.Identity);
        Vector3[] lightCameraFrustumCorners = new Vector3[8];
        List<Vector3> lightVolumePoints = new List<Vector3>();
        BoundingFrustum splitLightCameraFrustum = new BoundingFrustum(Matrix.Identity);
        List<Actor> allShadowCasters = new List<Actor>();
        List<Actor> splitShadowCasters = new List<Actor>();
        Vector3[] modelBoundingBoxCorners = new Vector3[8];

        #endregion

        #region Constructors

        public PssmScreenSpaceShadow(IRenderContext context)
            : base(context)
        {
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

        #region LoadContent/UnloadContent

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

            pssmVsmSceneEffect = EffectManager.Load<PssmVsmSceneEffect>();
            pssmPcfSceneEffect = EffectManager.Load<PssmPcfSceneEffect>();
            pssmClassicShadowSceneEffect = EffectManager.Load<PssmClassicShadowSceneEffect>();

            shadowMapBackBuffer = BackBufferManager.Load("ShadowMap");
            shadowMapBackBuffer.Width = ShadowSettings.Size;
            shadowMapBackBuffer.Height = ShadowSettings.Size;
            shadowMapBackBuffer.MipMap = false;
            shadowMapBackBuffer.SurfaceFormat = ShadowSettings.Format;
            shadowMapBackBuffer.DepthFormat = DepthFormat.Depth24Stencil8;
            shadowMapBackBuffer.MultiSampleCount = 0;
            shadowMapBackBuffer.RenderTargetCount = ShadowSettings.Pssm.SplitCount;
            shadowMapBackBuffer.Enabled = false;

            base.LoadContent();
        }

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

        void PreparePssm(GameTime gameTime)
        {
            #region Create the light volume

            lightCameraFrustum.Matrix = Pssm.Pov.ViewProjection;
            lightCameraFrustumCorners = lightCameraFrustum.GetCorners();
            lightVolumePoints.AddRange(lightCameraFrustumCorners);

            var boundingBox = BoundingBox.CreateFromPoints(lightVolumePoints);
            lightVolumePoints.Clear();

            #endregion

            #region Prepare splitted light camera

            Pssm.PrepareSplitLightCameras(ref boundingBox);

            #endregion

            #region Collect shadow casters in the global pssm light volume.

            CollectShadowCasters();

            #endregion
        }

        void CollectShadowCasters()
        {
            var ray = new Ray(Vector3.Zero, Pssm.LightDirection);

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
                allShadowCasters.Add(actor);
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
                Pssm.Pov.GetPosition(out povPosition);
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
                return true;
            }

            model.WorldBoundingBox.GetCorners(modelBoundingBoxCorners);
            for (int cornerIndex = 0; cornerIndex < 8; cornerIndex++)
            {
                lightRay.Position = modelBoundingBoxCorners[cornerIndex];
                if (lightRay.Intersects(lightCameraFrustum) != null)
                {
                    return true;
                }
            }

            return false;
        }

        public override void PrepareShadowMap(GameTime gameTime)
        {
            PreparePssm(gameTime);

            GraphicsDevice.RasterizerState = RasterizerState.CullNone;
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            GraphicsDevice.BlendState = BlendState.Opaque;

            #region Select the effect by the shadow test type

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

            #endregion

            #region Configure the back buffer for the shadow map

            shadowMapBackBuffer.Width = ShadowSettings.Size;
            shadowMapBackBuffer.Height = ShadowSettings.Size;
            shadowMapBackBuffer.SurfaceFormat = ShadowSettings.Format;
            shadowMapBackBuffer.RenderTargetCount = ShadowSettings.Pssm.SplitCount;

            #endregion

            #region Backward ligght volume to the first split light camera only

            Vector3 povPosition;
            Matrix povOrientation;
            Pssm.Pov.GetPosition(out povPosition);
            Pssm.Pov.GetOrientation(out povOrientation);

            var radius = ShadowSettings.BackwardLightVolumeRadius;

            var backPoint = povPosition + povOrientation.Backward * radius;
            var leftPoint = povPosition + povOrientation.Left * radius;
            var rightPoint = povPosition + povOrientation.Right * radius;
            var upPoint = povPosition + povOrientation.Up * radius;

            var firstSplitLightCamera = Pssm.GetSplitLightCamera(0);
            firstSplitLightCamera.AddLightVolumePoint(ref backPoint);
            firstSplitLightCamera.AddLightVolumePoint(ref leftPoint);
            firstSplitLightCamera.AddLightVolumePoint(ref rightPoint);
            firstSplitLightCamera.AddLightVolumePoint(ref upPoint);

            #endregion

            var ray = new Ray(Vector3.Zero, Pssm.LightDirection);
            bool modelIntersected;
            for (int i = 0; i < Pssm.SplitCount; i++)
            {
                shadowMapBackBuffer.CurrentIndex = i;

                var splitLightCamera = Pssm.GetSplitLightCamera(i);

                #region Collect shadow casters and create the light volume in the each split frustum

                splitLightCameraFrustum.Matrix = splitLightCamera.Pov.ViewProjection;
                foreach (var actor in allShadowCasters)
                {
                    var model = actor.ActorModel;

                    splitLightCameraFrustum.Intersects(ref model.WorldBoundingBox,  out modelIntersected);
                    
                    if (modelIntersected)
                    {
                        splitShadowCasters.Add(actor);
                        splitLightCamera.AddLightVolume(ref model.WorldBoundingBox);
                        continue;
                    }

                    model.WorldBoundingBox.GetCorners(modelBoundingBoxCorners);
                    for (int cornerIndex = 0; cornerIndex < 8; cornerIndex++)
                    {
                        ray.Position = modelBoundingBoxCorners[cornerIndex];
                        if (ray.Intersects(splitLightCameraFrustum) != null)
                        {
                            splitShadowCasters.Add(actor);
                            splitLightCamera.AddLightVolume(ref model.WorldBoundingBox);
                            break;
                        }
                    }
                }

                #endregion

                #region Prepare the current split's shadow camera

                splitLightCamera.Prepare();

                #endregion

                #region Prepare effect

                currentShadowMapEffect.LightViewProjection = splitLightCamera.LightViewProjection;

                #endregion

                #region Draw

                shadowMapBackBuffer.Begin();
                {
                    GraphicsDevice.Clear(
                        ClearOptions.Target | ClearOptions.DepthBuffer,
                        Color.White,
                        1.0f,
                        0);

                    #region Terrains

                    RenderContext.VisibleTerrains.ForEach(
                        actor =>
                        {
                            if (actor.ActorModel.CastShadowEnabled)
                            {
                                actor.ActorModel.Draw(gameTime, currentShadowMapEffect);
                            }
                        });

                    #endregion

                    #region Other shadow casters

                    splitShadowCasters.ForEach(
                        actor => actor.ActorModel.Draw(gameTime, currentShadowMapEffect));

                    #endregion
                }
                shadowMapBackBuffer.End();

                #endregion

                splitLightCamera.ClearLightVolumePoints();
                splitShadowCasters.Clear();

                #region VSM blur

                vsmShadowMapBlur.Enabled =
                    (ShadowSettings.Test == ShadowTest.Vsm &&
                    ShadowSettings.Vsm.BlurEnabled);
                if (vsmShadowMapBlur.Enabled)
                {
                    vsmShadowMapBlur.Radius = ShadowSettings.Vsm.BlurRadius;
                    vsmShadowMapBlur.Amount = ShadowSettings.Vsm.BlurAmount;
                    vsmShadowMapBlur.Filter(shadowMapBackBuffer.GetRenderTarget(), shadowMapBackBuffer);
                }

                #endregion

                #region Notify intermediate map

                if (DebugMap.Instance != null)
                {
                    DebugMap.Instance.Maps.Add(shadowMapBackBuffer.GetRenderTarget());
                }
                
                #endregion
            }

            allShadowCasters.Clear();

            GraphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;
        }

        public override void PrepareShadowSceneMap(GameTime gameTime)
        {
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            GraphicsDevice.BlendState = BlendState.Opaque;

            #region Select a shadow map technique

            PssmSceneEffect currentEffect;
            switch (ShadowSettings.Test)
            {
                case ShadowTest.Vsm:
                    currentEffect = pssmVsmSceneEffect;
                    break;
                case ShadowTest.Pcf:
                    pssmPcfSceneEffect.Configure(
                        ShadowSettings.Size,
                        ShadowSettings.Pcf.KernelSize);
                    currentEffect = pssmPcfSceneEffect;
                    break;
                case ShadowTest.Classic:
                default:
                    currentEffect = pssmClassicShadowSceneEffect;
                    break;
            }

            #endregion

            #region Prepare the effect

            var pov = Scene.ActiveCamera.Pov;
            currentEffect.View = pov.View;
            currentEffect.Projection = pov.Projection;
            currentEffect.DepthBias = ShadowSettings.DepthBias;
            currentEffect.SplitCount = shadowMapBackBuffer.RenderTargetCount;
            currentEffect.SplitDistances = Pssm.SplitDistances;
            currentEffect.SplitViewProjections = Pssm.SplitViewProjections;
            for (int i = 0; i < Pssm.SplitCount; i++)
            {
                shadowMapBackBuffer.CurrentIndex = i;
                currentEffect.SetShadowMap(i, shadowMapBackBuffer.GetRenderTarget());
            }

            #endregion

            #region Prepare Back buffers

            var pp = GraphicsDevice.PresentationParameters;
            var width = (int) (pp.BackBufferWidth * ShadowSettings.ScreenSpaceShadow.MapScale);
            var height = (int) (pp.BackBufferHeight * ShadowSettings.ScreenSpaceShadow.MapScale);
            ShadowSceneMapBackBuffer.Width = width;
            ShadowSceneMapBackBuffer.Height = height;

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
