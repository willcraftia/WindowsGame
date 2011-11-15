#region Using

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Willcraftia.Xna.Framework.Audio;
using Willcraftia.Xna.Foundation.Graphics;
using Willcraftia.Xna.Framework.Input;
using Willcraftia.Xna.Framework.Physics;
using Willcraftia.Xna.Foundation.Screens;

#endregion

namespace Willcraftia.Xna.Foundation.Scenes
{
    public sealed class Scene : IRenderableScene
    {
        #region Inner classes

        public sealed class ActorFactoryContext : IActorFactoryContext
        {
            Scene scene;

            public ActorFactoryContext(Scene scene)
            {
                this.scene = scene;
            }

            #region IActorFactoryContext

            /// <summary>
            /// Screen の LocalContent (Screen 毎の ContentManager) を返します。
            /// </summary>
            public ContentManager Content
            {
                get { return scene.sceneContext.LocalContent; }
            }

            #endregion
        }

        public sealed class ActorContext : IActorContext
        {
            Scene scene;

            public ActorContext(Scene scene)
            {
                this.scene = scene;
            }

            #region IActorContext

            /// <summary>
            /// Screen の LocalContent (Screen 毎の ContentManager) を返します。
            /// </summary>
            public ContentManager Content
            {
                get { return scene.sceneContext.LocalContent; }
            }

            public SceneSettings SceneSettings
            {
                get { return scene.SceneSettings; }
            }

            public CameraActor ActiveCamera
            {
                get { return scene.activeCamera; }
            }

            #endregion

            #region IGameContext

            public GraphicsDevice GraphicsDevice
            {
                get { return scene.sceneContext.GraphicsDevice; }
            }

            public void ResetElapsedTime()
            {
                scene.sceneContext.ResetElapsedTime();
            }

            #endregion

            #region IServiceProvider

            public object GetService(Type serviceType)
            {
                return scene.sceneContext.GetService(serviceType);
            }

            #endregion
        }

        public sealed class PhysicsVolumeContext : IPhysicsVolumeContext
        {
            Scene scene;

            public PhysicsVolumeContext(Scene scene)
            {
                this.scene = scene;
            }

            #region IPhysicsVolumeContext

            public ActorCollection<CharacterActor> Characters
            {
                get { return scene.characters; }
            }

            #endregion
        }

        public sealed class MediatorContext : IMediatorContext
        {
            public MediatorContext(Scene scene)
            {
                Scene = scene;
            }

            #region IMediatorContext

            public Scene Scene { get; private set; }

            #endregion
        }

        public sealed class ControllerContext : IControllerContext
        {
            public ControllerContext(Scene scene)
            {
                Scene = scene;
                InputDevice = scene.sceneContext.GetRequiredService<IInputDeviceService>().InputDevice;
            }

            #region IControllerContext

            public Scene Scene { get; private set; }

            public IInputDevice InputDevice { get; private set; }

            #endregion

            #region IServiceProvider

            public object GetService(Type serviceType)
            {
                return Scene.sceneContext.GetService(serviceType);
            }

            #endregion
        }

        public sealed class ActorCollisionShapeFactoryContext : IActorCollisionShapeFactoryContext
        {
            Scene scene;

            public ActorCollisionShapeFactoryContext(Scene scene)
            {
                this.scene = scene;
            }

            #region IServiceProvider

            public object GetService(Type serviceType)
            {
                return scene.sceneContext.GetService(serviceType);
            }

            #endregion
        }

        #endregion

        #region Fields and Properties
        
        string sceneName;

        /// <summary>
        /// シーン名。
        /// </summary>
        public string SceneName
        {
            get { return sceneName; }
        }

        SceneConfig sceneConfig;

        bool contentLoaded;

        /// <summary>
        /// LoadContent が呼び出されたかどうかを示す値。
        /// </summary>
        /// <value>
        /// true (LoadContent が呼び出された場合)、false (それ以外の場合)。
        /// </value>
        public bool ContentLoaded
        {
            get { return contentLoaded; }
        }

        ActorFactoryContext actorFactoryContext;
        ActorCollisionShapeFactoryContext actorCollisionShapeFactoryContext;
        ActorContext actorContext;
        PhysicsVolumeContext physicsVolumeContext;
        MediatorContext mediatorContext;
        ControllerContext controllerContext;

