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
    public sealed class Dof : RenderComponent
    {
        #region Inner classes

        public sealed class DofEffect : Effect
        {
            EffectParameter focusDistanceParameter;
            float focusDistance;
            public float FocusDistance
            {
                get { return focusDistance; }
                set
                {
                    if (focusDistance != value)
                    {
                        focusDistance = value;
                        focusDistanceParameter.SetValue(value);
                    }
                }
            }

            EffectParameter focusRangeParameter;
            float focusRange;
            public float FocusRange
            {
                get { return focusRange; }
                set
                {
                    if (focusRange != value)
                    {
                        focusRange = value;
                        focusRangeParameter.SetValue(value);
                    }
                }
            }

            EffectParameter nearPlaneDistanceParameter;
            float nearPlaneDistance;
            public float NearPlaneDistance
            {
                get { return nearPlaneDistance; }
                set
                {
                    if (nearPlaneDistance != value)
                    {
                        nearPlaneDistance = value;
                        nearPlaneDistanceParameter.SetValue(value);
                    }
                }
            }

            EffectParameter farPlaneDistanceParameter;
            float farPlaneDistance;
            public float FarPlaneDistance
            {
                get { return farPlaneDistance; }
                set
                {
                    if (farPlaneDistance != value)
                    {
                        farPlaneDistance = value;
                        farPlaneDistanceParameter.SetValue(value);
                    }
                }
            }

            EffectParameter sceneMapParameter;
            public Texture2D SceneMap
            {
                get { return sceneMapParameter.GetValueTexture2D(); }
                set { sceneMapParameter.SetValue(value); }
            }

            EffectParameter depthMapParameter;
            public Texture2D DepthMap
            {
                get { return depthMapParameter.GetValueTexture2D(); }
                set { depthMapParameter.SetValue(value); }
            }

            EffectParameter bluredSceneMapParameter;
            public Texture2D BluredSceneMap
            {
                get { return bluredSceneMapParameter.GetValueTexture2D(); }
                set { bluredSceneMapParameter.SetValue(value); }
            }

            public DofEffect(Effect cloneSource)
                : base(cloneSource)
            {
                focusDistanceParameter = Parameters["FocusDistance"];
                focusRangeParameter = Parameters["FocusRange"];
                nearPlaneDistanceParameter = Parameters["NearPlaneDistance"];
                farPlaneDistanceParameter = Parameters["FarPlaneDistance"];

                sceneMapParameter = Parameters["SceneMap"];
                depthMapParameter = Parameters["DepthMap"];
                bluredSceneMapParameter = Parameters["BluredSceneMap"];

                focusDistance = -1;
                focusRange = -1;
                nearPlaneDistance = -1;
                farPlaneDistance = -1;
            }
        }

        #endregion

        #region Fields and Properties

        public const SurfaceFormat DefaultDepthMapFormat = SurfaceFormat.Single;

        DepthMapEffect depthMapEffect;
        DofEffect depthOfFieldEffect;

        BackBuffer depthMapBackBuffer;
        BackBuffer bluredSceneMapBackBuffer;
        
        GaussianBlur gaussianBlur;

        SurfaceFormat depthMapFormat;
        public SurfaceFormat DepthMapFormat
        {
            get { return depthMapFormat; }
            set
            {
                if (depthMapFormat != value)
                {
                    depthMapFormat = value;

                    if (ContentLoaded)
                    {
                        depthMapBackBuffer.SurfaceFormat = value;
                    }
                }
            }
        }

        public DofSettings Settings;

        BoundingFrustum cameraFrustum;

        #endregion

        #region Constructors

        public Dof(IRenderContext renderContext)
            : base(renderContext)
        {
            depthMapFormat = DefaultDepthMapFormat;
            Settings = DofSettings.Default;
            cameraFrustum = new BoundingFrustum(Matrix.Identity);
        }

        #endregion

        public void PrepareDepthMap(GameTime gameTime)
        {
            if (!Enabled)
            {
                throw new InvalidOperationException("Depth of field disabled");
            }

            #region Prepare the reference camera

            var pov = Scene.ActiveCamera.Pov;
            Matrix projection;
            Matrix.CreatePerspectiveFieldOfView(
                pov.Fov, pov.AspectRatio, pov.NearPlaneDistance, Settings.FarPlaneDistance, out projection);
            cameraFrustum.Matrix = pov.View * projection;

            #endregion

            #region Prepare effects

            depthMapEffect.View = pov.View;
            depthMapEffect.Projection = projection;

            #endregion

            #region Draw

            GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            GraphicsDevice.BlendState = BlendState.Opaque;

            var cullingDistanceSquared = Settings.FarPlaneDistance * Settings.FarPlaneDistance;

            depthMapBackBuffer.Begin();
            {
                GraphicsDevice.Clear(
                    ClearOptions.Target | ClearOptions.DepthBuffer,
                    Color.White,
                    1.0f,
                    0);

                RenderContext.VisibleTerrains.ForEach(
                    actor => actor.ActorModel.Draw(gameTime, depthMapEffect));

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
                            actor.ActorModel.Draw(gameTime, depthMapEffect);
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
                            actor.ActorModel.Draw(gameTime, depthMapEffect);
                        }
                    }
                }
            }
            depthMapBackBuffer.End();

            #endregion

            #region Notify intermediate maps

            if (DebugMap.Instance != null)
            {
                DebugMap.Instance.Maps.Add(depthMapBackBuffer.GetRenderTarget());
            }

            #endregion
        }

        public void Filter(Texture2D source, BackBuffer destination)
        {
            if (!Enabled)
            {
                throw new InvalidOperationException("Depth of field disabled");
            }

            #region Adjust back buffer size if the destination is resized

            var pp = GraphicsDevice.PresentationParameters;
            bluredSceneMapBackBuffer.Width = (int) (pp.BackBufferWidth * Settings.MapScale);
            bluredSceneMapBackBuffer.Height = (int) (pp.BackBufferHeight * Settings.MapScale);
            
            #endregion

            #region Blur

            gaussianBlur.Radius = Settings.BlurRadius;
            gaussianBlur.Amount = Settings.BlurAmount;
            gaussianBlur.Filter(source, bluredSceneMapBackBuffer);
            
            #endregion

            #region Notify the intermadiate map

            if (DebugMap.Instance != null)
            {
                DebugMap.Instance.Maps.Add(bluredSceneMapBackBuffer.GetRenderTarget());
            }
            
            #endregion

            #region Apply DoF to the destination

            var pov = Scene.ActiveCamera.Pov;

            depthOfFieldEffect.NearPlaneDistance = pov.NearPlaneDistance;
            depthOfFieldEffect.FarPlaneDistance = pov.FarPlaneDistance / (pov.FarPlaneDistance - pov.NearPlaneDistance);

            if (Settings.FocusOverrideEnabled)
            {
                depthOfFieldEffect.FocusRange = Settings.FocusRange;
                depthOfFieldEffect.FocusDistance = Settings.FocusDistance;
            }
            else
            {
                depthOfFieldEffect.FocusRange = pov.FocusRange;
                depthOfFieldEffect.FocusDistance = pov.FocusDistance;
            }

            depthOfFieldEffect.DepthMap = depthMapBackBuffer.GetRenderTarget();
            depthOfFieldEffect.BluredSceneMap = bluredSceneMapBackBuffer.GetRenderTarget();

            GraphicsDevice.DepthStencilState = DepthStencilState.None;
            GraphicsDevice.BlendState = BlendState.Opaque;

            destination.Begin();
            {
                SpriteBatch.Render(depthOfFieldEffect, source, destination.Bounds);
            }
            destination.End();

            #endregion
        }

        #region LoadContent

        protected override void LoadContent()
        {
            #region Effects

            depthMapEffect = EffectManager.Load<DepthMapEffect>();
            depthOfFieldEffect = EffectManager.Load<DofEffect>();

            #endregion

            #region Back buffers

            const int depthMapSampleQuality = 0;

            var pp = GraphicsDevice.PresentationParameters;
            var width = (int) (pp.BackBufferWidth * Settings.MapScale);
            var height = (int) (pp.BackBufferHeight * Settings.MapScale);

            depthMapBackBuffer = BackBufferManager.Load("DepthMap");
            depthMapBackBuffer.Width = width;
            depthMapBackBuffer.Height = height;
            depthMapBackBuffer.MipMap = false;
            depthMapBackBuffer.SurfaceFormat = depthMapFormat;
            depthMapBackBuffer.DepthFormat = DepthFormat.Depth24Stencil8;
            depthMapBackBuffer.MultiSampleCount = depthMapSampleQuality;
            depthMapBackBuffer.Enabled = Enabled;

            bluredSceneMapBackBuffer = BackBufferManager.Load("BluredSceneMap");
            bluredSceneMapBackBuffer.Width = width;
            bluredSceneMapBackBuffer.Height = height;
            bluredSceneMapBackBuffer.MipMap = false;
            bluredSceneMapBackBuffer.SurfaceFormat = SurfaceFormat.Color;
            bluredSceneMapBackBuffer.MultiSampleCount = 0;
            bluredSceneMapBackBuffer.Enabled = Enabled;

            #endregion

            #region Gaussian blur

            gaussianBlur = new GaussianBlur(RenderContext, width, height, bluredSceneMapBackBuffer.SurfaceFormat);
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
                depthMapBackBuffer.Enabled = Enabled;
                bluredSceneMapBackBuffer.Enabled = Enabled;
                gaussianBlur.Enabled = Enabled;
            }

            base.OnEnabledChanged();
        }
    }
}
