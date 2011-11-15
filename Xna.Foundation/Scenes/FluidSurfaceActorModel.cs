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

namespace Willcraftia.Xna.Foundation.Scenes
{
    #region Aliases

    using WcDirectionalLight = Willcraftia.Xna.Foundation.Scenes.DirectionalLight;
    using XnaDirectionalLight = Microsoft.Xna.Framework.Graphics.DirectionalLight;

    #endregion

    public sealed class FluidSurfaceActorModel : AssetModelActorModel, IRenderTargetPrepared
    {
        #region Inner classes

        public sealed class FluidSurfaceNormalMapEffect : Effect
        {
            #region Fields and Properties

            public EffectTechnique CreateWaveMapTechnique
            {
                get; private set;
            }

            public EffectTechnique CreateNormalMapTechnique
            {
                get; private set;
            }

            EffectParameter sampleOffset;
            public Vector2 SampleOffset
            {
                get { return sampleOffset.GetValueVector2(); }
                set { sampleOffset.SetValue(value); }
            }

            EffectParameter springPower;
            public float SpringPower
            {
                get { return springPower.GetValueSingle(); }
                set { springPower.SetValue(value); }
            }

            EffectParameter addWavePosition;
            public Vector2 AddWavePosition
            {
                get { return addWavePosition.GetValueVector2(); }
                set { addWavePosition.SetValue(value); }
            }

            EffectParameter addWaveRadius;
            public float AddWaveRadius
            {
                get { return addWaveRadius.GetValueSingle(); }
                set { addWaveRadius.SetValue(value); }
            }

            EffectParameter addWaveVelocity;
            public float AddWaveVelocity
            {
                get { return addWaveVelocity.GetValueSingle(); }
                set { addWaveVelocity.SetValue(value); }
            }

            #endregion

            #region Constructors

            public FluidSurfaceNormalMapEffect(Effect cloneSource)
                : base(cloneSource)
            {
                sampleOffset = Parameters["SampleOffset"];
                springPower = Parameters["SpringPower"];

                addWavePosition = Parameters["AddWavePosition"];
                addWaveRadius = Parameters["AddWaveRadius"];
                addWaveVelocity = Parameters["AddWaveVelocity"];

                CreateWaveMapTechnique = Techniques["CreateWaveMap"];
                CreateNormalMapTechnique = Techniques["CreateNormalMap"];
            }

            #endregion
        }

