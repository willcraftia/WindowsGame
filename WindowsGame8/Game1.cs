#region Using

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;

using Willcraftia.Xna.Framework.Audio;
using Willcraftia.Xna.Framework.Input;
using Willcraftia.Xna.Framework.Physics;

using Willcraftia.Xna.Foundation;
using Willcraftia.Xna.Foundation.Debugs;
using Willcraftia.Xna.Foundation.Graphics;
using Willcraftia.Xna.Foundation.Scenes.Renders;
using Willcraftia.Xna.Foundation.Scenes.Controllers;
using Willcraftia.Xna.Foundation.Screens;

using Willcraftia.Xna.Foundation.Cube.Scenes;
using Willcraftia.Xna.Foundation.Cube.Physics;

using JigLibX.Collision;
using JigLibX.Physics;

using Willcraftia.Xna.Foundation.JigLib.Debugs;
using Willcraftia.Xna.Foundation.JigLib;
using Willcraftia.Xna.Foundation.JigLib.Cube;

using WindowsGame8.Physics;
using WindowsGame8.Screens;

#endregion

namespace WindowsGame8
{
    public sealed class Game1 : Game
    {
        #region Inner classes

        public sealed class FpsController
        {
            Game game;

            int fps;
            public int Fps
            {
                get { return fps; }
                set
                {
                    if (fps != value)
                    {
                        fps = value;
                        game.TargetElapsedTime = TimeSpan.FromSeconds(1.0f / (float) fps);
                    }
                }
            }

            public FpsController(Game game, int fps)
            {
                this.game = game;
                this.fps = fps;
                game.TargetElapsedTime = TimeSpan.FromSeconds(1.0f / (float) fps);
            }
        }

        #endregion

        #region Fields and Properties

        GraphicsDeviceManager graphics;
        ScreenContainer screenContainer;

        InputDeviceServiceComponent inputDeviceService;
        AudioService audioService;

        JigLibXAsyncPhysicsService physicsService;

#if DEBUG
        DebugContainer debugContainer;
        DebugConsoleComponent debugConsole;
        FpsCounterComponent fpsCounter;
        TimeRulerComponent timeRuler;
        FpsController fpsController;
        DebugMapComponent debugMap;
        PropertyDebug fpsControllerPropertyDebug;
        PropertyDebug fpsCounterPropertyDebug;
        PropertyDebug timeRulerPropertyDebug;
        PropertyDebug debugMapPropertyDebug;
#endif

        #endregion

        #region Constructors

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);

            #region Screen size
            //
            // REFERENCE: http://creators.xna.com/ja-jp/education/bestpractices
            //            http://social.msdn.microsoft.com/Forums/ja-JP/xnagameja/thread/60a6d9d9-1ede-4657-a77c-9ff65edd563a
            //
            // Xbox360:
            // 640x480
            // 720x480
            // 1280x720 (720p)
            // 1980x1080
            // 
            graphics.PreferredBackBufferWidth = 720;
            graphics.PreferredBackBufferHeight = 480;
            //graphics.PreferredBackBufferWidth = 1280;
            //graphics.PreferredBackBufferHeight = 720;
            // for recording settings
            //graphics.PreferredBackBufferWidth = 640;
            //graphics.PreferredBackBufferHeight = 480;
            //graphics.PreferredBackBufferWidth = 320;
            //graphics.PreferredBackBufferHeight = 240;
            #endregion

            graphics.PreferMultiSampling = true;

            #region Configure the calture (to get default exception messages)
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-US");
            #endregion

            #region Configure content root
            Content.RootDirectory = "Content";
            #endregion

