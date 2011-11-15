#region Using

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Willcraftia.Xna.Framework.Graphics;
using Willcraftia.Xna.Foundation.Debugs;
using Willcraftia.Xna.Foundation.Graphics;

#endregion

namespace Willcraftia.Xna.Foundation.Scenes.Renders
{
    public sealed class SceneRenderer : IRenderContext, IDisposable
    {
        #region Fields and Properties

        public const int DefaultCloudMapSize = 256;

        IRenderableScene scene;
        public IRenderableScene Scene
        {
            get { return scene; }
        }

        bool contentLoaded;
        public bool ContentLoaded
        {
            get { return contentLoaded; }
        }

        SpriteBatch spriteBatch;
        BackBuffer sceneMapBackBuffer;
        BackBuffer postProcessBackBuffer;
        BackBuffer cloudMapBackBuffer;
        BackBuffer cloudLayerMapBackBuffer;
        BackBufferManager backBufferManager;

        int cloudMapSize = DefaultCloudMapSize;

        SingleScreenSpaceShadow singleScreenSpaceShadow;
        PssmScreenSpaceShadow pssmScreenSpaceShadow;
        ScreenSpaceShadow ScreenSpaceShadow;

        VolumeFog volumeFog;

        Dof dof;
        EdgeDetection edgeDetection;
        GodRay godRay;
        Monochrome monochrome;
        Ssao ssao;
        Bloom bloom;
        ColorOverlap colorOverlap;

        BoundingFrustum cameraFrustum = new BoundingFrustum(Matrix.Identity);
        Vector3[] frustumCorners = new Vector3[BoundingFrustum.CornerCount];
        List<Vector3> sceneBoundingBoxCorners = new List<Vector3>();
        BoundingBox sceneBoundingBox;

        List<Actor> renderTargetPreparedActors = new List<Actor>();

        List<IDrawCallback> drawCallbacks = new List<IDrawCallback>();
        public IList<IDrawCallback> DrawCallbacks
        {
            get { return drawCallbacks; }
        }

        List<IDrawCallback> renderTargetPreparedDrawCallbacks = new List<IDrawCallback>();
        public IList<IDrawCallback> RenderTargetPreparedDrawCallbacks
        {
            get { return renderTargetPreparedDrawCallbacks; }
        }

        List<IDrawCallback> shadowMapDrawCallbacks = new List<IDrawCallback>();
        public IList<IDrawCallback> ShadowMapDrawCallbacks
        {
            get { return shadowMapDrawCallbacks; }
        }

        List<IDrawCallback> shadowSceneMapDrawCallbacks = new List<IDrawCallback>();
        public IList<IDrawCallback> ShadowSceneMapDrawCallbacks
        {
            get { return shadowSceneMapDrawCallbacks; }
        }

        List<IDrawCallback> sceneMapDrawCallbacks = new List<IDrawCallback>();
        public IList<IDrawCallback> SceneMapDrawCallbacks
        {
            get { return sceneMapDrawCallbacks; }
        }

        List<IDrawCallback> shadowPostProcessDrawCallbacks = new List<IDrawCallback>();
        public IList<IDrawCallback> ShadowPostProcessDrawCallbacks
        {
            get { return shadowPostProcessDrawCallbacks; }
        }

        List<IDrawCallback> ssaoDrawCallbacks = new List<IDrawCallback>();
        public IList<IDrawCallback> SsaoDrawCallbacks
        {
            get { return ssaoDrawCallbacks; }
        }

        List<IDrawCallback> dofDrawCallbacks = new List<IDrawCallback>();
        public IList<IDrawCallback> DofDrawCallbacks
        {
            get { return dofDrawCallbacks; }
        }

        List<IDrawCallback> finalScreenDrawCallbacks = new List<IDrawCallback>();
        public IList<IDrawCallback> FinalScreenDrawCallbacks
        {
            get { return finalScreenDrawCallbacks; }
        }

        DebugRenderer debugRenderer;
        public DebugRenderer DebugRenderer
        {
            get { return debugRenderer; }
        }

        Grid grid;

        #endregion