        public sealed class CustomEffect :
            Effect, IEffectMatrices, IEffectLights, IEffectFog
        {
            #region Fields

            EffectParameter world;
            EffectParameter view;
            EffectParameter projection;

            EffectParameter eyePosition;

            EffectParameter fogEnabled;
            EffectParameter fogStart;
            EffectParameter fogEnd;
            EffectParameter fogColor;

            EffectParameter diffuseColor;
            EffectParameter emissiveColor;
            EffectParameter specularColor;
            EffectParameter specularPower;

            EffectParameter ambientLightColor;

            EffectParameter dirLight0Direction;
            EffectParameter dirLight0DiffuseColor;
            EffectParameter dirLight0SpecularColor;

            EffectParameter dirLight1Direction;
            EffectParameter dirLight1DiffuseColor;
            EffectParameter dirLight1SpecularColor;

            EffectParameter dirLight2Direction;
            EffectParameter dirLight2DiffuseColor;
            EffectParameter dirLight2SpecularColor;

            XnaDirectionalLight directionalLight0;
            XnaDirectionalLight directionalLight1;
            XnaDirectionalLight directionalLight2;

            EffectParameter minAlpha;
            EffectParameter maxAlpha;
            EffectParameter distanceAlphaFactor;

            EffectParameter normalMap;
            EffectParameter sampleOffset;
            EffectParameter textureSize;
            EffectParameter textureScale;

            #endregion

            #region Constructors

            public CustomEffect(Effect cloneSource)
                : base(cloneSource)
            {
                world = Parameters["World"];
                view = Parameters["View"];
                projection = Parameters["Projection"];
                eyePosition = Parameters["EyePosition"];

                fogEnabled = Parameters["FogEnabled"];
                fogStart = Parameters["FogStart"];
                fogEnd = Parameters["FogEnd"];
                fogColor = Parameters["FogColor"];

                diffuseColor = Parameters["DiffuseColor"];
                emissiveColor = Parameters["EmissiveColor"];
                specularColor = Parameters["SpecularColor"];
                specularPower = Parameters["SpecularPower"];

                ambientLightColor = Parameters["AmbientLightColor"];

                dirLight0Direction = Parameters["DirLight0Direction"];
                dirLight0DiffuseColor = Parameters["DirLight0DiffuseColor"];
                dirLight0SpecularColor = Parameters["DirLight0SpecularColor"];

                dirLight1Direction = Parameters["DirLight1Direction"];
                dirLight1DiffuseColor = Parameters["DirLight1DiffuseColor"];
                dirLight1SpecularColor = Parameters["DirLight1SpecularColor"];

                dirLight2Direction = Parameters["DirLight2Direction"];
                dirLight2DiffuseColor = Parameters["DirLight2DiffuseColor"];
                dirLight2SpecularColor = Parameters["DirLight2SpecularColor"];
                
                minAlpha = Parameters["MinAlpha"];
                maxAlpha = Parameters["MaxAlpha"];
                distanceAlphaFactor = Parameters["DistanceAlphaFactor"];

                normalMap = Parameters["NormalMap"];
                sampleOffset = Parameters["SampleOffset"];
                textureSize = Parameters["TextureSize"];
                textureScale = Parameters["TextureScale"];

                directionalLight0 = new XnaDirectionalLight(
                    dirLight0Direction,
                    dirLight0DiffuseColor,
                    dirLight0SpecularColor, null);
                directionalLight1 = new XnaDirectionalLight(
                    dirLight1Direction,
                    dirLight1DiffuseColor,
                    dirLight1SpecularColor, null);
                directionalLight2 = new XnaDirectionalLight(
                    dirLight2Direction,
                    dirLight2DiffuseColor,
                    dirLight2SpecularColor, null);
            }

            #endregion

            #region IEffectMatrices

            public Matrix Projection
            {
                get { return projection.GetValueMatrix(); }
                set { projection.SetValue(value); }
            }

            public Matrix View
            {
                get { return view.GetValueMatrix(); }
                set
                {
                    view.SetValue(value);
                    var viewInv = Matrix.Invert(value);
                    eyePosition.SetValue(new Vector3(viewInv.M41, viewInv.M42, viewInv.M43));
                }
            }

            public Matrix World
            {
                get { return world.GetValueMatrix(); }
                set { world.SetValue(value); }
            }

            #endregion

            #region IEffectFog

            public Vector3 FogColor
            {
                get { return fogColor.GetValueVector3(); }
                set { fogColor.SetValue(value); }
            }

            public bool FogEnabled
            {
                get { return fogEnabled.GetValueBoolean(); }
                set { fogEnabled.SetValue(value); }
            }

            public float FogEnd
            {
                get { return fogEnd.GetValueSingle(); }
                set { fogEnd.SetValue(value); }
            }

            public float FogStart
            {
                get { return fogStart.GetValueSingle(); }
                set { fogStart.SetValue(value); }
            }

            #endregion

            #region IEffectLights

            public Vector3 AmbientLightColor
            {
                get { return ambientLightColor.GetValueVector3(); }
                set { ambientLightColor.SetValue(value); }
            }

            public XnaDirectionalLight DirectionalLight0
            {
                get { return directionalLight0; }
            }

            public XnaDirectionalLight DirectionalLight1
            {
                get { return directionalLight1; }
            }

            public XnaDirectionalLight DirectionalLight2
            {
                get { return directionalLight2; }
            }

            public void EnableDefaultLighting()
            {
            }

            public bool LightingEnabled
            {
                get { return true; }
                set { }
            }

            #endregion

            #region Material colors

            public Vector3 DiffuseColor
            {
                get { return diffuseColor.GetValueVector3(); }
                set { diffuseColor.SetValue(value); }
            }

            public Vector3 EmissiveColor
            {
                get { return emissiveColor.GetValueVector3(); }
                set { emissiveColor.SetValue(value); }
            }

            public Vector3 SpecularColor
            {
                get { return specularColor.GetValueVector3(); }
                set { specularColor.SetValue(value); }
            }

            public float SpecularPower
            {
                get { return specularPower.GetValueSingle(); }
                set { specularPower.SetValue(value); }
            }

            #endregion

            #region Transparency

            public float MinAlpha
            {
                get { return minAlpha.GetValueSingle(); }
                set { minAlpha.SetValue(value); }
            }

            public float MaxAlpha
            {
                get { return maxAlpha.GetValueSingle(); }
                set { maxAlpha.SetValue(value); }
            }

            public float DistanceAlphaFactor
            {
                get { return distanceAlphaFactor.GetValueSingle(); }
                set { distanceAlphaFactor.SetValue(value); }
            }

            #endregion

            #region Normal map settings

            public Vector2 SampleOffset
            {
                get { return sampleOffset.GetValueVector2(); }
                set { sampleOffset.SetValue(value); }
            }

            public Vector2 TextureSize
            {
                get { return textureSize.GetValueVector2(); }
                set { textureSize.SetValue(value); }
            }

            public float TextureScale
            {
                get { return textureScale.GetValueSingle(); }
                set { textureScale.SetValue(value); }
            }

            public Texture2D NormalMap
            {
                get { return normalMap.GetValueTexture2D(); }
                set { normalMap.SetValue(value); }
            }

            #endregion
        }

