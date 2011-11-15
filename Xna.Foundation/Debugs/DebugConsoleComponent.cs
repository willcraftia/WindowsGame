#region Using

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using Willcraftia.Xna.Foundation.Graphics;
using Willcraftia.Xna.Framework.Input;

#endregion

namespace Willcraftia.Xna.Foundation.Debugs
{
    /// <summary>
    /// デバッグ用コンソールを表示および制御する DebugComponent です。
    /// </summary>
    /// <remarks>
    /// ゲーム内で動作するデバックコマンドウィンドウ UI 部分
    /// キーボード入力によってコマンドを入力、実行することができる。
    /// Xbox 360 でも USB キーボードを接続することで動作可能。
    /// 
    /// 使用方法:
    /// 1) このコンポーネントをゲームに追加。
    /// 2) RegisterCommand メソッドを使ってコマンドを登録する
    /// 3) Tab キーでデバッグウィンドウの開閉しコマンド入力
    /// </remarks>
    public sealed class DebugConsoleComponent : DebugComponent, IDebugConsole, IDebugCommandHost
    {
        #region Fields and Properties

        /// <summary>
        /// 最大行数
        /// </summary>
        const int MaxLineCount = 20;

        /// <summary>
        /// コマンドヒストリ数
        /// </summary>
        const int MaxCommandHistory = 32;

        /// <summary>
        /// カーソル文字。ここではUnicodeのBlock Eleemntsからカーソルっぽいものを使用
        /// http://www.unicode.org/charts/PDF/U2580.pdf
        /// </summary>
        const string Cursor = "\u2582";

        /// <summary>
        /// デフォルトのコマンドプロンプト文字列
        /// </summary>
        const string DefaultPrompt = ">";

        Color windowColor = new Color(0, 0, 0, 128);

        /// <summary>
        /// ウィンドウ背景色を取得または設定します。
        /// </summary>
        public Color WindowColor
        {
            get { return windowColor; }
            set { windowColor = value; }
        }

        Color fontColor = Color.White;

        /// <summary>
        /// 文字色を取得または設定します。
        /// </summary>
        public Color FontColor
        {
            get { return fontColor; }
            set { fontColor = value; }
        }

        string prompt = DefaultPrompt;

        /// <summary>
        /// コマンドプロンプト文字列を取得または設定します。
        /// </summary>
        public string Prompt
        {
            get { return prompt; }
            set { prompt = value; }
        }

        /// <summary>
        /// キー入力待機状態を取得します。
        /// </summary>
        public bool Focused
        {
            get { return state != DebugCommandState.Closed; }
        }

        // 現在のステート
        DebugCommandState state = DebugCommandState.Closed;

        // ステート移行用のタイマー
        float stateTransition;

        // 登録されているEchoリスナー
        List<IDebugEchoListner> debugEchoListenrs = new List<IDebugEchoListner>();

        // 登録されているコマンド実行者
        Stack<IDebugCommandExecutor> debugCommandExecutors = new Stack<IDebugCommandExecutor>();

        // 登録されているコマンド
        Dictionary<string, DebugCommandInfo> debugCommandDictionaty = new Dictionary<string, DebugCommandInfo>();

        // 現在入力中のコマンドライン文字列と、カーソル位置
        string commandLine = String.Empty;
        int cursorIndex;

        // コマンドライン表示文字列
        Queue<string> lines = new Queue<string>();

        // コマンド履歴用バッファ
        List<string> commandHistory = new List<string>();

        // 現在選択されている履歴インデックス
        int commandHistoryIndex;

        //　最後に押されたキー
        Keys lastPressedKey;

        //　キーリピートタイマー
        float keyRepeatTimer;

        // 最初のキー押下時のリピート時間(秒)
        float keyRepeatStartDuration = 0.3f;

        // ２回目以降のキーリピート時間(秒)
        float keyRepeatDuration = 0.03f;

        #endregion

        #region Events

        public event EventHandler Activated;
        public event EventHandler Deactivated;

        #endregion

        #region Constructors

        /// <summary>
        /// インスタンスを生成します。
        /// </summary>
        /// <param name="game">インスタンスを登録する Game。</param>
        public DebugConsoleComponent(Game game)
            : base(game)
        {
            game.Components.ComponentRemoved += new EventHandler<GameComponentCollectionEventArgs>(OnComponentRemoved);
            game.Components.ComponentAdded += new EventHandler<GameComponentCollectionEventArgs>(OnComponentAdded);
        }