        public SceneRenderer(IRenderableScene scene)
        {
            if (scene == null) throw new ArgumentNullException("scene");

            this.scene = scene;
        }

        #region LoadContent

        public void LoadContent()
        {
            if (contentLoaded) return;

            spriteBatch = new SpriteBatch(GraphicsDevice);

            #region BackBuffer の初期化

            var pp = GraphicsDevice.PresentationParameters;
            var viewport = GraphicsDevice.Viewport;

            backBufferManager = new BackBufferManager(GraphicsDevice);

            sceneMapBackBuffer = backBufferManager.Load("SceneMap");
            sceneMapBackBuffer.Width = viewport.Width;
            sceneMapBackBuffer.Height = viewport.Height;
            sceneMapBackBuffer.MipMap = false;
            sceneMapBackBuffer.SurfaceFormat = pp.BackBufferFormat;
            sceneMapBackBuffer.DepthFormat = DepthFormat.Depth24Stencil8;
            sceneMapBackBuffer.MultiSampleCount = pp.MultiSampleCount;
            sceneMapBackBuffer.RenderTargetUsage = pp.RenderTargetUsage;
            sceneMapBackBuffer.Enabled = true;

            postProcessBackBuffer = backBufferManager.Load("PostProcess");
            postProcessBackBuffer.Width = viewport.Width;
            postProcessBackBuffer.Height = viewport.Height;
            postProcessBackBuffer.MipMap = false;
            postProcessBackBuffer.SurfaceFormat = pp.BackBufferFormat;
            postProcessBackBuffer.DepthFormat = DepthFormat.Depth24Stencil8;
            postProcessBackBuffer.MultiSampleCount = pp.MultiSampleCount;
            postProcessBackBuffer.RenderTargetUsage = pp.RenderTargetUsage;
            postProcessBackBuffer.Enabled = true;

            cloudMapBackBuffer = backBufferManager.Load("CloudMap");
            cloudMapBackBuffer.Width = cloudMapSize;
            cloudMapBackBuffer.Height = cloudMapSize;
            cloudMapBackBuffer.MipMap = true;
            cloudMapBackBuffer.SurfaceFormat = SurfaceFormat.Color;
            cloudMapBackBuffer.DepthFormat = DepthFormat.Depth24Stencil8;
            cloudMapBackBuffer.MultiSampleCount = 0;
            cloudMapBackBuffer.Enabled = true;

            cloudLayerMapBackBuffer = backBufferManager.Load("CloudLayerMap");
            cloudLayerMapBackBuffer.Width = cloudMapSize;
            cloudLayerMapBackBuffer.Height = cloudMapSize;
            cloudLayerMapBackBuffer.MipMap = true;
            cloudLayerMapBackBuffer.SurfaceFormat = SurfaceFormat.Color;
            cloudLayerMapBackBuffer.DepthFormat = DepthFormat.Depth24Stencil8;
            cloudLayerMapBackBuffer.MultiSampleCount = 0;
            cloudLayerMapBackBuffer.RenderTargetCount = 3;
            cloudLayerMapBackBuffer.Enabled = true;

            #endregion

            #region ポストプロセス機能の初期化

            #region ScreenSpaceShadow

            singleScreenSpaceShadow = new SingleScreenSpaceShadow(this);
            singleScreenSpaceShadow.Initialize();
            pssmScreenSpaceShadow = new PssmScreenSpaceShadow(this);
            pssmScreenSpaceShadow.Initialize();
            switch (scene.SceneSettings.ShadowSettings.Shape)
            {
                case LightFrustumShape.Pssm:
                    pssmScreenSpaceShadow.Enabled = true;
                    break;
                case LightFrustumShape.Lspsm:
                case LightFrustumShape.Usm:
                default:
                    singleScreenSpaceShadow.Enabled = true;
                    break;
            }

            #endregion

            //
            // TODO
            //
            volumeFog = new VolumeFog(this);
            volumeFog.Initialize();
            volumeFog.Enabled = true;

            #region DoF

            dof = new Dof(this);
            dof.Initialize();
            dof.Enabled = false;

            #endregion

            #region God ray

            godRay = new GodRay(this);
            godRay.Initialize();
            godRay.Enabled = false;

            #endregion

            #region Edge detection

            edgeDetection = new EdgeDetection(this);
            edgeDetection.Initialize();
            edgeDetection.Enabled = false;

            #endregion

            #region Monochrome

            monochrome = new Monochrome(this);
            monochrome.Initialize();
            monochrome.Enabled = false;

            #endregion

            #region SSAO

            ssao = new Ssao(this);
            ssao.Initialize();
            ssao.Enabled = false;

            #endregion

            #region Bloom

            bloom = new Bloom(this);
            bloom.Initialize();
            bloom.Enabled = false;

            #endregion

            #region ColorOverlap

            colorOverlap = new ColorOverlap(this);
            colorOverlap.Initialize();
            colorOverlap.Enabled = false;

            #endregion

            #endregion

            #region DebugRenderer の初期化

            debugRenderer = new DebugRenderer(this);
            debugRenderer.Initialize();
            debugRenderer.Enabled = true;

            #endregion

            grid = new Grid(GraphicsDevice);

            contentLoaded = true;
        }