        #endregion

        public int TextureSize = 256;
        public TextureAddressMode TextureAddress = TextureAddressMode.Wrap;

        SpriteBatch spriteBatch;
        EffectManager effectManager;
        FluidSurfaceNormalMapEffect normalMapEffect;

        BackBufferManager backBufferManager;
        BackBuffer prevWaveMapBackBuffer;
        BackBuffer waveMapBackBuffer;
        BackBuffer normalMapBackBuffer;

        #region LoadContent

        public override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            #region Effect の初期化

            effectManager = new EffectManager(GraphicsDevice, Content);
            normalMapEffect = effectManager.Load<FluidSurfaceNormalMapEffect>();
            normalMapEffect.SampleOffset = new Vector2(1.0f / (float) TextureSize, 1.0f / (float) TextureSize);

            #endregion

            #region 内部で使用する RenderTarget の初期化

            backBufferManager = new BackBufferManager(GraphicsDevice);

            prevWaveMapBackBuffer = backBufferManager.Load("FluidSurfacePrevWaveMap");
            prevWaveMapBackBuffer.Width = TextureSize;
            prevWaveMapBackBuffer.Height = TextureSize;
            prevWaveMapBackBuffer.MipMap = true;
            prevWaveMapBackBuffer.SurfaceFormat = SurfaceFormat.Vector2;
            prevWaveMapBackBuffer.DepthFormat = DepthFormat.None;
            prevWaveMapBackBuffer.MultiSampleCount = 0;
            prevWaveMapBackBuffer.Enabled = true;

            waveMapBackBuffer = backBufferManager.Load("FluidSurfaceWaveMap");
            waveMapBackBuffer.Width = TextureSize;
            waveMapBackBuffer.Height = TextureSize;
            waveMapBackBuffer.MipMap = true;
            waveMapBackBuffer.SurfaceFormat = SurfaceFormat.Vector2;
            waveMapBackBuffer.DepthFormat = DepthFormat.None;
            waveMapBackBuffer.MultiSampleCount = 0;
            waveMapBackBuffer.Enabled = true;

            normalMapBackBuffer = backBufferManager.Load("FluidSurfaceNormalMap");
            normalMapBackBuffer.Width = TextureSize;
            normalMapBackBuffer.Height = TextureSize;
            normalMapBackBuffer.MipMap = true;
            normalMapBackBuffer.SurfaceFormat = SurfaceFormat.Vector2;
            normalMapBackBuffer.DepthFormat = DepthFormat.None;
            normalMapBackBuffer.MultiSampleCount = 0;
            normalMapBackBuffer.Enabled = true;

            prevWaveMapBackBuffer.Begin();
            {
                GraphicsDevice.Clear(Color.Black);
            }
            prevWaveMapBackBuffer.End();

            #endregion

            base.LoadContent();
        }

        protected override void RemapEffects()
        {
            if (!(Model.Meshes[0].Effects[0] is CustomEffect))
            {
                foreach (var mesh in Model.Meshes)
                {
                    foreach (var part in mesh.MeshParts)
                    {
                        part.Effect = new CustomEffect(part.Effect);
                    }
                }
            }
            base.RemapEffects();
        }

        #endregion

        #region UnloadContent

        public override void UnloadContent()
        {
            effectManager.Unload();
            backBufferManager.Unload();

            base.UnloadContent();
        }

        #endregion