        /// <summary>
        /// 自分自身が GameComponent として登録される時に、自分自身を現在有効な IDebugConsole として設定します。
        /// </summary>
        /// <param name="sender">イベントのソース。</param>
        /// <param name="e">イベント データ。</param>
        void OnComponentAdded(object sender, GameComponentCollectionEventArgs e)
        {
            if (e.GameComponent == this)
            {
                DebugConsole.Instance = this;
            }
        }

        /// <summary>
        /// 自分自身が GameComponent としての登録から解除される時に、
        /// 自分自身が現在有効な IDebugConsole ではなくなるように設定します。
        /// </summary>
        /// <param name="sender">イベントのソース。</param>
        /// <param name="e">イベント データ。</param>
        void OnComponentRemoved(object sender, GameComponentCollectionEventArgs e)
        {
            if (e.GameComponent == this)
            {
                DebugConsole.Instance = null;
            }
        }

        #endregion

        #region LoadContent

        protected override void LoadContent()
        {
            // ビルドインのコマンドを登録します。

            // [help] コマンドは登録されている全てのコマンドを表示します。
            RegisterDebugCommand("help", "Show Command helps",
                delegate(IDebugCommandHost host, string command, IList<string> arguments)
                {
                    int maxLen = 0;
                    foreach (var cmd in debugCommandDictionaty.Values)
                    {
                        maxLen = Math.Max(maxLen, cmd.Name.Length);
                    }

                    string fmt = String.Format("{{0,-{0}}}    {{1}}", maxLen);

                    foreach (var cmd in debugCommandDictionaty.Values)
                    {
                        Echo(String.Format(fmt, cmd.Name, cmd.Description));
                    }
                });

            // [clear] コマンドはコンソールをクリアします。
            RegisterDebugCommand("cls", "Clear Screen",
                delegate(IDebugCommandHost host, string command, IList<string> arguments)
                {
                    lines.Clear();
                });

            base.LoadContent();
        }

        #endregion

        #region IDebugConsole

        public void RegisterDebugCommand(string name, string description, DebugCommandExecute callback)
        {
            var lowerName = name.ToLower();
            if (debugCommandDictionaty.ContainsKey(lowerName))
            {
                throw new InvalidOperationException(String.Format("Name '{0}' already exists", name));
            }

            debugCommandDictionaty.Add(lowerName, new DebugCommandInfo(name, description, callback));
        }

        public void UnregisterDebugCommand(string name)
        {
            var lowerName = name.ToLower();
            if (!debugCommandDictionaty.ContainsKey(lowerName))
            {
                throw new InvalidOperationException(String.Format("Name '{0}' not found", name));
            }

            debugCommandDictionaty.Remove(name);
        }

        #endregion

        #region IDebugCommandHost

        public void ExecuteDebugCommand(string name)
        {
            // 他のコマンド実行者が登録されている場合は、最新の登録者にコマンドを
            // 実行させる。
            if (debugCommandExecutors.Count != 0)
            {
                debugCommandExecutors.Peek().ExecuteDebugCommand(name);
                return;
            }

            // コマンドの実行
            var spaceChars = new char[] { ' ' };

            Echo(Prompt + name);

            name = name.TrimStart(spaceChars);

            var arguments = new List<string>(name.Split(spaceChars));
            var commandText = arguments[0];
            arguments.RemoveAt(0);

            DebugCommandInfo debugCommandInfo;
            if (debugCommandDictionaty.TryGetValue(commandText.ToLower(), out debugCommandInfo))
            {
                try
                {
                    // 登録されているコマンドのデリゲートを呼び出す
                    debugCommandInfo.Callback(this, name, arguments);
                }
                catch (Exception e)
                {
                    // 例外がコマンド実行中に発生
                    EchoError("Unhandled Exception occured");

                    string[] lines = e.Message.Split(new char[] { '\n' });
                    foreach (string line in lines)
                        EchoError(line);
                }
            }
            else
            {
                Echo("Unknown Command");
            }

            // コマンドヒストリに追加する
            commandHistory.Add(name);
            while (commandHistory.Count > MaxCommandHistory)
            {
                commandHistory.RemoveAt(0);
            }

            commandHistoryIndex = commandHistory.Count;
        }