        #endregion

        #region UnloadContent

        public void UnloadContent()
        {
            if (!contentLoaded) return;

            spriteBatch.Dispose();

            backBufferManager.Dispose();

            singleScreenSpaceShadow.Dispose();
            pssmScreenSpaceShadow.Dispose();

            dof.Dispose();
            edgeDetection.Dispose();
            monochrome.Dispose();
            godRay.Dispose();
            ssao.Dispose();
            bloom.Dispose();

            debugRenderer.Dispose();

            // TODO: 必要なの？
            drawCallbacks.Clear();

            contentLoaded = false;
        }

        #endregion

        #region Draw

        public void Draw(GameTime gameTime)
        {
            CallOnDrawStarted(gameTime, drawCallbacks);

            // 状態再構築の可能性のために必要に応じて再ロードします。
            LoadContent();

            // アクティブ カメラを取得します。
            var pov = scene.ActiveCamera.Pov;

            // カメラの視錐台を初期化します。
            cameraFrustum.Matrix = pov.ViewProjection;

            // 可読性向上のため。
            var sceneSettings = scene.SceneSettings;
            var shadowSettings = sceneSettings.ShadowSettings;

            #region アクティブ カメラから見える Actor の収集

            // 各リストを初期化します。
            visibleCharacters.Clear();
            visibleStaticMeshes.Clear();
            visibleTerrains.Clear();
            visibleFluidSurfaces.Clear();
            visibleSkyDomes.Clear();
            visibleVolumeFogs.Clear();

            // アクティブ カメラから見える Actor を収集します。
            scene.Characters.FindAll(IsCameraVisible, visibleCharacters);
            scene.StaticMeshes.FindAll(IsCameraVisible, visibleStaticMeshes);
            scene.Terrains.FindAll(IsCameraVisible, visibleTerrains);
            scene.FluidSurfaces.FindAll(IsCameraVisible, visibleFluidSurfaces);
            scene.SkyDomes.FindAll(IsCameraVisible, visibleSkyDomes);
            scene.VolumeFogs.FindAll(IsCameraVisible, visibleVolumeFogs);

            // CharacterActor はカメラからの距離でソートしておきます。
            visibleCharacters.Sort(CompareActorDistance);

            #endregion

            #region PostProcessVolumeActor の設定をポストプロセスへ反映

            bool dofEnabled = false;
            bool ssaoEnabled = false;
            bool godRayEnabled = false;
            bool monochromeEnabled = false;
            bool edgeDetectionEnabled = false;
            bool bloomEnabled = false;
            bool colorOverlapEnabled = false;
            foreach (var volume in scene.PostProcessVolumes)
            {
                if (!volume.Active)
                {
                    continue;
                }

                if (volume.Settings.DofEnabled)
                {
                    dof.Settings = volume.Settings.DofSettings;
                    dofEnabled = true;
                }

                //if (volume.Settings.SsaoEnabled)
                //{
                //    Ssao.Settings = volume.Settings.SsaoSettings;
                //    ssaoEnabled = true;
                //}

                if (volume.Settings.GodRayEnabled)
                {
                    godRay.Settings = volume.Settings.GodRaySettings;
                    godRayEnabled = true;
                }

                if (volume.Settings.MonochromeEnabled)
                {
                    monochrome.Settings = volume.Settings.MonochromeSettings;
                    monochromeEnabled = true;
                }

                if (volume.Settings.EdgeDetectionEnabled)
                {
                    edgeDetection.Settings = volume.Settings.EdgeDetectionSettings;
                    edgeDetectionEnabled = true;
                }

                if (volume.Settings.BloomEnabled)
                {
                    bloom.Settings = volume.Settings.BloomSettings;
                    bloomEnabled = true;
                }

                if (volume.Settings.ColorOverlapEnabled)
                {
                    colorOverlap.Settings = volume.Settings.ColorOverlapSettings;
                    colorOverlapEnabled = true;
                }
            }
            dof.Enabled = dofEnabled;
            ssao.Enabled = ssaoEnabled;
            godRay.Enabled = godRayEnabled;
            monochrome.Enabled = monochromeEnabled;
            edgeDetection.Enabled = edgeDetectionEnabled;
            bloom.Enabled = bloomEnabled;
            colorOverlap.Enabled = colorOverlapEnabled;

            #endregion

            #region DebugRenderer へ VolumeActor のワイヤフレームを登録

            scene.PhysicsVolumes.ForEach(RegisterVolumeToDebugRenderer);
            scene.PostProcessVolumes.ForEach(RegisterVolumeToDebugRenderer);
            
            #endregion

            //
            // prepare light volume and collect visible models
            //
            sceneBoundingBoxCorners.Clear();

            cameraFrustum.GetCorners(frustumCorners);
            sceneBoundingBoxCorners.AddRange(frustumCorners);

            sceneBoundingBox = BoundingBox.CreateFromPoints(sceneBoundingBoxCorners);

            #region GraphicsDevice の初期化

            GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            GraphicsDevice.BlendState = BlendState.Opaque;

            #endregion

            #region Screen Space Shadow Maps (SSSM) の描画

            if (shadowSettings.Enabled && sceneSettings.DirectionalLight0.ShadowEnabled)
            {
                #region SSSM 機能の設定

                Vector3 povPosition;
                Matrix povOrientation;
                pov.GetPosition(out povPosition);
                pov.GetOrientation(out povOrientation);

                var lspsmPov = singleScreenSpaceShadow.LspsmLightCamera.Pov;
                lspsmPov.SetPosition(ref povPosition);
                lspsmPov.SetOrientation(ref povOrientation);
                lspsmPov.Fov = shadowSettings.Fov;
                lspsmPov.AspectRatio = shadowSettings.AspectRatio;
                lspsmPov.NearPlaneDistance = shadowSettings.NearPlaneDistance;
                lspsmPov.FarPlaneDistance = shadowSettings.FarPlaneDistance;
                lspsmPov.Update(false);
                singleScreenSpaceShadow.LspsmLightCamera.LightDirection = sceneSettings.DirectionalLight0.Direction;

                var pssmPov = pssmScreenSpaceShadow.Pssm.Pov;
                pssmPov.SetPosition(ref povPosition);
                pssmPov.SetOrientation(ref povOrientation);
                pssmPov.Fov = shadowSettings.Fov;
                pssmPov.AspectRatio = shadowSettings.AspectRatio;
                pssmPov.NearPlaneDistance = shadowSettings.NearPlaneDistance;
                pssmPov.FarPlaneDistance = shadowSettings.FarPlaneDistance;
                pssmPov.Update(false);
                pssmScreenSpaceShadow.Pssm.LightDirection = sceneSettings.DirectionalLight0.Direction;
                pssmScreenSpaceShadow.Pssm.SplitCount = shadowSettings.Pssm.SplitCount;
                pssmScreenSpaceShadow.Pssm.SplitLambda = shadowSettings.Pssm.SplitLambda;
                pssmScreenSpaceShadow.Pssm.ShadowMapSize = shadowSettings.Size;

                #endregion

                #region 使用する LightFrustumShape に応じた SSSM 機能の設定

                switch (shadowSettings.Shape)
                {
                    case LightFrustumShape.Pssm:
                        singleScreenSpaceShadow.Enabled = false;
                        pssmScreenSpaceShadow.Enabled = true;
                        ScreenSpaceShadow = pssmScreenSpaceShadow;
                        break;
                    case LightFrustumShape.Lspsm:
                        singleScreenSpaceShadow.LspsmLightCamera.LspsmEnabled = true;
                        singleScreenSpaceShadow.Enabled = true;
                        pssmScreenSpaceShadow.Enabled = false;
                        ScreenSpaceShadow = singleScreenSpaceShadow;
                        break;
                    case LightFrustumShape.Usm:
                    default:
                        singleScreenSpaceShadow.LspsmLightCamera.LspsmEnabled = false;
                        singleScreenSpaceShadow.Enabled = true;
                        pssmScreenSpaceShadow.Enabled = false;
                        ScreenSpaceShadow = singleScreenSpaceShadow;
                        break;
                }

                #endregion

                #region Shadow Maps の描画

                CallOnDrawStarted(gameTime, shadowMapDrawCallbacks);

                ScreenSpaceShadow.PrepareShadowMap(gameTime);

                CallOnDrawCompleted(gameTime, shadowMapDrawCallbacks);

                #endregion

                #region Shadow Scene Map の描画

                CallOnDrawStarted(gameTime, shadowSceneMapDrawCallbacks);

                ScreenSpaceShadow.PrepareShadowSceneMap(gameTime);

                CallOnDrawCompleted(gameTime, shadowSceneMapDrawCallbacks);

                #endregion
            }

            #endregion

            #region ActorModel の RenderTarget の準備

            CallOnDrawStarted(gameTime, renderTargetPreparedDrawCallbacks);

            // リストを初期化します。
            renderTargetPreparedActors.Clear();

            // IRenderTargetPrepared を実装する ActorModel を持った Actor を収集します。
            visibleCharacters.FindAll(HasRenderTargetPrepared, renderTargetPreparedActors);
            visibleStaticMeshes.FindAll(HasRenderTargetPrepared, renderTargetPreparedActors);
            visibleTerrains.FindAll(HasRenderTargetPrepared, renderTargetPreparedActors);
            visibleFluidSurfaces.FindAll(HasRenderTargetPrepared, renderTargetPreparedActors);
            visibleFluidSurfaces.FindAll(HasRenderTargetPrepared, renderTargetPreparedActors);
            visibleVolumeFogs.FindAll(HasRenderTargetPrepared, renderTargetPreparedActors);

            // ActorModel の RenderTarget を準備します。
            foreach (var actor in renderTargetPreparedActors)
            {
                var renderTargetPrepared = actor.ActorModel as IRenderTargetPrepared;
                renderTargetPrepared.PrepareRenderTargets(gameTime);
            }

            CallOnDrawCompleted(gameTime, renderTargetPreparedDrawCallbacks);

            #endregion

            #region シーンの描画

            CallOnDrawStarted(gameTime, sceneMapDrawCallbacks);

            PrepareScene(gameTime, sceneMapBackBuffer);

            CallOnDrawCompleted(gameTime, sceneMapDrawCallbacks);

            #endregion

            #region ポストプロセス

            #region Volume fog
            {
                var source = sceneMapBackBuffer;
                var destination = postProcessBackBuffer;

                volumeFog.Prepare(gameTime);
                volumeFog.Filter(source.GetRenderTarget(), destination);

                sceneMapBackBuffer = destination;
                postProcessBackBuffer = source;
            }
            #endregion

            #region Screen space shadow

            if (shadowSettings.Enabled && sceneSettings.DirectionalLight0.ShadowEnabled)
            {
                CallOnDrawStarted(gameTime, shadowPostProcessDrawCallbacks);

                var source = sceneMapBackBuffer;
                var destination = postProcessBackBuffer;

                ScreenSpaceShadow.Enabled = true;
                ScreenSpaceShadow.Filter(source.GetRenderTarget(), destination);

                sceneMapBackBuffer = destination;
                postProcessBackBuffer = source;

                CallOnDrawCompleted(gameTime, shadowPostProcessDrawCallbacks);
            }

            #endregion

            #region SSAO

            if (ssao.Enabled)
            {
                CallOnDrawStarted(gameTime, ssaoDrawCallbacks);

                ssao.PrepareNormalDepthMap(gameTime);
                ssao.PrepareSsaoMap(gameTime);
                ssao.PrepareBluredSsaoMap(gameTime);

                var source = sceneMapBackBuffer;
                var destination = postProcessBackBuffer;

                ssao.Filter(source.GetRenderTarget(), destination);

                sceneMapBackBuffer = destination;
                postProcessBackBuffer = source;

                CallOnDrawCompleted(gameTime, ssaoDrawCallbacks);
            }

            #endregion

            #region Bloom

            if (bloom.Enabled)
            {
                var source = sceneMapBackBuffer;
                var destination = postProcessBackBuffer;

                bloom.Filter(source.GetRenderTarget(), destination);

                sceneMapBackBuffer = destination;
                postProcessBackBuffer = source;
            }

            #endregion

            #region Edge detection

            if (edgeDetection.Enabled)
            {
                edgeDetection.PrepareNormalDepthMap(gameTime);

                var source = sceneMapBackBuffer;
                var destination = postProcessBackBuffer;

                edgeDetection.Filter(source.GetRenderTarget(), destination);

                sceneMapBackBuffer = destination;
                postProcessBackBuffer = source;
            }

            #endregion

            #region God ray

            if (godRay.Enabled)
            {
                var source = sceneMapBackBuffer;
                var destination = postProcessBackBuffer;

                godRay.PrepareOcclusionMap(gameTime);
                godRay.Filter(source.GetRenderTarget(), destination.GetRenderTarget());

                sceneMapBackBuffer = destination;
                postProcessBackBuffer = source;
            }

            #endregion

            #region Depth of field

            if (dof.Enabled)
            {
                CallOnDrawStarted(gameTime, dofDrawCallbacks);

                dof.PrepareDepthMap(gameTime);

                var source = sceneMapBackBuffer;
                var destination = postProcessBackBuffer;

                dof.Filter(source.GetRenderTarget(), destination);

                sceneMapBackBuffer = destination;
                postProcessBackBuffer = source;

                CallOnDrawCompleted(gameTime, dofDrawCallbacks);
            }

            #endregion

            #region Monochrome

            if (monochrome.Enabled)
            {
                var source = sceneMapBackBuffer;
                var destination = postProcessBackBuffer;

                monochrome.Filter(source.GetRenderTarget(), destination);

                sceneMapBackBuffer = destination;
                postProcessBackBuffer = source;
            }

            #endregion

            #region ColorOverlap

            if (colorOverlap.Enabled)
            {
                var source = sceneMapBackBuffer;
                var destination = postProcessBackBuffer;

                colorOverlap.Filter(source.GetRenderTarget(), destination);

                sceneMapBackBuffer = destination;
                postProcessBackBuffer = source;
            }

            #endregion

            #endregion

            #region 描画結果のスクリーンへの反映

            CallOnDrawStarted(gameTime, finalScreenDrawCallbacks);

            DrawFinalMapToScreen(gameTime);

            CallOnDrawCompleted(gameTime, finalScreenDrawCallbacks);

            #endregion

            CallOnDrawCompleted(gameTime, drawCallbacks);
        }