            //IsMouseVisible = false;
            //TargetElapsedTime = TimeSpan.FromSeconds(1.0f / 30.0f);
            fpsController = new FpsController(this, 60);
        }

        #endregion

        #region Initialize

        protected override void Initialize()
        {
            #region IInputDeviceService

            inputDeviceService = new InputDeviceServiceComponent(this);
            inputDeviceService.UpdateOrder = 0;
            Components.Add(inputDeviceService);
            Services.AddService<IInputDeviceService>(inputDeviceService);

            #endregion

            #region IAudioService

            audioService = new AudioService(this, true);
            audioService.UpdateOrder = 10;
            Components.Add(audioService);
            Services.AddService<IAudioService>(audioService);

            #endregion

            #region IScreenService

            screenContainer = new ScreenContainer(this);
            screenContainer.BackgroundColor = Color.DimGray;
            screenContainer.UpdateOrder = 20;
            screenContainer.DrawOrder = 10;
            Components.Add(screenContainer);

            #endregion

            #region IPhysicsService

            // JigLibX を初期化します。
            var physicsSystem = new PhysicsSystem();
            
            // Freezing を有効にします。
            physicsSystem.EnableFreezing = true;

            // CollisionSystem を初期化します。
            physicsSystem.CollisionSystem = new CollisionSystemSAP();

            // DetectFunctor を登録します。
            physicsSystem.CollisionSystem.RegisterCollDetectFunctor(new CollDetectBoxCubicHeightmap());
            physicsSystem.CollisionSystem.RegisterCollDetectFunctor(new CollDetectCapsuleCubicHeightmap());

            // 非同期 IPhysicsService を初期化します。
            physicsService = new JigLibXAsyncPhysicsService(this, physicsSystem);

            // ICollisionShape を登録します。
            physicsService.AddCollisionShape<IBoxCollisionShape, BoxCollisionShape>();
            physicsService.AddCollisionShape<IMeshCollisionShape, MeshCollisionShape>();
            physicsService.AddCollisionShape<ISlopeCollisionShape, SlopeCollisionShape>();
            physicsService.AddCollisionShape<IPlaneCollisionShape, PlaneCollisionShape>();
            physicsService.AddCollisionShape<ICubeHeightmapCollisionShape, CubeHeightmapCollisionShape>();

            // テスト用の ICollisionTester を登録します。
            //physicsService.CollisionTester = new SampleCollisionTester();

            // 非同期 IPhysicsService をサービス登録します。
            Services.AddService<IPhysicsService>(physicsService);

            #endregion

            #region Debugs

#if DEBUG
            InitializeDebugContent();
#endif

            #endregion

            base.Initialize();

            audioService.SetVolume("Music", 0.1f);
        }

        void InitializeDebugContent()
        {
            //
            // Debug
            //

            // DebugContainer は DebugComponent の追加を監視するために一番最初に登録する必要があります。
            // また、入力デバイスのキャプチャーを行うので一番最初に Update が呼び出される必要があります。
            debugContainer = new DebugContainer(this);
            debugContainer.UpdateOrder = 0;
            Components.Add(debugContainer);

            // DebugConsole を登録します。
            // DebugConsole が描画されている間はゲームの更新を止めたいので、
            // その制御のための EventHandler を登録します。
            debugConsole = new DebugConsoleComponent(this);
            debugConsole.UpdateOrder = 10000;
            debugConsole.DrawOrder = 10000;
            debugConsole.Activated += new EventHandler(OnDebugConsoleActivated);
            debugConsole.Deactivated += new EventHandler(OnDebugConsoleDeactivated);
            Components.Add(debugConsole);

            // FpsCounter を登録します。
            fpsCounter = new FpsCounterComponent(this);
            fpsCounter.UpdateOrder = 1;
            fpsCounter.Visible = true;
            fpsCounter.DrawOrder = 1000;
            Components.Add(fpsCounter);

            // TimeRuler を登録します。
            timeRuler = new TimeRulerComponent(this);
            timeRuler.UpdateOrder = 0;
            timeRuler.Visible = false;
            timeRuler.DrawOrder = 1000;
            Components.Add(timeRuler);

            // DebugMap を登録します。
            debugMap = new DebugMapComponent(this);
            debugMap.UpdateOrder = 0;
            debugMap.Visible = false;
            debugMap.DrawOrder = 1000;
            Components.Add(debugMap);

            physicsService.AsyncIntegrationListeners.Add(new TimeRulerAsyncIntegrationListener("Integration", Color.Cyan, 0));

            RegisterDebugCommands();
        }

        void RegisterDebugCommands()
        {
            #region Command [exit]
            debugConsole.RegisterDebugCommand("exit", "Exit",
                delegate(IDebugCommandHost host, string command, IList<string> arguments)
                {
                    Exit();
                });
            #endregion

            #region Command[fps]
            fpsControllerPropertyDebug = new PropertyDebug(fpsController, new PropertyDebug.Command[]
            {
                new PropertyDebug.Command("value", "Fps")
            });
            fpsCounterPropertyDebug = new PropertyDebug(fpsCounter, new PropertyDebug.Command[]
            {
                new PropertyDebug.Command("Visible")
            });
            debugConsole.RegisterDebugCommand("fps", "Configure game FPS or FPS Counter",
                delegate(IDebugCommandHost host, string command, IList<string> arguments)
                {
                    if (0 == arguments.Count)
                    {
                        fpsControllerPropertyDebug.EchoAllProperties(host);
                        fpsCounterPropertyDebug.EchoAllProperties(host);
                    }
                    else
                    {
                        if (!fpsControllerPropertyDebug.HandlePropertyCommand(host, arguments[0], arguments.Count == 1 ? null : arguments[1]))
                        {
                            if (!fpsCounterPropertyDebug.HandlePropertyCommand(host, arguments[0], arguments.Count == 1 ? null : arguments[1]))
                            {
                                host.EchoError(string.Format("Unknown sub command '{0}'", arguments[0]));
                            }
                        }
                    }
                });
            #endregion

            #region Command[tr]
            timeRulerPropertyDebug = new PropertyDebug(timeRuler, new PropertyDebug.Command[]
            {
                new PropertyDebug.Command("Visible"),
                new PropertyDebug.Command("Log", "LogVisible"),
                new PropertyDebug.Command("Bar", "BarVisible")
            });
            debugConsole.RegisterDebugCommand("tr", "Show/Hide time ruler",
                delegate(IDebugCommandHost host, string command, IList<string> arguments)
                {
                    if (0 == arguments.Count)
                    {
                        timeRulerPropertyDebug.EchoAllProperties(host);
                    }
                    else
                    {
                        if (!timeRulerPropertyDebug.HandlePropertyCommand(host, arguments[0], arguments.Count == 1 ? null : arguments[1]))
                        {
                            host.EchoError(string.Format("Unknown sub command '{0}'", arguments[0]));
                        }
                    }
                });
            #endregion

            #region Command [im]
            debugMapPropertyDebug = new PropertyDebug(
                debugMap, new PropertyDebug.Command[]
                {
                    new PropertyDebug.Command("Visible"),
                    new PropertyDebug.Command("Size", "MapSize")
                });
            debugConsole.RegisterDebugCommand("im", "Configure intermediate maps",
                delegate(IDebugCommandHost host, string command, IList<string> arguments)
                {
                    if (0 == arguments.Count)
                    {
                        debugMapPropertyDebug.EchoAllProperties(host);
                    }
                    else
                    {
                        if (!debugMapPropertyDebug.HandlePropertyCommand(host, arguments[0], arguments.Count == 1 ? null : arguments[1]))
                        {
                            host.EchoError(string.Format("Unknown sub command '{0}'", arguments[0]));
                        }
                    }
                });
            #endregion

            #region Command[physics]
            debugConsole.RegisterDebugCommand("physics", "Turn on/off physics",
                delegate(IDebugCommandHost host, string command, IList<string> arguments)
                {
                    if (0 == arguments.Count)
                    {
                        physicsService.Enabled = !physicsService.Enabled;
                    }
                    else
                    {
                        host.Echo("Unknown sub command" + arguments[0]);
                    }
                });
            #endregion

            #region Command[avol]
            debugConsole.RegisterDebugCommand("avol", "Configure audio volume",
                delegate(IDebugCommandHost host, string command, IList<string> arguments)
                {
                    if (2 <= arguments.Count)
                    {
                        var categoryName = arguments[0];
                        var volume = float.Parse(arguments[1]);
                        audioService.SetVolume(categoryName, volume);
                    }
                    else
                    {
                        host.Echo("avol <category-name> <volume>");
                    }
                });


            #endregion

            debugConsole.RegisterDebugCommand("twitter", "Twitter test",
                delegate(IDebugCommandHost host, string command, IList<string> arguments)
                {
                    const string EXP = "http://twitter.com/statuses/user_timeline/{0}.xml";
                    string account = "myvideogamenews";
                    string uri = string.Format(EXP, account);
                    System.Net.WebRequest request = System.Net.HttpWebRequest.Create(uri);
                    System.Net.WebResponse response = request.GetResponse();
                    System.IO.StreamReader reader = new System.IO.StreamReader(response.GetResponseStream());
                    string result = reader.ReadToEnd();
                    reader.Close();
                    response.Close();
                    System.Xml.XmlDocument document = new System.Xml.XmlDocument();
                    document.LoadXml(result);
                    System.Xml.XmlNodeList nodes = document.SelectNodes("//statuses/status/text");
                    foreach (System.Xml.XmlNode node in nodes)
                    {
                        host.Echo(node.InnerText);
                    }
                });

        }

        /// <summary>
        /// DebugConsole の表示に応じてゲームの更新処理を止めます。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnDebugConsoleActivated(object sender, EventArgs e)
        {
            physicsService.Enabled = false;
            screenContainer.Enabled = false;
        }

        /// <summary>
        /// DebugConsole の非表示に応じてゲームの更新処理を再開します。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnDebugConsoleDeactivated(object sender, EventArgs e)
        {
            physicsService.Enabled = true;
            screenContainer.Enabled = true;
        }

        #endregion

        #region LoadContent

        protected override void LoadContent()
        {
            screenContainer.AddScreen(new BackgroundScreen());
            screenContainer.AddScreen(new MainMenuScreen());

            base.LoadContent();
        }

        #endregion

        #region Update

        protected override void Update(GameTime gameTime)
        {
            physicsService.EndUpdate(gameTime);

            base.Update(gameTime);

            physicsService.BeginUpdate(gameTime);
        }

        #endregion

        /// <summary>
        /// ゲーム ウィンドウが他のウィンドウの背面に映る時に呼び出され、
        /// ゲームの更新処理を止めます。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        protected override void OnActivated(object sender, EventArgs args)
        {
            inputDeviceService.Enabled = true;
            physicsService.Enabled = true;

            base.OnActivated(sender, args);
        }

        /// <summary>
        /// 他のウィンドウの背面にあったゲーム ウィンドウが最前面に映る時に呼び出され、
        /// ゲームの更新処理を再開します。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        protected override void OnDeactivated(object sender, EventArgs args)
        {
            inputDeviceService.Enabled = false;
            physicsService.Enabled = false;

            base.OnDeactivated(sender, args);
        }
    }
}