        public void RegisterDebugEchoListner(IDebugEchoListner listner)
        {
            debugEchoListenrs.Add(listner);
        }

        public void UnregisterDebugEchoListner(IDebugEchoListner listner)
        {
            debugEchoListenrs.Remove(listner);
        }

        public void Echo(DebugMessageLevels messageType, string text)
        {
            lines.Enqueue(text);
            while (lines.Count >= MaxLineCount)
            {
                lines.Dequeue();
            }

            foreach (IDebugEchoListner listner in debugEchoListenrs)
            {
                listner.Echo(messageType, text);
            }
        }

        public void Echo(string text)
        {
            Echo(DebugMessageLevels.Standard, text);
        }

        public void EchoWarning(string text)
        {
            Echo(DebugMessageLevels.Warning, text);
        }

        public void EchoError(string text)
        {
            Echo(DebugMessageLevels.Error, text);
        }

        public void PushDebugCommandExecutor(IDebugCommandExecutor executor)
        {
            debugCommandExecutors.Push(executor);
        }

        public void PopDebugCommandExecutor()
        {
            debugCommandExecutors.Pop();
        }

        #endregion

        #region Update

        public override void Update(GameTime gameTime)
        {
            var dt = gameTime.GetDeltaTime();
            const float OpenSpeed = 8.0f;
            const float CloseSpeed = 8.0f;

            switch (state)
            {
                case DebugCommandState.Closed:
                    if (InputDevice.GetKeybord(PlayerIndex.One).IsKeyDown(Keys.Tab))
                    {
                        state = DebugCommandState.Opening;

                        if (Activated != null)
                        {
                            Activated(this, EventArgs.Empty);
                        }
                    }
                    break;
                case DebugCommandState.Opening:
                    stateTransition += dt * OpenSpeed;
                    if (stateTransition > 1.0f)
                    {
                        stateTransition = 1.0f;
                        state = DebugCommandState.Opened;
                    }
                    break;
                case DebugCommandState.Opened:
                    ProcessKeyInputs(dt);
                    break;
                case DebugCommandState.Closing:
                    stateTransition -= dt * CloseSpeed;
                    if (stateTransition < 0.0f)
                    {
                        stateTransition = 0.0f;
                        state = DebugCommandState.Closed;

                        if (Deactivated != null)
                        {
                            Deactivated(this, EventArgs.Empty);
                        }
                    }
                    break;
            }
        }

        /// <summary>
        /// キー入力処理
        /// </summary>
        /// <param name="deltaTime">前回の Update が呼び出されてからの経過時間。</param>
        void ProcessKeyInputs(float deltaTime)
        {
            var keyboard = InputDevice.GetKeybord(PlayerIndex.One);

            var keys = keyboard.CurrentState.GetPressedKeys();
            var shiftKeyPressed = keyboard.IsKeyDown(Keys.LeftShift) || keyboard.IsKeyDown(Keys.RightShift);

            foreach (Keys key in keys)
            {
                if (!IsKeyPressed(key, deltaTime))
                {
                    continue;
                }

                char ch;
                if (KeyboardUtils.KeyToString(key, shiftKeyPressed, out ch))
                {
                    // 通常文字の入力
                    commandLine = commandLine.Insert(cursorIndex, new string(ch, 1));
                    cursorIndex++;
                }
                else
                {
                    switch (key)
                    {
                        case Keys.Back:
                            if (cursorIndex > 0)
                            {
                                commandLine = commandLine.Remove(--cursorIndex, 1);
                            }
                            break;
                        case Keys.Delete:
                            if (cursorIndex < commandLine.Length)
                            {
                                commandLine = commandLine.Remove(cursorIndex, 1);
                            }
                            break;
                        case Keys.Left:
                            if (cursorIndex > 0)
                            {
                                cursorIndex--;
                            }
                            break;
                        case Keys.Right:
                            if (cursorIndex < commandLine.Length)
                            {
                                cursorIndex++;
                            }
                            break;
                        case Keys.Enter:
                            // コマンドの実行
                            ExecuteDebugCommand(commandLine);
                            commandLine = string.Empty;
                            cursorIndex = 0;
                            break;
                        case Keys.Up:
                            // ヒストリ表示
                            if (commandHistory.Count > 0)
                            {
                                commandHistoryIndex = Math.Max(0, commandHistoryIndex - 1);
                                commandLine = commandHistory[commandHistoryIndex];
                                cursorIndex = commandLine.Length;
                            }
                            break;
                        case Keys.Down:
                            // ヒストリ表示
                            if (commandHistory.Count > 0)
                            {
                                commandHistoryIndex = Math.Min(commandHistory.Count - 1, commandHistoryIndex + 1);
                                commandLine = commandHistory[commandHistoryIndex];
                                cursorIndex = commandLine.Length;
                            }
                            break;
                        case Keys.Tab:
                            state = DebugCommandState.Closing;
                            break;
                    }
                }
            }

        }