        /// <summary>
        /// IDrawCallback リストの各要素に対して OnDrawStarted を呼び出すヘルパ メソッドです。
        /// </summary>
        /// <param name="gameTime">前回の Update が呼び出されてからの経過時間。</param>
        /// <param name="callbacks">IDrawCallback リスト。</param>
        void CallOnDrawStarted(GameTime gameTime, IList<IDrawCallback> callbacks)
        {
            foreach (var callback in callbacks)
            {
                callback.OnDrawStarted(gameTime);
            }
        }

        /// <summary>
        /// IDrawCallback リストの各要素に対して OnDrawCompleted を呼び出すヘルパ メソッドです。
        /// </summary>
        /// <param name="gameTime">前回の Update が呼び出されてからの経過時間。</param>
        /// <param name="callbacks">IDrawCallback リスト。</param>
        void CallOnDrawCompleted(GameTime gameTime, IList<IDrawCallback> callbacks)
        {
            foreach (var callback in callbacks)
            {
                callback.OnDrawCompleted(gameTime);
            }
        }

        void RegisterVolumeToDebugRenderer(VolumeActor volume)
        {
            BoundingBox box;
            volume.GetBoundingBox(out box);
            bool intersected;
            cameraFrustum.Intersects(ref box, out intersected);
            if (intersected)
            {
                debugRenderer.Volumes.Add(volume);
            }
        }