        ISceneContext sceneContext;
        public ISceneContext SceneContext
        {
            get { return sceneContext; }
        }

        // ActorFactory の Dictionary です。
        // 型名をキーとしてインスタンス化された ActorFactory を値としています。
        Dictionary<string, ActorFactory> actorFactories = new Dictionary<string, ActorFactory>();

        Dictionary<string, ActorCollisionShapeFactory> actorCollisionShapeFactories = new Dictionary<string, ActorCollisionShapeFactory>();

        public SceneSettings SceneSettings
        {
            get { return sceneConfig.SceneSettings; }
        }

        IAudioManager sceneAudioManager;
        public IAudioManager SceneAudioManager
        {
            get { return sceneAudioManager; }
        }

        IAudioManager characterAudioManager;
        public IAudioManager CharacterAudioManager
        {
            get { return characterAudioManager; }
        }

        AmbientSoundManager ambientSoundManager;
        public AmbientSoundManager AmbientSoundManager
        {
            get { return ambientSoundManager; }
        }

        Player player = new Player();
        public Player Player
        {
            get { return player; }
        }

        CameraActor activeCamera;
        public CameraActor ActiveCamera
        {
            get { return activeCamera; }
        }

        ActorCollection<CameraActor> cameras = new ActorCollection<CameraActor>();
        public ActorCollection<CameraActor> Cameras
        {
            get { return cameras; }
        }

        ActorCollection<CharacterActor> characters = new ActorCollection<CharacterActor>();
        public ActorCollection<CharacterActor> Characters
        {
            get { return characters; }
        }

        ActorCollection<StaticMeshActor> staticMeshes = new ActorCollection<StaticMeshActor>();
        public ActorCollection<StaticMeshActor> StaticMeshes
        {
            get { return staticMeshes; }
        }

        ActorCollection<TerrainActor> terrains = new ActorCollection<TerrainActor>();
        public ActorCollection<TerrainActor> Terrains
        {
            get { return terrains; }
        }

        ActorCollection<FluidSurfaceActor> fluidSurfaces = new ActorCollection<FluidSurfaceActor>();
        public ActorCollection<FluidSurfaceActor> FluidSurfaces
        {
            get { return fluidSurfaces; }
        }

        ActorCollection<SkyDomeActor> skyDomes = new ActorCollection<SkyDomeActor>();
        public ActorCollection<SkyDomeActor> SkyDomes
        {
            get { return skyDomes; }
        }

        ActorCollection<VolumeFogActor> volumeFogs = new ActorCollection<VolumeFogActor>();
        public ActorCollection<VolumeFogActor> VolumeFogs
        {
            get { return volumeFogs; }
        }

        ActorCollection<PhysicsVolumeActor> physicsVolumes = new ActorCollection<PhysicsVolumeActor>();
        public ActorCollection<PhysicsVolumeActor> PhysicsVolumes
        {
            get { return physicsVolumes; }
        }

        ActorCollection<PostProcessVolumeActor> postProcessVolumes = new ActorCollection<PostProcessVolumeActor>();
        public ActorCollection<PostProcessVolumeActor> PostProcessVolumes
        {
            get { return postProcessVolumes; }
        }

        ControllerCollection controllers = new ControllerCollection();
        public ControllerCollection Controllers
        {
            get { return controllers; }
        }

        MediatorCollection startupMediators = new MediatorCollection();
        public MediatorCollection StartupMediators
        {
            get { return startupMediators; }
        }

        #endregion

        #region Constructors