        /// <summary>
        /// キー リピートに対応したキー押下を判定します。
        /// </summary>
        /// <param name="key">判定するキー。</param>
        /// <param name="deltaTime">前回の Update が呼び出されてからの経過時間。</param>
        /// <returns></returns>
        bool IsKeyPressed(Keys key, float deltaTime)
        {
            var keyboard = InputDevice.GetKeybord(PlayerIndex.One);

            // 前フレームでキーが押されていなければ、キーが押されていると判定
            if (keyboard.PreviousState.IsKeyUp(key))
            {
                keyRepeatTimer = keyRepeatStartDuration;
                lastPressedKey = key;
                return true;
            }

            // 前フレームでキーが押されていた場合はリピート処理
            if (key == lastPressedKey)
            {
                keyRepeatTimer -= deltaTime;
                if (keyRepeatTimer <= 0.0f)
                {
                    keyRepeatTimer += keyRepeatDuration;
                    return true;
                }
            }

            return false;
        }

        #endregion

        #region Draw

        public override void Draw(GameTime gameTime)
        {
            // コマンドウィンドウが完全に閉じている場合は描画処理をしない
            if (state == DebugCommandState.Closed)
            {
                return;
            }

            // コマンドウィンドウのサイズ計算と描画
            var vp = GraphicsDevice.Viewport;
            var layout = new Layout(new Rectangle(vp.X, vp.Y, vp.Width, vp.Height), vp.TitleSafeArea);
            var clientArea = layout.ClientArea;
            float w = clientArea.Width;
            float h = clientArea.Height;
            float topMargin = h * 0.1f;
            float leftMargin = w * 0.1f;

            var rect = new Rectangle();
            rect.X = (int) leftMargin;
            rect.Y = (int) topMargin;
            rect.Width = (int) (w * 0.8f);
            rect.Height = (int) (MaxLineCount * Font.LineSpacing);

            var transform = Matrix.CreateTranslation(new Vector3(0, -rect.Height * (1.0f - stateTransition), 0));

            SpriteBatch.Begin(
                SpriteSortMode.Deferred,
                null,
                null,
                null,
                null,
                null,
                transform);
            {
                // ウィンドウの背景を塗り潰します。
                SpriteBatch.Draw(FillTexture, rect, windowColor);

                // 文字列の描画
                var position = new Vector2(leftMargin, topMargin);
                foreach (string line in lines)
                {
                    //var builder = new System.Text.StringBuilder();
                    //foreach (var c in line)
                    //{
                    //    if (Font.Characters.Contains(c))
                    //    {
                    //        builder.Append(c);
                    //    }
                    //    else
                    //    {
                    //        builder.Append('*');
                    //    }
                    //}

                    SpriteBatch.DrawString(Font, line, position, fontColor);
                    //SpriteBatch.DrawString(Font, builder.ToString(), position, fontColor);
                    position.Y += Font.LineSpacing;
                }

                // プロンプト文字列の描画
                var leftPart = prompt + commandLine.Substring(0, cursorIndex);
                Vector2 cursorPos = position + Font.MeasureString(leftPart);
                cursorPos.Y = position.Y;

                SpriteBatch.DrawString(Font, string.Format("{0}{1}", prompt, commandLine), position, fontColor);
                SpriteBatch.DrawString(Font, Cursor, cursorPos, fontColor);
            }
            SpriteBatch.End();
        }

        #endregion
    }
}