        #region Comparision

        int CompareActorDistance(Actor x, Actor y)
        {
            if (x == y)
            {
                return 0;
            }

            var pov = scene.ActiveCamera.Pov;

            Vector3 xPosition;
            Vector3.Transform(ref x.Position, ref pov.View, out xPosition);

            Vector3 yPosition;
            Vector3.Transform(ref y.Position, ref pov.View, out yPosition);

            if (xPosition.Z == yPosition.Z)
            {
                return 0;
            }
            else if (xPosition.Z < yPosition.Z)
            {
                return -1;
            }
            else
            {
                return 1;
            }
        }

        #endregion

        #region Predicate

        bool IsCameraVisible(Actor actor)
        {
            if (!actor.Visible) return false;

            var actorModel = actor.ActorModel;

            if (actorModel.MaxDrawDistance <= 0)
            {
                return cameraFrustum.Intersects(actorModel.WorldBoundingBox);
                //return true;
            }

            var maxDistanceSquared = actorModel.MaxDrawDistance * actorModel.MaxDrawDistance;
            var cameraToActor = scene.ActiveCamera.Position - actor.Position;

            if (cameraToActor.LengthSquared() <= maxDistanceSquared)
            {
                return cameraFrustum.Intersects(actorModel.WorldBoundingBox);
            }
            else
            {
                return false;
            }
        }

