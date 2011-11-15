#region Using

using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Willcraftia.Xna.Foundation.Scenes;

#endregion

namespace Willcraftia.Content.Studio.Plugin.Foundation.Models
{
    public sealed class SceneConfigEdit : IRenderableScene
    {
        #region Inner classes

        public sealed class ActorFactoryContext : IActorFactoryContext
        {
            SceneConfigEdit owner;

            public ActorFactoryContext(SceneConfigEdit owner)
            {
                this.owner = owner;
            }

            #region IActorFactoryContext

            public ContentManager Content
            {
                get { return owner.SceneContext.LocalContent; }
            }

            #endregion
        }

        public sealed class ActorContext : IActorContext
        {
            SceneConfigEdit owner;

            public ActorContext(SceneConfigEdit owner)
            {
                this.owner = owner;
            }

            #region IActorContext

            /// <summary>
            /// Screen の LocalContent (Screen 毎の ContentManager) を返します。
            /// </summary>
            public ContentManager Content
            {
                get { return owner.SceneContext.LocalContent; }
            }

            public SceneSettings SceneSettings
            {
                get { return owner.SceneSettings; }
            }

            public CameraActor ActiveCamera
            {
                get { return owner.ActiveCamera; }
            }

            #endregion

            #region IGameContext

            public GraphicsDevice GraphicsDevice
            {
                get { return owner.SceneContext.GraphicsDevice; }
            }

            public void ResetElapsedTime()
            {
                owner.SceneContext.ResetElapsedTime();
            }

            #endregion

            #region IServiceProvider

            public object GetService(Type serviceType)
            {
                return owner.SceneContext.GetService(serviceType);
            }

            #endregion
        }

        public sealed class PhysicsVolumeContext : IPhysicsVolumeContext
        {
            SceneConfigEdit owner;

            public PhysicsVolumeContext(SceneConfigEdit owner)
            {
                this.owner = owner;
            }

            #region IPhysicsVolumeContext

            public ActorCollection<CharacterActor> Characters
            {
                get { return owner.Characters; }
            }

            #endregion
        }

        #endregion

        SceneConfig sceneConfig;

        public ActorCollection<CameraActor> Cameras { get; private set; }

        Dictionary<string, ActorFactory> actorFactories = new Dictionary<string, ActorFactory>();

        ActorFactoryContext actorFactoryContext;
        ActorContext actorContext;
        PhysicsVolumeContext physicsVolumeContext;

        public bool ContentLoaded { get; private set; }

        public SceneConfigEdit(SceneConfig sceneConfig)
        {
            if (sceneConfig == null) throw new ArgumentNullException("sceneConfig");

            this.sceneConfig = sceneConfig;

            actorFactoryContext = new ActorFactoryContext(this);
            actorContext = new ActorContext(this);
            physicsVolumeContext = new PhysicsVolumeContext(this);

            Cameras = new ActorCollection<CameraActor>();
            Characters = new ActorCollection<CharacterActor>();
            StaticMeshes = new ActorCollection<StaticMeshActor>();
            Terrains = new ActorCollection<TerrainActor>();
            FluidSurfaces = new ActorCollection<FluidSurfaceActor>();
            SkyDomes = new ActorCollection<SkyDomeActor>();
            VolumeFogs = new ActorCollection<VolumeFogActor>();
            PhysicsVolumes = new ActorCollection<PhysicsVolumeActor>();
            PostProcessVolumes = new ActorCollection<PostProcessVolumeActor>();
        }

        public void LoadContent()
        {
            if (SceneContext == null) throw new InvalidOperationException("SceneContext is null.");
            if (ContentLoaded) return;

            LoadActors();

            ContentLoaded = true;
        }

