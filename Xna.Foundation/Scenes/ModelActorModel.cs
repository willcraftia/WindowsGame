#region Using

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Willcraftia.Xna.Foundation.Graphics;

#endregion

namespace Willcraftia.Xna.Foundation.Scenes
{
    using WcDirectionalLight = Willcraftia.Xna.Foundation.Scenes.DirectionalLight;
    using XnaDirectionalLight = Microsoft.Xna.Framework.Graphics.DirectionalLight;

    /// <summary>
    /// Model クラスを用いて描画する ActorModel です。
    /// </summary>
    public class ModelActorModel : ActorModel
    {
        /// <summary>
        /// The magic number to rescale kernelSize.
        /// I think that XNA FBX importer and/or
        /// Blender FBX exporter have a scaling bug.
        /// </summary>
        public const float BlenderRadiusAdjustmentScale = 100.0f;

        Model model;

        /// <summary>
        /// Model。
        /// </summary>
        [ContentSerializerIgnore]
        public Model Model
        {
            get { return model; }
            set { model = value; }
        }

        Vector3[] corners = new Vector3[8];
        Matrix[] absoluteBoneTransforms;

        public override void LoadContent()
        {
            if (model != null)
            {
                LocalBoundingBox = model.GetBoundingBox();
                absoluteBoneTransforms = new Matrix[model.Bones.Count];
                model.CopyAbsoluteBoneTransformsTo(absoluteBoneTransforms);
                RemapEffects();
            }

            base.LoadContent();
        }

        /// <summary>
        /// Model 内の Effect を再配置するために呼び出されます。
        /// </summary>
        protected virtual void RemapEffects()
        {
        }

        protected override void UpdateWorldBoundingBox(GameTime gameTime)
        {
            LocalBoundingBox.GetCorners(corners);

            Matrix transform;
            Matrix.Multiply(ref Actor.Scale, ref Actor.Translation, out transform);

            Vector3.Transform(corners, ref transform, corners);
            WorldBoundingBox = BoundingBox.CreateFromPoints(corners);
        }

        protected override bool PreDraw(GameTime gameTime)
        {
            if (model == null)
            {
                return false;
            }

            return base.PreDraw(gameTime);
        }

        /// <summary>
        /// Draw メソッドの呼び出し毎に Effect を初期化するためのメソッドです。
        /// </summary>
        /// <param name="gameTime">前回の Update が呼び出されてからの経過時間。</param>
        /// <param name="effect">Effect。</param>
        /// <param name="world">World 行列。</param>
        protected virtual void PrepareEffectEachDraw(GameTime gameTime, Effect effect, ref Matrix world)
        {
            SetWorldParameter(effect, ref world);
            SetAlphaParameter(effect);
        }

        /// <summary>
        /// Effect が IEffectMatrices を実装している場合、その World プロパティへ World 行列を設定します。
        /// </summary>
        /// <param name="effect">Effect。</param>
        /// <param name="world">World 行列。</param>
        /// <remarks>
        /// ActorModel は複数の Actor 間で共有されるため、
        /// 描画に使用する World 行列はその時点での Actor から計算して設定する必要があります。
        /// このため、このメソッドは PrepareEffectEachDraw から呼び出されます。
        /// </remarks>
        void SetWorldParameter(Effect effect, ref Matrix world)
        {
            var effectMatrices = effect as IEffectMatrices;
            if (effectMatrices != null)
            {
                effectMatrices.World = world;
            }
        }

        /// <summary>
        /// Effect が BasicEffect の場合、その Alpha プロパティへ CurrentAlpha プロパティを設定します。
        /// Effect が IEffectTransparency を実装している場合、その Alpha プロパティへ CurrentAlpha プロパティを設定します。
        /// </summary>
        /// <param name="effect">Effect。</param>
        /// <remarks>
        /// ActorModel は複数の Actor 間で共有されるため、
        /// 描画に使用するアルファ値はその時点での Actor から計算して設定する必要があります。
        /// このため、このメソッドは PrepareEffectEachDraw から呼び出されます。
        /// </remarks>
        void SetAlphaParameter(Effect effect)
        {
            var basicEffect = effect as BasicEffect;
            if (basicEffect != null)
            {
                basicEffect.Alpha = CurrentAlpha;
                return;
            }

            var effectTransparency = effect as IEffectTransparency;
            if (effectTransparency != null)
            {
                effectTransparency.Alpha = CurrentAlpha;
            }
        }