        bool HasRenderTargetPrepared(Actor actor)
        {
            if (actor.ActorModel == null)
            {
                return false;
            }

            return actor.ActorModel is IRenderTargetPrepared;
        }

        #endregion

        void PrepareScene(GameTime gameTime, BackBuffer destination)
        {
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            GraphicsDevice.BlendState = BlendState.Opaque;

            destination.Begin();
            {
                GraphicsDevice.Clear(Color.CornflowerBlue);

                // StaticMeshActor を描画します。
                DrawActors<StaticMeshActor>(gameTime, visibleStaticMeshes);

                // TODO: 描画順序を色々検討したい。
                // TODO: Terrain を SkyDome の後に描画しようとすると描画が壊れる。
                // TODO: StaticMeshActor を SkyDome の後に描画しようとすると描画が壊れる。

                // TerrainActor を描画します。
                DrawActors<TerrainActor>(gameTime, visibleTerrains);

                // CharacterActor を描画します。
                // FluidSurfaceActor は透過する可能性があるので最後に描画します。
                DrawActors<CharacterActor>(gameTime, visibleCharacters);

                // FluidSurfaceActor を描画します。
                // FluidSurfaceActor は透過する可能性があるので最後に描画します。
                DrawActors<FluidSurfaceActor>(gameTime, visibleFluidSurfaces);

                // SkyDomeActor を描画します。
                // TODO: 描画順序の問題から最後。
                // でも、この状態だと CharactorActor に接近した時の透過処理で SkyDomeActor が除外されてしまう。
                DrawActors<SkyDomeActor>(gameTime, visibleSkyDomes);

                // TODO: 非 DEBUG 時の制御を書かないと。

                // StaticMeshActor のワイヤフレームを DebugRenderer へ登録します。
                RegisterDebugCollisionBoundsWireframes<StaticMeshActor>(visibleStaticMeshes);

                // CharacterActor のワイヤフレームを DebugRenderer へ登録します。
                RegisterDebugCollisionBoundsWireframes<CharacterActor>(visibleCharacters);

                var pov = scene.ActiveCamera.Pov;
                grid.View = pov.View;
                grid.Projection = pov.Projection;
                grid.Draw();

                // DebugRenderer の描画は必ず一番最後で行います。
                debugRenderer.Draw(gameTime);
            }
            destination.End();
        }