        /// <summary>
        /// SceneConfig で指定された ActorConfig から Actor をロードします。
        /// </summary>
        void LoadActors()
        {
            // Actor を生成して追加します。
            foreach (var config in sceneConfig.Actors)
            {
                // Actor を生成します。
                var actor = CreateActor(config);

                // Actor を適切なリストへ追加します。
                RegisterActor(actor);
            }

            // Actor のコンテンツをロードします。
            LoadContent<CameraActor>(Cameras);
            LoadContent<CharacterActor>(Characters);
            LoadContent<StaticMeshActor>(StaticMeshes);
            LoadContent<TerrainActor>(Terrains);
            LoadContent<FluidSurfaceActor>(FluidSurfaces);
            LoadContent<SkyDomeActor>(SkyDomes);
            LoadContent<VolumeFogActor>(VolumeFogs);
            LoadContent<PhysicsVolumeActor>(PhysicsVolumes);
            LoadContent<PostProcessVolumeActor>(PostProcessVolumes);
        }

        /// <summary>
        /// ActorConfig に基づいて Actor を生成します。
        /// </summary>
        /// <param name="actorConfig">ActorConfig。</param>
        /// <returns>生成された Actor。</returns>
        /// <remarks>
        /// 生成される Actor は、まだ ActionContext は設定されておらず、コンテンツのロードも行われていません。
        /// </remarks>
        public Actor CreateActor(ActorConfig actorConfig)
        {
            // ActorFactory を取得します。
            var factory = GetActorFactory(actorConfig);

            // Actor を生成します。
            var actor = factory.CreateActor(actorConfig);

            return actor;
        }

        /// <summary>
        /// ActorConfig で指定された ActorFactory を取得します。
        /// </summary>
        /// <param name="actorConfig">ActorFactory。</param>
        /// <returns>ActorFactory。</returns>
        ActorFactory GetActorFactory(ActorConfig actorConfig)
        {
            ActorFactory factory;
            if (!actorFactories.TryGetValue(actorConfig.Factory, out factory))
            {
                // まだ ActorFactory インスタンスが存在しないならば生成して登録します。
                var factoryType = GetTypeAcrossAssemblies(actorConfig.Factory);
                var constructor = factoryType.GetConstructor(Type.EmptyTypes);
                factory = constructor.Invoke(null) as ActorFactory;
                factory.ActorFactoryContext = actorFactoryContext;
                actorFactories.Add(actorConfig.Factory, factory);
            }
            return factory;
        }

        /// <summary>
        /// 現在の AppDomain にある全ての Assembly から指定の型を検索します。
        /// </summary>
        /// <param name="typeName">名前空間を含む型名 (Assembly 指定は含まない)。</param>
        /// <returns>検索で見つかった型。</returns>
        Type GetTypeAcrossAssemblies(string typeName)
        {
            Type type = null;
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                type = assembly.GetType(typeName);
                if (type != null)
                {
                    return type;
                }
            }

            throw new ArgumentException(string.Format("Can not resolve the type '{0}'.", typeName));
        }

        /// <summary>
        /// Actor を適切なリストへ追加します。
        /// </summary>
        /// <param name="actor">Actor。</param>
        public void RegisterActor(Actor actor)
        {
            var camera = actor as CameraActor;
            if (camera != null)
            {
                Cameras.Add(camera);
                return;
            }

            var character = actor as CharacterActor;
            if (character != null)
            {
                Characters.Add(character);
                return;
            }

            var staticMesh = actor as StaticMeshActor;
            if (staticMesh != null)
            {
                StaticMeshes.Add(staticMesh);
                return;
            }

            var terrain = actor as TerrainActor;
            if (terrain != null)
            {
                Terrains.Add(terrain);
                return;
            }

            var fluidSurface = actor as FluidSurfaceActor;
            if (fluidSurface != null)
            {
                FluidSurfaces.Add(fluidSurface);
                return;
            }

            var skyDome = actor as SkyDomeActor;
            if (skyDome != null)
            {
                SkyDomes.Add(skyDome);
                return;
            }

            var volumeFog = actor as VolumeFogActor;
            if (volumeFog != null)
            {
                VolumeFogs.Add(volumeFog);
                return;
            }

            var physicsVolume = actor as PhysicsVolumeActor;
            if (physicsVolume != null)
            {
                PhysicsVolumes.Add(physicsVolume);
                return;
            }

            var postProcessVolume = actor as PostProcessVolumeActor;
            if (postProcessVolume != null)
            {
                PostProcessVolumes.Add(postProcessVolume);
                return;
            }

            throw new InvalidOperationException(
                string.Format("Actor type '{0}' is not supported.", actor.GetType().FullName));
        }