        protected override void PrepareEffectOnceDraw(GameTime gameTime)
        {
            foreach (var mesh in model.Meshes)
            {
                foreach (var effect in mesh.Effects)
                {
                    PrepareEffectOnceDraw(gameTime, effect);
                }
            }
            base.PrepareEffectOnceDraw(gameTime);
        }

        /// <summary>
        /// 複数回呼び出される Draw メソッドに対して、1 度だけ Effect を初期化するために呼び出されます。
        /// </summary>
        /// <param name="gameTime">前回の Update が呼び出されてからの経過時間。</param>
        /// <param name="effect">Effect。</param>
        protected virtual void PrepareEffectOnceDraw(GameTime gameTime, Effect effect)
        {
            SetViewProjectionParameters(effect);
            SetBasicDirectionaLightParameters(effect);
            SetFogParameters(effect);
        }

        /// <summary>
        /// Effect が IEffectMatrices を実装している場合、
        /// IEffectMatrices のプロパティへ View/Projection 行列を設定します。
        /// </summary>
        /// <param name="effect">Effect。</param>
        /// <remarks>
        /// Game の Draw メソッド呼び出し内では、カメラの View/Projection 行列は一定です。
        /// つまり、ActorModel の Draw メソッドが複数回呼び出されたとしても変更の必要がないため、
        /// このメソッドは PrepareEffectOnceDraw から呼び出されます。
        /// </remarks>
        void SetViewProjectionParameters(Effect effect)
        {
            var effectMatrices = effect as IEffectMatrices;
            if (effectMatrices == null)
            {
                // ignore.
                return;
            }

            Matrix view;
            Matrix projection;
            CalculateViewProjection(out view, out projection);

            effectMatrices.View = view;
            effectMatrices.Projection = projection;
        }

        /// <summary>
        /// Effect が IEffectLights を実装している場合、
        /// IEffectLights のプロパティへシーンのライト情報を設定します。
        /// </summary>
        /// <param name="effect">Effect。</param>
        /// <remarks>
        /// Game の Draw メソッド呼び出し内では、シーンのライト情報は一定です。
        /// つまり、ActorModel の Draw メソッドが複数回呼び出されたとしても変更の必要がないため、
        /// このメソッドは PrepareEffectOnceDraw から呼び出されます。
        /// </remarks>
        void SetBasicDirectionaLightParameters(Effect effect)
        {
            var effectLights = effect as IEffectLights;
            if (effectLights == null)
            {
                return;
            }

            var sceneSettings = Actor.ActorContext.SceneSettings;

            effectLights.AmbientLightColor = sceneSettings.GlobalAmbientColor;

            var lightingEnabled = false;
            if (SetBasicDirectionaLightParameter(effectLights.DirectionalLight0, ref sceneSettings.DirectionalLight0))
            {
                lightingEnabled = true;
            }
            if (SetBasicDirectionaLightParameter(effectLights.DirectionalLight1, ref sceneSettings.DirectionalLight1))
            {
                lightingEnabled = true;
            }
            if (SetBasicDirectionaLightParameter(effectLights.DirectionalLight2, ref sceneSettings.DirectionalLight2))
            {
                lightingEnabled = true;
            }

            effectLights.LightingEnabled = lightingEnabled;
        }