        /// <summary>
        /// Actor リストの各要素に対して ActorModel プロパティの Draw メソッドを呼び出すヘルパ メソッドです。
        /// </summary>
        /// <typeparam name="T">Actor の型。</typeparam>
        /// <param name="gameTime">前回の Update が呼び出されてからの経過時間。</param>
        /// <param name="actors">Actor のリスト。</param>
        void DrawActors<T>(GameTime gameTime, List<T> actors) where T : Actor
        {
            foreach (var actor in actors)
            {
                actor.ActorModel.Draw(gameTime);
            }
        }

        /// <summary>
        /// リスト内の Actor の衝突境界のワイヤフレームを DebugRenderer に登録します。
        /// </summary>
        /// <typeparam name="T">Actor の型。</typeparam>
        /// <param name="actors">Actor のリスト。</param>
        void RegisterDebugCollisionBoundsWireframes<T>(IList<T> actors) where T : Actor
        {
            foreach (var actor in actors)
            {
                if (actor.CollisionBounds == null) continue;

                var wf = actor.CollisionBounds.GetDebugWireframe();
                for (int i = 0; i < wf.Length; i++)
                {
                    ReplaceWireframeColor(ref wf[i], ref debugRenderer.CollisionColor);
                }
                debugRenderer.AddVertices(wf);
            }
        }