        public Scene(ISceneContext sceneContext, string sceneName)
        {
            if (sceneContext == null) throw new ArgumentNullException("sceneContext");
            if (string.IsNullOrEmpty(sceneName)) throw new ArgumentNullException("sceneName");

            this.sceneContext = sceneContext;
            this.sceneName = sceneName;

            actorFactoryContext = new ActorFactoryContext(this);
            actorCollisionShapeFactoryContext = new ActorCollisionShapeFactoryContext(this);
            actorContext = new ActorContext(this);
            physicsVolumeContext = new PhysicsVolumeContext(this);
            mediatorContext = new MediatorContext(this);
            controllerContext = new ControllerContext(this);

            // 必要に応じて自動的に Actor のコンテンツをロードするためにイベント ハンドラを関連付けます。
            cameras.ItemAdded += new EventHandler<ActorCollectionEventArgs>(OnActorAdded);
            cameras.ItemRemoved += new EventHandler<ActorCollectionEventArgs>(OnActorRemoved);
            characters.ItemAdded += new EventHandler<ActorCollectionEventArgs>(OnActorAdded);
            characters.ItemRemoved += new EventHandler<ActorCollectionEventArgs>(OnActorRemoved);
            staticMeshes.ItemAdded += new EventHandler<ActorCollectionEventArgs>(OnActorAdded);
            staticMeshes.ItemRemoved += new EventHandler<ActorCollectionEventArgs>(OnActorRemoved);
            terrains.ItemAdded += new EventHandler<ActorCollectionEventArgs>(OnActorAdded);
            terrains.ItemRemoved += new EventHandler<ActorCollectionEventArgs>(OnActorRemoved);
            fluidSurfaces.ItemAdded += new EventHandler<ActorCollectionEventArgs>(OnActorAdded);
            fluidSurfaces.ItemRemoved += new EventHandler<ActorCollectionEventArgs>(OnActorRemoved);
            skyDomes.ItemAdded += new EventHandler<ActorCollectionEventArgs>(OnActorAdded);
            skyDomes.ItemRemoved += new EventHandler<ActorCollectionEventArgs>(OnActorRemoved);
            volumeFogs.ItemAdded += new EventHandler<ActorCollectionEventArgs>(OnActorAdded);
            volumeFogs.ItemRemoved += new EventHandler<ActorCollectionEventArgs>(OnActorRemoved);
            physicsVolumes.ItemAdded += new EventHandler<ActorCollectionEventArgs>(OnActorAdded);
            physicsVolumes.ItemRemoved += new EventHandler<ActorCollectionEventArgs>(OnActorRemoved);
            postProcessVolumes.ItemAdded += new EventHandler<ActorCollectionEventArgs>(OnActorAdded);
            postProcessVolumes.ItemRemoved += new EventHandler<ActorCollectionEventArgs>(OnActorRemoved);

            // アクティブ カメラが適切に再設定されるようにイベント ハンドラを関連付けます。
            cameras.ItemAdded += new EventHandler<ActorCollectionEventArgs>(OnCameraAdded);
            cameras.ItemRemoved += new EventHandler<ActorCollectionEventArgs>(OnCameraRemoved);

            startupMediators.ItemAdded += new EventHandler<MediatorCollectionEventArgs>(OnStartupMediatorAdded);
            startupMediators.ItemRemoved += new EventHandler<MediatorCollectionEventArgs>(OnStartupMediatorRemoved);

            controllers.ItemAdded += new EventHandler<ControllerCollectionEventArgs>(OnControllerAdded);
            controllers.ItemRemoved += new EventHandler<ControllerCollectionEventArgs>(OnControllerRemoved);
        }

        /// <summary>
        /// Actor が追加された場合に呼び出され、
        /// コンテナが既にコンテンツ ロード済みの状態ならば、追加された Actor のコンテンツをロードします。
        /// </summary>
        /// <param name="sender">イベント元。</param>
        /// <param name="e">イベント データ</param>
        void OnActorAdded(object sender, ActorCollectionEventArgs e)
        {
            var actor = e.Item;
            if (contentLoaded && !actor.ContentLoaded)
            {
                LoadContent(actor);
            }
        }

        /// <summary>
        /// Actor が削除された場合に呼び出され、
        /// Actor のコンテンツがロード済みならばそれをアンロードします。
        /// </summary>
        /// <param name="sender">イベント元。</param>
        /// <param name="e">イベント データ</param>
        void OnActorRemoved(object sender, ActorCollectionEventArgs e)
        {
            var actor = e.Item;
            if (actor.ContentLoaded)
            {
                actor.UnloadContent();
            }
        }

        /// <summary>
        /// CameraActor が追加された場合に呼び出され、
        /// 追加された CameraActor が Active ならば、それをシーンのアクティブ カメラとして設定します。
        /// この時、他の CameraActor がアクティブ カメラに設定されていたならば、その Active を false に設定します。
        /// </summary>
        /// <param name="sender">イベント元。</param>
        /// <param name="e">イベント データ</param>
        void OnCameraAdded(object sender, ActorCollectionEventArgs e)
        {
            var camera = e.Item as CameraActor;
            camera.ActiveChanged += new EventHandler<EventArgs>(OnCameraActiveChanged);
            
            if (camera.Active)
            {
                if (activeCamera != null)
                {
                    activeCamera.Active = false;
                }
                activeCamera = camera;
            }
        }

