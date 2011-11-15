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
using Willcraftia.Xna.Foundation.Graphics;

#endregion

namespace Willcraftia.Xna.Foundation.Scenes
{
    #region Aliases

    using WcDirectionalLight = Willcraftia.Xna.Foundation.Scenes.DirectionalLight;
    using XnaDirectionalLight = Microsoft.Xna.Framework.Graphics.DirectionalLight;

    #endregion

    public sealed class SkyDomeActorModel : AssetModelActorModel
    {
        #region Inner classes

        public sealed class CustomEffect : Effect, IEffectMatrices, IEffectLights
        {
            EffectParameter world;
            EffectParameter view;
            EffectParameter projection;
            EffectParameter worldInvertTranspose;

            EffectParameter lightDirection;
            EffectParameter lightDiffuseColor;
            EffectParameter lightingEnabled;

            EffectParameter sunPower;
            EffectParameter time;
            EffectParameter skyMap;

            XnaDirectionalLight directionalLight0;
            XnaDirectionalLight directionalLight1;
            XnaDirectionalLight directionalLight2;

            public CustomEffect(Effect cloneSource)
                : base(cloneSource)
            {
                world = Parameters["World"];
                view = Parameters["View"];
                projection = Parameters["Projection"];
                worldInvertTranspose = Parameters["WorldInvertTranspose"];

                lightDirection = Parameters["LightDirection"];
                lightDiffuseColor = Parameters["LightDiffuseColor"];
                lightingEnabled = Parameters["LightingEnabled"];
                
                sunPower = Parameters["SunPower"];
                time = Parameters["Time"];
                skyMap = Parameters["SkyMap"];

                directionalLight0 = new XnaDirectionalLight(lightDirection, lightDiffuseColor, null, null);
                directionalLight1 = new XnaDirectionalLight(null, null, null, null);
                directionalLight2 = new XnaDirectionalLight(null, null, null, null);
            }

            #region IEffectMatrices

            public Matrix Projection
            {
                get { return projection.GetValueMatrix(); }
                set { projection.SetValue(value); }
            }

            public Matrix View
            {
                get { return view.GetValueMatrix(); }
                set { view.SetValue(value); }
            }

            public Matrix World
            {
                get { return world.GetValueMatrix(); }
                set
                {
                    world.SetValue(value);
                    worldInvertTranspose.SetValue(Matrix.Transpose(Matrix.Invert(value)));
                }
            }

            #endregion

            #region IEffectLights

            public Vector3 AmbientLightColor
            {
                get { return Vector3.Zero; }
                set { }
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
                get { return lightingEnabled.GetValueBoolean(); }
                set { lightingEnabled.SetValue(value); }
            }

            #endregion

            public float SunPower
            {
                get { return sunPower.GetValueSingle(); }
                set { sunPower.SetValue(value); }
            }

            public float Time
            {
                get { return time.GetValueSingle(); }
                set { time.SetValue(value); }
            }

            public Texture2D SkyMap
            {
                get { return skyMap.GetValueTexture2D(); }
                set { skyMap.SetValue(value); }
            }
        }

        #endregion

        #region Fields and Properties

        float sunPower = 500.0f;

        [DefaultValue(500.0f)]
        public float SunPower
        {
            get { return sunPower; }
            set
            {
                if (sunPower != value)
                {
                    sunPower = value;
                }
            }
        }

        bool adjustBlenderRadiusEnabled;

        [DefaultValue(false)]
        public bool AdjustBlenderRadiusEnabled
        {
            get { return adjustBlenderRadiusEnabled; }
            set
            {
                if (adjustBlenderRadiusEnabled != value)
                {
                    adjustBlenderRadiusEnabled = value;
                }
            }
        }

        Dictionary<string, string> skyMapDefinitions = new Dictionary<string, string>();
        public Dictionary<string, string> SkyMapDefinitions
        {
            get { return skyMapDefinitions; }
        }

        string activeSkyMapName;

        [DefaultValue(null)]
        public string ActiveSkyMapName
        {
            get { return activeSkyMapName; }
            set
            {
                if (activeSkyMapName != value)
                {
                    activeSkyMapName = value;
                }
            }
        }

        NamedTexture2DCollection skyMaps = new NamedTexture2DCollection();

        [Browsable(false)]
        [ContentSerializerIgnore]
        public NamedTexture2DCollection SkyMaps
        {
            get { return skyMaps; }
        }

        [Browsable(false)]
        [ContentSerializerIgnore]
        public Texture2D ActiveSkyMap { get; set; }

        float modelRadius;

        #endregion

        #region LoadContent

        public override void LoadContent()
        {
            foreach (var definition in skyMapDefinitions)
            {
                var name = definition.Key;
                var assetName = definition.Value;

                var texture = Content.Load<Texture2D>(assetName);
                texture.Name = name;
                SkyMaps.Add(texture);
            }

            ActiveSkyMap = SkyMaps[activeSkyMapName];

            base.LoadContent();

            if (base.Model != null)
            {
                modelRadius = Model.GetBoundingSphere().Radius;
                if (AdjustBlenderRadiusEnabled)
                {
                    modelRadius *= BlenderRadiusAdjustmentScale;
                }
            }
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

        protected override void PrepareEffectOnceDraw(GameTime gameTime, Effect effect)
        {
            var customEffect = effect as CustomEffect;

            customEffect.SunPower = SunPower;

            // MEMO:
            // ワールド時間は [0, 1] で 0 時 - 24 時、テクスチャは [0, 1] で 12 時 - 24 時で Mirror。

            customEffect.Time = 2.0f * ActorContext.SceneSettings.Time - 1.0f;
            customEffect.SkyMap = ActiveSkyMap;

            base.PrepareEffectOnceDraw(gameTime, effect);
        }

        protected override void CalculateMeshWorld(ModelMesh mesh, out Matrix result)
        {
            Matrix meshWorld;
            base.CalculateMeshWorld(mesh, out meshWorld);

            Matrix positionTranslation;
            Matrix.CreateTranslation(ref ActorContext.ActiveCamera.Position, out positionTranslation);

            Matrix.Multiply(ref meshWorld, ref positionTranslation, out result);
        }

        protected override void CalculateViewProjection(out Matrix view, out Matrix projection)
        {
            var scale = new Vector3(Actor.Scale.M11, Actor.Scale.M22, Actor.Scale.M33);
            var maxScale = Math.Max(Math.Max(scale.X, scale.Y), scale.Z);
            var far = modelRadius * maxScale;

            var pov = ActorContext.ActiveCamera.Pov;

            // use the view matrix of the active camera.
            view = pov.View;

            // use the special projection matrix.
            Matrix.CreatePerspectiveFieldOfView(pov.Fov, pov.AspectRatio, pov.NearPlaneDistance, far, out projection);
        }

        protected override bool PreDraw(GameTime gameTime)
        {
            GraphicsDevice.RasterizerState = RasterizerState.CullClockwise;

            return base.PreDraw(gameTime);
        }

        protected override void PostDraw(GameTime gameTime)
        {
            GraphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;

            base.PostDraw(gameTime);
        }
    }
}