        bool SetBasicDirectionaLightParameter(
            XnaDirectionalLight xnaDirectionalLight,
            ref WcDirectionalLight wcDirectionalLight)
        {
            if (wcDirectionalLight.Enabled)
            {
                xnaDirectionalLight.DiffuseColor = wcDirectionalLight.DiffuseColor;
                xnaDirectionalLight.SpecularColor = wcDirectionalLight.SpecularColor;
                xnaDirectionalLight.Direction = wcDirectionalLight.Direction;
                xnaDirectionalLight.Enabled = true;
                return true;
            }
            else
            {
                xnaDirectionalLight.Enabled = false;
                return false;
            }
        }

        /// <summary>
        /// Effect が IEffectFog を実装している場合、
        /// IEffectFog のプロパティへシーンのフォグ情報を設定します。
        /// </summary>
        /// <param name="effect">Effect。</param>
        /// <remarks>
        /// Game の Draw メソッド呼び出し内では、シーンのフォグ情報は一定です。
        /// つまり、ActorModel の Draw メソッドが複数回呼び出されたとしても変更の必要がないため、
        /// このメソッドは PrepareEffectOnceDraw から呼び出されます。
        /// </remarks>
        void SetFogParameters(Effect effect)
        {
            var effectFog = effect as IEffectFog;
            if (effectFog == null)
            {
                // ignore.
                return;
            }

            var sceneSettings = Actor.ActorContext.SceneSettings;

            if (sceneSettings.Fog.Enabled)
            {
                effectFog.FogColor = sceneSettings.Fog.Color;
                effectFog.FogStart = sceneSettings.Fog.Start;
                effectFog.FogEnd = sceneSettings.Fog.End;
                effectFog.FogEnabled = true;
            }
            else
            {
                effectFog.FogEnabled = false;
            }
        }

        protected override void OnDraw(GameTime gameTime)
        {
            foreach (var mesh in model.Meshes)
            {
                Matrix world;
                CalculateMeshWorld(mesh, out world);

                foreach (var effect in mesh.Effects)
                {
                    PrepareEffectEachDraw(gameTime, effect, ref world);
                }
                mesh.Draw();
            }
        }

        public override void Draw(GameTime gameTime, Effect effect)
        {
            foreach (var mesh in model.Meshes)
            {
                Matrix world;
                CalculateMeshWorld(mesh, out world);

                var effectMatrices = effect as IEffectMatrices;
                if (effectMatrices != null)
                {
                    effectMatrices.World = world;
                }

                foreach (var part in mesh.MeshParts)
                {
                    GraphicsDevice.Indices = part.IndexBuffer;
                    GraphicsDevice.SetVertexBuffer(part.VertexBuffer, part.VertexOffset);

                    foreach (var pass in effect.CurrentTechnique.Passes)
                    {
                        pass.Apply();
                        GraphicsDevice.DrawIndexedPrimitives(
                            PrimitiveType.TriangleList,
                            0,
                            0,
                            part.NumVertices,
                            part.StartIndex,
                            part.PrimitiveCount);
                    }
                }
            }

            base.Draw(gameTime, effect);
        }

        /// <summary>
        /// ModelMesh の World 行列を計算します。
        /// </summary>
        /// <param name="mesh">ModelMesh。</param>
        /// <param name="result">ModelMesh の World 行列。</param>
        /// <remarks>
        /// このクラスの実装では、ModelMesh の AbsoluteBoneTransform へ Actor の Transform 行列を乗算して返します。
        /// </remarks>
        protected virtual void CalculateMeshWorld(ModelMesh mesh, out Matrix result)
        {
            Matrix.Multiply(ref absoluteBoneTransforms[mesh.ParentBone.Index], ref Actor.Transform, out result);
        }

        /// <summary>
        /// Draw メソッドの呼び出しで使用する View/Projection 行列を計算します。
        /// </summary>
        /// <param name="view"></param>
        /// <param name="projection"></param>
        /// <remarks>
        /// このクラスの実装では、シーンのアクティブ カメラから View/Projection 行列を取得して返します。
        /// </remarks>
        protected virtual void CalculateViewProjection(out Matrix view, out Matrix projection)
        {
            var pov = ActorContext.ActiveCamera.Pov;
            view = pov.View;
            projection = pov.Projection;
        }
    }
}