        void ReplaceWireframeColor(ref VertexPositionColor vpc, ref Color color)
        {
            vpc.Color.R = color.R;
            vpc.Color.G = color.G;
            vpc.Color.B = color.B;
            vpc.Color *= vpc.Color.A;
        }

        void DrawFinalMapToScreen(GameTime gameTime)
        {
            SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Opaque);
            SpriteBatch.Draw(sceneMapBackBuffer.GetRenderTarget(), Vector2.Zero, Color.White);
            SpriteBatch.End();
        }

        #endregion

        #region IRenderContext

        public ContentManager Content
        {
            get { return scene.SceneContext.LocalContent; }
        }

        public SpriteBatch SpriteBatch
        {
            get { return spriteBatch; }
        }

        List<CharacterActor> visibleCharacters = new List<CharacterActor>();
        public IEnumerable<CharacterActor> VisibleCharacters
        {
            get { return visibleCharacters; }
        }

        List<StaticMeshActor> visibleStaticMeshes = new List<StaticMeshActor>();
        public IEnumerable<StaticMeshActor> VisibleStaticMeshes
        {
            get { return visibleStaticMeshes; }
        }

        List<TerrainActor> visibleTerrains = new List<TerrainActor>();
        public IEnumerable<TerrainActor> VisibleTerrains
        {
            get { return visibleTerrains; }
        }

        List<FluidSurfaceActor> visibleFluidSurfaces = new List<FluidSurfaceActor>();
        public IEnumerable<FluidSurfaceActor> VisibleFluidSurfaces
        {
            get { return visibleFluidSurfaces; }
        }

        List<SkyDomeActor> visibleSkyDomes = new List<SkyDomeActor>();
        public IEnumerable<SkyDomeActor> VisibleSkyDomes
        {
            get { return visibleSkyDomes; }
        }

        List<VolumeFogActor> visibleVolumeFogs = new List<VolumeFogActor>();
        public IEnumerable<VolumeFogActor> VisibleVolumeFogs
        {
            get { return visibleVolumeFogs; }
        }

        #endregion

        #region IGameContext

        public GraphicsDevice GraphicsDevice
        {
            get { return scene.SceneContext.GraphicsDevice; }
        }

        public void ResetElapsedTime()
        {
            scene.SceneContext.ResetElapsedTime();
        }

        #endregion

        #region IServiceProvider

        public object GetService(Type serviceType)
        {
            return scene.SceneContext.GetService(serviceType);
        }

        #endregion

        #region IDisposable

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        bool disposed;

        ~SceneRenderer()
        {
            Dispose(false);
        }

        void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    UnloadContent();
                }
                disposed = true;
            }
        }

        #endregion
    }
}