        /// <summary>
        /// CameraActor が削除された場合に呼び出され、
        /// 削除された CameraActor の Active を false にします。
        /// この時、CameraActor がアクティブ カメラであったならば、
        /// OnCameraActiveChanged メソッドの呼び出しによりアクティブ カメラに null が設定されます。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnCameraRemoved(object sender, ActorCollectionEventArgs e)
        {
            var camera = e.Item as CameraActor;
            camera.Active = false;
            camera.ActiveChanged -= new EventHandler<EventArgs>(OnCameraActiveChanged);
        }

        /// <summary>
        /// CameraActor の Active プロパティに変更が合った場合に呼び出され、
        /// Active が true に設定されたならば、その CameraActor をアクティブ カメラに設定します。
        /// この時、前回のアクティブ カメラであった CameraActor の Active は false に設定されます。
        /// 一方、Active が false に設定されたならば、アクティブ カメラに null が設定されます。 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnCameraActiveChanged(object sender, EventArgs e)
        {
            var camera = sender as CameraActor;
            if (camera.Active)
            {
                if (activeCamera != null)
                {
                    activeCamera.Active = false;
                }
                activeCamera = camera;
            }
            else
            {
                if (activeCamera == camera)
                {
                    activeCamera = null;
                }
            }
        }

        void OnStartupMediatorAdded(object sender, MediatorCollectionEventArgs e)
        {
            e.Item.MediatorContext = mediatorContext;
        }

        void OnStartupMediatorRemoved(object sender, MediatorCollectionEventArgs e)
        {
            e.Item.MediatorContext = null;
        }

        void OnControllerAdded(object sender, ControllerCollectionEventArgs e)
        {
            e.Item.ControllerContext = controllerContext;
        }

        void OnControllerRemoved(object sender, ControllerCollectionEventArgs e)
        {
            e.Item.ControllerContext = null;
        }

        #endregion

        #region LoadContent

        /// <summary>
        /// コンテンツをロードします。
        /// </summary>
        public void LoadContent()
        {
            if (contentLoaded) return;

            sceneConfig = sceneContext.LocalContent.Load<SceneConfig>(sceneName);

            LoadAudios();
            LoadActors();
            LoadKillPlane();

            foreach (var mediator in startupMediators)
            {
                mediator.Execue();
            }

            foreach (var controller in controllers)
            {
                controller.LoadContent();
            }

            contentLoaded = true;
        }

        /// <summary>
        /// AudioManager と AmbientSoundManager をロードします。
        /// </summary>
        void LoadAudios()
        {
            var audioService = sceneContext.GetRequiredService<IAudioService>();

            sceneAudioManager = audioService.CreateAudioManager(
                "Scene", sceneContext.LocalContent.Load<AudioConfig>(sceneConfig.SceneAudioAssetName));

            characterAudioManager = audioService.CreateAudioManager(
                "Character", sceneContext.LocalContent.Load<AudioConfig>(sceneConfig.CharacterAudioAssetName));

            ambientSoundManager = new AmbientSoundManager(sceneAudioManager);

            foreach (var ambientSound in sceneConfig.AmbientSounds)
            {
                ambientSoundManager.AmbientSounds.Add(ambientSound);
            }
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
            LoadContent<CameraActor>(cameras);
            LoadContent<CharacterActor>(characters);
            LoadContent<StaticMeshActor>(staticMeshes);
            LoadContent<TerrainActor>(terrains);
            LoadContent<FluidSurfaceActor>(fluidSurfaces);
            LoadContent<SkyDomeActor>(skyDomes);
            LoadContent<VolumeFogActor>(volumeFogs);
            LoadContent<PhysicsVolumeActor>(physicsVolumes);
            LoadContent<PostProcessVolumeActor>(postProcessVolumes);

            // プレイヤの CharacterActor を設定します。
            player.Actor = characters[sceneConfig.PlayerCharacterActorName];
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
                cameras.Add(camera);
                return;
            }

            var character = actor as CharacterActor;
            if (character != null)
            {
                characters.Add(character);
                return;
            }

