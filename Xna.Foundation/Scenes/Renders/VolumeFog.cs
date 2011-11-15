#region Using

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
    public sealed class VolumeFog : RenderComponent
    {
        #region Inner classes

        public sealed class VolumeFogSceneDepthMapEffect : Effect, IEffectMatrices
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

            #region Constructors

            public VolumeFogSceneDepthMapEffect(Effect cloneSource)
                : base(cloneSource)
            {
                world = Parameters["World"];
                view = Parameters["View"];
                projection = Parameters["Projection"];
            }

            #endregion
        }

        public sealed class VolumeFogDepthMapEffect : Effect, IEffectMatrices
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

            #region Fields and Properties

            EffectParameter sceneDepthMap;
            public Texture2D SceneDepthMap
            {
                get { return sceneDepthMap.GetValueTexture2D(); }
                set { sceneDepthMap.SetValue(value); }
            }

            public EffectTechnique FogDepthCWTechnique
            {
                get;
                private set;
            }

            public EffectTechnique FogDepthCCWTechnique
            {
                get;
                private set;
            }

            #endregion

            #region Constructors

            public VolumeFogDepthMapEffect(Effect cloneSource)
                : base(cloneSource)
            {
                world = Parameters["World"];
                view = Parameters["View"];
                projection = Parameters["Projection"];
                sceneDepthMap = Parameters["SceneDepthMap"];

                FogDepthCWTechnique = Techniques["FogDepthCW"];
                FogDepthCCWTechnique = Techniques["FogDepthCCW"];
            }

            #endregion
        }

        public sealed class VolumeFogMapEffect : Effect, IEffectMatrices
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

            #region Fields and Properties

            EffectParameter fogColor;
            public Vector3 FogColor
            {
                get { return fogColor.GetValueVector3(); }
                set { fogColor.SetValue(value); }
            }

            EffectParameter fogScale;
            public float FogScale
            {
                get { return fogScale.GetValueSingle(); }
                set { fogScale.SetValue(value); }
            }

            EffectParameter fogDepthCWMap;
            public Texture2D FogDepthCWMap
            {
                get { return fogDepthCWMap.GetValueTexture2D(); }
                set { fogDepthCWMap.SetValue(value); }
            }

            EffectParameter fogDepthCCWMap;
            public Texture2D FogDepthCCWMap
            {
                get { return fogDepthCCWMap.GetValueTexture2D(); }
                set { fogDepthCCWMap.SetValue(value); }
            }

            #endregion

            #region Constructors

            public VolumeFogMapEffect(Effect cloneSource)
                : base(cloneSource)
            {
                world = Parameters["World"];
                view = Parameters["View"];
                projection = Parameters["Projection"];

                fogColor = Parameters["FogColor"];
                fogScale = Parameters["FogScale"];
                fogDepthCWMap = Parameters["FogDepthCWMap"];
                fogDepthCCWMap = Parameters["FogDepthCCWMap"];
            }

            #endregion
        }

        public sealed class VolumeFogBlendEffect : Effect
        {
            #region Fields and Properties

            EffectParameter viewportSize;
            public Vector2 ViewportSize
            {
                get { return viewportSize.GetValueVector2(); }
                set { viewportSize.SetValue(value); }
            }

            EffectParameter sceneMap;
            public Texture2D SceneMap
            {
                get { return sceneMap.GetValueTexture2D(); }
                set { sceneMap.SetValue(value); }
            }

            EffectParameter fogMap;
            public Texture2D FogMap
            {
                get { return fogMap.GetValueTexture2D(); }
                set { fogMap.SetValue(value); }
            }

            #endregion

            #region Constructors

            public VolumeFogBlendEffect(Effect cloneSource)
                : base(cloneSource)
            {
                viewportSize = Parameters["ViewportSize"];
                sceneMap = Parameters["SceneMap"];
                fogMap = Parameters["FogMap"];
            }

            #endregion
        }

        #endregion

        #region Fields and Properties

        VolumeFogSceneDepthMapEffect volumeFogSceneDepthMapEffect;
        VolumeFogDepthMapEffect volumeFogDepthMapEffect;
        VolumeFogMapEffect volumeFogMapEffect;
        VolumeFogBlendEffect volumeFogBlendEffect;

        BackBuffer sceneDepthMapBackBuffer;
        BackBuffer fogDepthCWMapBackBuffer;
        BackBuffer fogDepthCCWMapBackBuffer;
        BackBuffer fogMapBackBuffer;

        public float MapScale = 0.5f;

        BoundingFrustum cameraFrustum = new BoundingFrustum(Matrix.Identity);

        QuadVertexBuffer quadVertexBuffer;

        #endregion

        #region Constructors

        public VolumeFog(IRenderContext RenderContext)
            : base(RenderContext)
        {
        }

        #endregion

        protected override void OnEnabledChanged()
        {
            if (ContentLoaded)
            {
                sceneDepthMapBackBuffer.Enabled = Enabled;
                fogDepthCWMapBackBuffer.Enabled = Enabled;
                fogDepthCCWMapBackBuffer.Enabled = Enabled;
                fogMapBackBuffer.Enabled = Enabled;
            }

            base.OnEnabledChanged();
        }

        public void Prepare(GameTime gameTime)
        {
            var pov = Scene.ActiveCamera.Pov;
            cameraFrustum.Matrix = pov.ViewProjection;

            volumeFogSceneDepthMapEffect.View = pov.View;
            volumeFogSceneDepthMapEffect.Projection = pov.Projection;
            volumeFogDepthMapEffect.View = pov.View;
            volumeFogDepthMapEffect.Projection = pov.Projection;
            volumeFogMapEffect.View = pov.View;
            volumeFogMapEffect.Projection = pov.Projection;

            var pp = GraphicsDevice.PresentationParameters;
            int width = (int) (pp.BackBufferWidth * MapScale);
            int height = (int) (pp.BackBufferHeight * MapScale);

            sceneDepthMapBackBuffer.Width = width;
            sceneDepthMapBackBuffer.Height = height;

            fogDepthCWMapBackBuffer.Width = width;
            fogDepthCWMapBackBuffer.Height = height;

            fogDepthCCWMapBackBuffer.Width = width;
            fogDepthCCWMapBackBuffer.Height = height;

            fogMapBackBuffer.Width = width;
            fogMapBackBuffer.Height = height;

            #region Scene Depth

            sceneDepthMapBackBuffer.Begin();
            {
                GraphicsDevice.DepthStencilState = DepthStencilState.Default;
                GraphicsDevice.BlendState = BlendState.Opaque;
                GraphicsDevice.Clear(
                    ClearOptions.Target | ClearOptions.DepthBuffer,
                    new Color(1.0f, 0.0f, 0.0f, 0.0f),
                    // important
                    1.0f,
                    0);

                DrawSceneDepthMap(gameTime);
            }
            sceneDepthMapBackBuffer.End();

            #endregion

            volumeFogDepthMapEffect.SceneDepthMap = sceneDepthMapBackBuffer.GetRenderTarget();

            #region Fog Depth CW

            fogDepthCWMapBackBuffer.Begin();
            {
                GraphicsDevice.DepthStencilState = DepthStencilState.Default;
                GraphicsDevice.BlendState = BlendState.Opaque;
                GraphicsDevice.Clear(
                    ClearOptions.Target | ClearOptions.DepthBuffer,
                    new Color(0.0f, 0.0f, 0.0f, 0.0f),
                    // important
                    0.0f,
                    //1.0f,
                    0);

                volumeFogDepthMapEffect.CurrentTechnique = volumeFogDepthMapEffect.FogDepthCWTechnique;
                DrawFogDepthMap(gameTime);
            }
            fogDepthCWMapBackBuffer.End();

            #endregion

            #region Fog Depth CCW

            fogDepthCCWMapBackBuffer.Begin();
            {
                GraphicsDevice.DepthStencilState = DepthStencilState.Default;
                GraphicsDevice.BlendState = BlendState.Opaque;
                GraphicsDevice.Clear(
                    ClearOptions.Target | ClearOptions.DepthBuffer,
                    new Color(0.0f, 0.0f, 0.0f, 0.0f),
                    // important
                    1.0f,
                    0);

                volumeFogDepthMapEffect.CurrentTechnique = volumeFogDepthMapEffect.FogDepthCCWTechnique;
                DrawFogDepthMap(gameTime);
            }
            fogDepthCCWMapBackBuffer.End();

            #endregion

            #region Fog Map

            fogMapBackBuffer.Begin();
            {
                GraphicsDevice.DepthStencilState = DepthStencilState.Default;
                GraphicsDevice.BlendState = BlendState.Opaque;
                GraphicsDevice.RasterizerState = RasterizerState.CullNone;
                GraphicsDevice.Clear(
                    ClearOptions.Target | ClearOptions.DepthBuffer,
                    new Color(0.0f, 0.0f, 0.0f, 0.0f),
                    1.0f,
                    0);
                DrawFogMap(gameTime);
                GraphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;
            }
            fogMapBackBuffer.End();

            #endregion

            #region Notify intermediate maps

            if (DebugMap.Instance != null)
            {
                DebugMap.Instance.Maps.Add(sceneDepthMapBackBuffer.GetRenderTarget());
                DebugMap.Instance.Maps.Add(fogDepthCWMapBackBuffer.GetRenderTarget());
                DebugMap.Instance.Maps.Add(fogDepthCCWMapBackBuffer.GetRenderTarget());
                DebugMap.Instance.Maps.Add(fogMapBackBuffer.GetRenderTarget());
            }

            #endregion
        }

        void DrawSceneDepthMap(GameTime gameTime)
        {
            var pov = Scene.ActiveCamera.Pov;
            var cullingDistanceSquared = pov.FarPlaneDistance * pov.FarPlaneDistance;

            RenderContext.VisibleTerrains.ForEach(
                actor => actor.ActorModel.Draw(gameTime, volumeFogSceneDepthMapEffect));

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
                        actor.ActorModel.Draw(gameTime, volumeFogSceneDepthMapEffect);
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
                        actor.ActorModel.Draw(gameTime, volumeFogSceneDepthMapEffect);
                    }
                }
            }

            //
            // TODO: OK ?
            //
            GraphicsDevice.RasterizerState = RasterizerState.CullClockwise;
            {
                RenderContext.VisibleFluidSurfaces.ForEach(
                    actor => actor.ActorModel.Draw(gameTime, volumeFogSceneDepthMapEffect));
            }
            GraphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;
        }

        void DrawFogDepthMap(GameTime gameTime)
        {
            foreach (var actor in RenderContext.VisibleVolumeFogs)
            {
                actor.ActorModel.Draw(gameTime, volumeFogDepthMapEffect);
            }
        }

        void DrawFogMap(GameTime gameTime)
        {
            foreach (var actor in RenderContext.VisibleVolumeFogs)
            {
                var model = actor.ActorModel as VolumeFogActorModel;

                volumeFogMapEffect.FogColor = model.FogColor;
                volumeFogMapEffect.FogScale = model.FogScale;
                volumeFogMapEffect.FogDepthCWMap = fogDepthCWMapBackBuffer.GetRenderTarget();
                volumeFogMapEffect.FogDepthCCWMap = fogDepthCCWMapBackBuffer.GetRenderTarget();

                actor.ActorModel.Draw(gameTime, volumeFogMapEffect);
            }
        }

        public void Filter(Texture2D source, BackBuffer destination)
        {
            if (!Enabled)
            {
                throw new InvalidOperationException("VolumeFog disabled");
            }

            int w = destination.Width;
            int h = destination.Height;

            volumeFogBlendEffect.ViewportSize = new Vector2(w, h);
            volumeFogBlendEffect.SceneMap = source;
            volumeFogBlendEffect.FogMap = fogMapBackBuffer.GetRenderTarget();

            destination.Begin();
            {

                GraphicsDevice.DepthStencilState = DepthStencilState.DepthRead;
                GraphicsDevice.BlendState = BlendState.NonPremultiplied;

                foreach (var pass in volumeFogBlendEffect.CurrentTechnique.Passes)
                {
                    pass.Apply();
                    quadVertexBuffer.Draw();
                }

                GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            }
            destination.End();
        }

        #region LoadContent

        protected override void LoadContent()
        {
            if (ContentLoaded)
            {
                return;
            }

            #region Effects

            volumeFogSceneDepthMapEffect = EffectManager.Load<VolumeFogSceneDepthMapEffect>();
            volumeFogDepthMapEffect = EffectManager.Load<VolumeFogDepthMapEffect>();
            volumeFogMapEffect = EffectManager.Load<VolumeFogMapEffect>();
            volumeFogBlendEffect = EffectManager.Load<VolumeFogBlendEffect>();

            #endregion

            #region Back buffers

            var pp = GraphicsDevice.PresentationParameters;
            var width = (int) (pp.BackBufferWidth * MapScale);
            var height = (int) (pp.BackBufferHeight * MapScale);

            sceneDepthMapBackBuffer = BackBufferManager.Load(
                "VolumeFog.SceneDepthMap");
            sceneDepthMapBackBuffer.Width = width;
            sceneDepthMapBackBuffer.Height = height;
            sceneDepthMapBackBuffer.MipMap = true;
            sceneDepthMapBackBuffer.SurfaceFormat = SurfaceFormat.Single;
            sceneDepthMapBackBuffer.DepthFormat = DepthFormat.Depth24Stencil8;
            sceneDepthMapBackBuffer.MultiSampleCount = pp.MultiSampleCount;
            sceneDepthMapBackBuffer.RenderTargetUsage = pp.RenderTargetUsage;

            fogDepthCWMapBackBuffer = BackBufferManager.Load(
                "VolumeFog.FogDepthCWMap");
            fogDepthCWMapBackBuffer.Width = width;
            fogDepthCWMapBackBuffer.Height = height;
            fogDepthCWMapBackBuffer.MipMap = true;
            fogDepthCWMapBackBuffer.SurfaceFormat = SurfaceFormat.Single;
            fogDepthCWMapBackBuffer.DepthFormat = DepthFormat.Depth24Stencil8;
            fogDepthCWMapBackBuffer.MultiSampleCount = pp.MultiSampleCount;
            fogDepthCWMapBackBuffer.RenderTargetUsage = pp.RenderTargetUsage;

            fogDepthCCWMapBackBuffer = BackBufferManager.Load(
                "VolumeFog.FogDepthCCWMap",
                fogDepthCWMapBackBuffer);

            fogMapBackBuffer = BackBufferManager.Load(
                "VolumeFog.FogMap");
            fogMapBackBuffer.Width = width;
            fogMapBackBuffer.Height = height;
            fogMapBackBuffer.MipMap = true;
            fogMapBackBuffer.SurfaceFormat = SurfaceFormat.Color;
            fogMapBackBuffer.DepthFormat = DepthFormat.Depth24Stencil8;
            fogMapBackBuffer.MultiSampleCount = pp.MultiSampleCount;
            fogMapBackBuffer.RenderTargetUsage = pp.RenderTargetUsage;

            //
            // TODO
            //
            sceneDepthMapBackBuffer.Enabled = false;
            fogDepthCWMapBackBuffer.Enabled = false;
            fogDepthCCWMapBackBuffer.Enabled = false;
            fogMapBackBuffer.Enabled = false;

            #endregion

            quadVertexBuffer = new QuadVertexBuffer(GraphicsDevice);

            base.LoadContent();
        }

        #endregion

        #region UnloadContent

        protected override void UnloadContent()
        {
            quadVertexBuffer.Dispose();

            base.UnloadContent();
        }

        #endregion
    }
}