        #region Draw

        protected override bool PreDraw(GameTime gameTime)
        {
            // カメラが水面の上下いずれの位置にあっても描画されるように Culling を停止させます。
            GraphicsDevice.RasterizerState = RasterizerState.CullNone;
            GraphicsDevice.BlendState = BlendState.NonPremultiplied;

            return base.PreDraw(gameTime);
        }

        protected override void PrepareEffectOnceDraw(GameTime gameTime, Effect effect)
        {
            SetTextureParameters(effect as CustomEffect);

            base.PrepareEffectOnceDraw(gameTime, effect);
        }

        void SetTextureParameters(CustomEffect effect)
        {
            effect.SampleOffset = new Vector2(1.0f / (float) TextureSize, 1.0f / (float) TextureSize);
            effect.TextureSize = new Vector2(TextureSize, TextureSize);
            effect.NormalMap = normalMapBackBuffer.GetRenderTarget();
        }

        protected override void PostDraw(GameTime gameTime)
        {
            // 描画前に Culling を停止させたのでデフォルト設定へ戻します。
            GraphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;

            base.PostDraw(gameTime);
        }

        #endregion

        #region IRenderTargetPrepared

        public void PrepareRenderTargets(GameTime gameTime)
        {
            CreateWaveMap();
            MementoWaveMap();
            CreateNormalMap();
        }

        //------------------------
        // TODO
        int wavePositionIndex;
        int wavePositionCount = 1;
        Random random = new Random();
        //------------------------

        void CreateWaveMap()
        {
            var samplerState = ResolveSamplerState();

            waveMapBackBuffer.Begin();
            {
                //------------------------
                // TODO
                if (wavePositionIndex < wavePositionCount)
                {
                    //fluidSurfaceNormalMapEffect.AddWavePosition = new Vector2(
                    //    (float) random.NextDouble(),
                    //    (float) random.NextDouble());
                    normalMapEffect.AddWavePosition = new Vector2(0.2f, 0.2f);
                    normalMapEffect.AddWaveRadius = 0.1f;
                    normalMapEffect.AddWaveVelocity = 0.001f;
                    wavePositionIndex++;
                }
                //------------------------

                normalMapEffect.CurrentTechnique = normalMapEffect.CreateWaveMapTechnique;
                spriteBatch.Begin(
                    SpriteSortMode.Deferred,
                    BlendState.Opaque,
                    ResolveSamplerState(),
                    null,
                    null,
                    normalMapEffect);
                spriteBatch.Draw(prevWaveMapBackBuffer.GetRenderTarget(), Vector2.Zero, Color.White);
                spriteBatch.End();
            }
            waveMapBackBuffer.End();

            if (DebugMap.Instance != null)
            {
                DebugMap.Instance.Maps.Add(waveMapBackBuffer.GetRenderTarget());
            }
        }

        SamplerState ResolveSamplerState()
        {
            switch (TextureAddress)
            {
                case TextureAddressMode.Wrap:
                    return SamplerState.PointWrap;
                case TextureAddressMode.Clamp:
                default:
                    return SamplerState.PointClamp;
            }
        }

        void MementoWaveMap()
        {
            prevWaveMapBackBuffer.Begin();
            {
                spriteBatch.Begin(
                    SpriteSortMode.Deferred,
                    BlendState.Opaque,
                    SamplerState.PointClamp,
                    null,
                    null);
                spriteBatch.Draw(waveMapBackBuffer.GetRenderTarget(), Vector2.Zero, Color.White);
                spriteBatch.End();
            }
            prevWaveMapBackBuffer.End();
        }

        void CreateNormalMap()
        {
            normalMapBackBuffer.Begin();
            {
                normalMapEffect.CurrentTechnique =
                    normalMapEffect.CreateNormalMapTechnique;
                spriteBatch.Begin(
                    SpriteSortMode.Deferred,
                    BlendState.Opaque,
                    ResolveSamplerState(),
                    null,
                    null,
                    normalMapEffect);
                spriteBatch.Draw(prevWaveMapBackBuffer.GetRenderTarget(), Vector2.Zero, Color.White);
                spriteBatch.End();
            }
            normalMapBackBuffer.End();

            if (DebugMap.Instance != null)
            {
                DebugMap.Instance.Maps.Add(normalMapBackBuffer.GetRenderTarget());
            }
        }

        #endregion
    }
}
