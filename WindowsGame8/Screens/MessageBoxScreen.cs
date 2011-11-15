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
using Willcraftia.Xna.Foundation;
using Willcraftia.Xna.Framework.Audio;
using Willcraftia.Xna.Foundation.Screens;

using WindowsGame8.Inputs;

#endregion

namespace WindowsGame8.Screens
{
    /// <summary>
    /// "are you sure? (よろしいですか?)" 確認メッセージの表示に使用する
    /// ポップアップ メッセージ ボックス画面。
    /// </summary>
    class MessageBoxScreen : Screen
    {
        #region Fields

        string message;
        Texture2D gradientTexture;

        #endregion

        #region Events

        public event EventHandler<PlayerIndexEventArgs> Accepted;
        public event EventHandler<PlayerIndexEventArgs> Cancelled;

        #endregion

        #region Constructors

        /// <summary>
        /// コンストラクターは標準の "A = OK、B = キャンセル" の 2 択を
        /// 自動的に組み込みます。
        /// </summary>
        public MessageBoxScreen(string message)
            : this(message, true)
        {
        }

        /// <summary>
        /// コンストラクターは、ポップアップ画面で "A = OK、B = キャンセル" のような
        /// 選択項目を組み込むかどうかを指定できます。
        /// </summary>
        public MessageBoxScreen(string message, bool includeUsageText)
        {
            const string usageText = "\nA button, Space, Enter = ok" +
                                     "\nB button, Esc = cancel";

            if (includeUsageText)
            {
                this.message = message + usageText;
            }
            else
            {
                this.message = message;
            }

            IsPopup = true;

            TransitionOnTime = TimeSpan.FromSeconds(0.2);
            TransitionOffTime = TimeSpan.FromSeconds(0.2);
        }

        #endregion

        #region LoadContent

        public override void LoadContent()
        {
            base.LoadContent();

            var audioConfig = new AudioConfig();
            audioConfig.XactEnabled = true;
            audioConfig.WaveBank = new WaveBankConfig();
            audioConfig.WaveBank.Filename = "Content/Audio/WaveBank.xwb";
            audioConfig.SoundBank = new SoundBankConfig();
            audioConfig.SoundBank.Filename = "Content/Audio/SoundBank.xsb";

            // デフォルトの ContentManager でテクスチャをロードします。
            // したがって、このテクスチャは Screen が削除されてもロードされたままとなり、
            // これ以降の MessageBoxScreen が同じテクスチャをロードしようすると、
            // 既にロードされているテクスチャへの新たな参照が返されます。
            gradientTexture = Content.Load<Texture2D>("Screens/Textures/Gradient");
        }

        #endregion

        #region Handle Input

        /// <summary>
        /// ユーザー入力に応答し、メッセージ ボックスの選択項目を決定またはキャンセルします。
        /// </summary>
        public override void HandleInput(GameTime gameTime)
        {
            PlayerIndex playerIndex;

            // ControllingPlayer 引数に null を渡した場合は、すべてのプレイヤーからの入力を
            // 受け付け、特定のインデックスを渡した場合はそのインデックスに割り当てられている
            // プレイヤーのみから入力を受け付けます。
            // null を渡した場合であっても、InputState ヘルパーは実際に入力を行った
            // プレイヤーを返します。これを Accepted および Cancelled イベントに渡します。
            // これにより、どのプレイヤーが決定およびキャンセルのアクションをトリガーしたかを
            // 判別することが可能になります。
            if (InputDevice.IsMenuSelect(ControllingPlayer, out playerIndex))
            {
                // 決定イベントが発生し、メッセージ ボックスを終了します。
                if (Accepted != null)
                {
                    Accepted(this, new PlayerIndexEventArgs(playerIndex));
                }

                ScreenContext.ExitScreen(this);
            }
            else if (InputDevice.IsMenuCancel(ControllingPlayer, out playerIndex))
            {
                // キャンセル イベントが発生し、メッセージ ボックスを終了します。
                if (Cancelled != null)
                {
                    Cancelled(this, new PlayerIndexEventArgs(playerIndex));
                }

                Audio.SharedAudioManager.CreateSound(SoundConstants.CancelSound).Play();

                ScreenContext.ExitScreen(this);
            }
        }

        #endregion

        #region Draw

        /// <summary>
        /// メッセージ ボックスを描画します。
        /// </summary>
        public override void Draw(GameTime gameTime)
        {
            var font = ScreenContext.Font;

            // ポップアップの下に描画されたその他すべての画面が暗くなります。
            ScreenContext.FadeBackBuffer(TransitionAlpha * 2 / 3, Color.Black);

            // ビューポート内のメッセージ テキストを中央揃えします。
            var viewport = GraphicsDevice.Viewport;
            var viewportSize = new Vector2(viewport.Width, viewport.Height);
            var textSize = font.MeasureString(message);
            var textPosition = (viewportSize - textSize) / 2;

            // 背景にはテキストそのものよりも若干大きな境界が含まれます。
            const int hPad = 32;
            const int vPad = 16;

            var backgroundRectangle = new Rectangle((int) textPosition.X - hPad,
                                                    (int) textPosition.Y - vPad,
                                                    (int) textSize.X + hPad * 2,
                                                    (int) textSize.Y + vPad * 2);

            // 移行時にポップアップ画面のアルファを減衰させます。
            var color = Color.White * TransitionAlpha;

            SpriteBatch.Begin();
            // 背景の矩形を描画します。
            SpriteBatch.Draw(gradientTexture, backgroundRectangle, color);
            // メッセージ ボックス テキストを描画します。
            SpriteBatch.DrawString(font, message, textPosition, color);
            SpriteBatch.End();
        }

        #endregion
    }
}