        void LoadContent<T>(ActorCollection<T> actors) where T : Actor
        {
            foreach (var actor in actors)
            {
                LoadContent(actor);
            }
        }

        /// <summary>
        /// Actor のコンテンツをロードします。
        /// </summary>
        /// <param name="actor">Actor。</param>
        /// <remarks>
        /// Actor のコンテンツのロード前には ActorContext が設定されます。
        /// </remarks>
        void LoadContent(Actor actor)
        {
            // IActorContext を設定します。
            actor.ActorContext = actorContext;

            // Actor のコンテンツをロードします。
            actor.LoadContent();

            // TODO: もっとスマートに書かなくて大丈夫か？
            if (actor is PhysicsVolumeActor)
            {
                (actor as PhysicsVolumeActor).PhysicsVolumeContext = physicsVolumeContext;
            }
        }

        public void UnloadContent()
        {
            if (!ContentLoaded) return;

            UnloadContent<CameraActor>(Cameras);
            UnloadContent<CharacterActor>(Characters);
            UnloadContent<StaticMeshActor>(StaticMeshes);
            UnloadContent<TerrainActor>(Terrains);
            UnloadContent<FluidSurfaceActor>(FluidSurfaces);
            UnloadContent<SkyDomeActor>(SkyDomes);
            UnloadContent<VolumeFogActor>(VolumeFogs);
            UnloadContent<PhysicsVolumeActor>(PhysicsVolumes);
            UnloadContent<PostProcessVolumeActor>(PostProcessVolumes);

            ContentLoaded = false;
        }

        void UnloadContent<T>(Collection<T> actors) where T : Actor
        {
            foreach (var actor in actors)
            {
                if (actor.ContentLoaded)
                {
                    actor.UnloadContent();
                }
            }

            actors.Clear();
        }

        public void Update(GameTime gameTime)
        {
            Update<CameraActor>(gameTime, Cameras);
            Update<CharacterActor>(gameTime, Characters);
            Update<StaticMeshActor>(gameTime, StaticMeshes);
            Update<TerrainActor>(gameTime, Terrains);
            Update<FluidSurfaceActor>(gameTime, FluidSurfaces);
            Update<SkyDomeActor>(gameTime, SkyDomes);
            Update<VolumeFogActor>(gameTime, VolumeFogs);
            Update<PhysicsVolumeActor>(gameTime, PhysicsVolumes);
            Update<PostProcessVolumeActor>(gameTime, PostProcessVolumes);
        }

        void Update<T>(GameTime gameTime, IEnumerable<T> actors) where T : Actor
        {
            foreach (var actor in actors)
            {
                if (actor.Enabled)
                {
                    actor.Update(gameTime);
                }
            }
        }

        #region IRenderableScene

        public ISceneContext SceneContext { get; set; }

        public SceneSettings SceneSettings
        {
            get { return sceneConfig.SceneSettings; }
        }

        public CameraActor ActiveCamera { get; set; }

        public ActorCollection<CharacterActor> Characters { get; private set; }
        public ActorCollection<StaticMeshActor> StaticMeshes { get; private set; }
        public ActorCollection<TerrainActor> Terrains { get; private set; }
        public ActorCollection<FluidSurfaceActor> FluidSurfaces { get; private set; }
        public ActorCollection<SkyDomeActor> SkyDomes { get; private set; }
        public ActorCollection<VolumeFogActor> VolumeFogs { get; private set; }
        public ActorCollection<PhysicsVolumeActor> PhysicsVolumes { get; private set; }
        public ActorCollection<PostProcessVolumeActor> PostProcessVolumes { get; private set; }

        #endregion
    }
}