            var staticMesh = actor as StaticMeshActor;
            if (staticMesh != null)
            {
                staticMeshes.Add(staticMesh);
                return;
            }

            var terrain = actor as TerrainActor;
            if (terrain != null)
            {
                terrains.Add(terrain);
                return;
            }

            var fluidSurface = actor as FluidSurfaceActor;
            if (fluidSurface != null)
            {
                fluidSurfaces.Add(fluidSurface);
                return;
            }

            var skyDome = actor as SkyDomeActor;
            if (skyDome != null)
            {
                skyDomes.Add(skyDome);
                return;
            }

            var volumeFog = actor as VolumeFogActor;
            if (volumeFog != null)
            {
                volumeFogs.Add(volumeFog);
                return;
            }

            var physicsVolume = actor as PhysicsVolumeActor;
            if (physicsVolume != null)
            {
                physicsVolumes.Add(physicsVolume);
                return;
            }

            var postProcessVolume = actor as PostProcessVolumeActor;
            if (postProcessVolume != null)
            {
                postProcessVolumes.Add(postProcessVolume);
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

            // TODO: もっとスマートに書かなくて大丈夫か？
            if (actor is PhysicsVolumeActor)
            {
                (actor as PhysicsVolumeActor).PhysicsVolumeContext = physicsVolumeContext;
            }

            // Actor のコンテンツをロードします。
            actor.LoadContent();

            // Actor の物理を初期化します。
            InitializeActorPhysics(actor);
        }

        void InitializeActorPhysics(Actor actor)
        {
            if (actor is CharacterActor)
            {
                InitializeCharacterActorPhysics(actor as CharacterActor);
                return;
            }

            if (actor.CollisionBoundsConfig != null && actor.CollisionBoundsConfig.CollisionShapeConfigs.Count > 0)
            {
                var physicsService = GetRequiredPhysicsService();

                actor.CollisionBounds = physicsService.CollisionBoundsFactory.CreateCollisionBounds();
                actor.CollisionBounds.Entity = actor;

                foreach (var shapeConfig in actor.CollisionBoundsConfig.CollisionShapeConfigs)
                {
                    var collisionShape = CreateCollisionShape(actor, shapeConfig);
                    actor.CollisionBounds.AddCollisionShape(collisionShape);
                }

                actor.CollisionBounds.UpdateCollisionShapeTransforms(ref actor.Position, ref actor.Orientation);
                actor.CollisionBounds.Enabled = true;
            }
        }

        void InitializeCharacterActorPhysics(CharacterActor actor)
        {
            if (actor.RigidBodyConfig == null) return;

            var physicsService = GetRequiredPhysicsService();
            var rigidBody = physicsService.RigidBodyFactory.CreateRigidBody();

            // デフォルトの重力を設定します。
            var gravity = physicsService.Gravity;
            rigidBody.UpdateGravity(ref gravity);

            if (actor.CollisionBoundsConfig != null && actor.CollisionBoundsConfig.CollisionShapeConfigs.Count > 0)
            {
                foreach (var shapeConfig in actor.CollisionBoundsConfig.CollisionShapeConfigs)
                {
                    var collisionShape = CreateCollisionShape(actor, shapeConfig);
                    rigidBody.CollisionBounds.AddCollisionShape(collisionShape);
                }
            }

            rigidBody.CalculateMassProperties(actor.RigidBodyConfig.MassDensity);

            rigidBody.AutoDisabled = actor.RigidBodyConfig.AutoDisabled;
            rigidBody.Immovable = actor.RigidBodyConfig.Immovable;
            if (!actor.RigidBodyConfig.InertiaTensorEnabled)
            {
                var inertiaTensor = Matrix.Identity;
                inertiaTensor.M11 = 0;
                inertiaTensor.M22 = 0;
                inertiaTensor.M33 = 0;
                rigidBody.InertiaTensor = inertiaTensor;
            }
            rigidBody.Enabled = actor.RigidBodyConfig.Enabled;

            actor.RigidBody = rigidBody;
            actor.CollisionBounds = actor.RigidBody.CollisionBounds;
            actor.CollisionBounds.Entity = actor;

            // Initial position/orientation
            actor.RigidBody.UpdatePosition(ref actor.Position);
            if (!actor.RigidBodyOrientationIgnored)
            {
                actor.RigidBody.UpdateOrientation(ref actor.Orientation);
            }
        }

        ICollisionShape CreateCollisionShape(Actor actor, CollisionShapeConfig config)
        {
            var factory = GetActorCollisionShapeFactory(config);
            return factory.CreateCollisionShape(actor, config);
        }

        ActorCollisionShapeFactory GetActorCollisionShapeFactory(CollisionShapeConfig config)
        {
            ActorCollisionShapeFactory factory;
            if (!actorCollisionShapeFactories.TryGetValue(config.Factory, out factory))
            {
                // まだ ActorFactory インスタンスが存在しないならば生成して登録します。
                var factoryType = GetTypeAcrossAssemblies(config.Factory);
                var constructor = factoryType.GetConstructor(Type.EmptyTypes);
                factory = constructor.Invoke(null) as ActorCollisionShapeFactory;
                factory.ActorCollisionShapeFactoryContext = actorCollisionShapeFactoryContext;
                actorCollisionShapeFactories.Add(config.Factory, factory);
            }
            return factory;
        }

        ICollisionBounds killPlaneCollisionBounds;

        void LoadKillPlane()
        {
            var physicsService = GetRequiredPhysicsService();
            var physicsFactory = physicsService.CollisionBoundsFactory;

            killPlaneCollisionBounds = physicsService.CollisionBoundsFactory.CreateCollisionBounds();

            var plane = physicsService.CollisionShapeFactory.CreateCollisionShape(
                typeof(IPlaneCollisionShape)) as IPlaneCollisionShape;
            plane.Normal = Vector3.Up;
            //
            // TODO
            //
            plane.D = 100;

            killPlaneCollisionBounds.AddCollisionShape(plane);
            killPlaneCollisionBounds.Enabled = true;
        }

        IPhysicsService GetRequiredPhysicsService()
        {
            return sceneContext.GetRequiredService<IPhysicsService>();
        }

        #endregion

        #region UnloadContent

        public void UnloadContent()
        {
            if (!contentLoaded) return;

            sceneAudioManager.Dispose();
            characterAudioManager.Dispose();

            UnloadContent<CameraActor>(cameras);
            UnloadContent<CharacterActor>(characters);
            UnloadContent<StaticMeshActor>(staticMeshes);
            UnloadContent<TerrainActor>(terrains);
            UnloadContent<FluidSurfaceActor>(fluidSurfaces);
            UnloadContent<SkyDomeActor>(skyDomes);
            UnloadContent<VolumeFogActor>(volumeFogs);
            UnloadContent<PhysicsVolumeActor>(physicsVolumes);
            UnloadContent<PostProcessVolumeActor>(postProcessVolumes);

            foreach (var controller in controllers)
            {
                controller.UnloadContent();
            }

            contentLoaded = false;
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

        #endregion

        #region Update

        public void Update(GameTime gameTime)
        {
            Update<CameraActor>(gameTime, cameras);
            Update<CharacterActor>(gameTime, characters);
            Update<StaticMeshActor>(gameTime, staticMeshes);
            Update<TerrainActor>(gameTime, terrains);
            Update<FluidSurfaceActor>(gameTime, fluidSurfaces);
            Update<SkyDomeActor>(gameTime, skyDomes);
            Update<VolumeFogActor>(gameTime, volumeFogs);
            Update<PhysicsVolumeActor>(gameTime, physicsVolumes);
            Update<PostProcessVolumeActor>(gameTime, postProcessVolumes);

            if (activeCamera != null)
            {
                Vector3 velocity;
                player.Actor.RigidBody.GetVeclocity(out velocity);

                sceneAudioManager.Listener.Position = activeCamera.Position;
                sceneAudioManager.Listener.Forward = activeCamera.Orientation.Forward;
                sceneAudioManager.Listener.Up = activeCamera.Orientation.Up;
                sceneAudioManager.Listener.Velocity = velocity;

                characterAudioManager.Listener.Position = activeCamera.Position;
                characterAudioManager.Listener.Forward = activeCamera.Orientation.Forward;
                characterAudioManager.Listener.Up = activeCamera.Orientation.Up;
                characterAudioManager.Listener.Velocity = velocity;
            }
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

        // TODO: これでいいのか？
        public void HandleInput(GameTime gameTime)
        {
            foreach (var controller in controllers)
            {
                controller.Update(gameTime);
            }
        }

        #endregion
    }
}
