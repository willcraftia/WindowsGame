#region Using

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

#endregion

namespace Willcraftia.Xna.Framework.Input
{
    /// <summary>
    /// IInputDevice の実装クラスです。
    /// </summary>
    public sealed class InputDevice : IInputDevice
    {
        /// <summary>
        /// 対応している最大プレイヤ数。
        /// </summary>
        public const int MaxPlayerCount = 4;

        PlayerKeyboard[] keyboards = new PlayerKeyboard[MaxPlayerCount];
        PlayerGamePad[] gamePads = new PlayerGamePad[MaxPlayerCount];
        PlayerMouse mouse;

        /// <summary>
        /// インスタンスを生成します。
        /// </summary>
        public InputDevice()
        {
            for (int i = 0; i < MaxPlayerCount; i++)
            {
                keyboards[i] = new PlayerKeyboard(this, (PlayerIndex) i);
                gamePads[i] = new PlayerGamePad(this, (PlayerIndex) i);
            }
            mouse = new PlayerMouse(this);
        }

        /// <summary>
        /// 入力状態を初期化します。
        /// </summary>
        public void Initialize()
        {
            for (int i = 0; i < MaxPlayerCount; i++)
            {
                keyboards[i].Initialize();
                gamePads[i].Initialize();
            }
            mouse.Initialize(0, 0);
        }

        /// <summary>
        /// 入力状態をキャプチャーします。
        /// </summary>
        /// <param name="gameTime">前回の Update が呼び出されてからの経過時間。</param>
        public void Update(GameTime gameTime)
        {
            if (!Enabled) return;

            for (int i = 0; i < MaxPlayerCount; i++)
            {
                keyboards[i].CaptureState();

                if (gamePads[i].Enabled)
                {
                    gamePads[i].CaptureState();
                }
            }

            mouse.CaptureState();
        }

        #region IInputDevice

        public bool Enabled { get; set; }

        public PlayerKeyboard GetKeybord(PlayerIndex playerIndex)
        {
            return keyboards[(int) playerIndex];
        }

        public PlayerGamePad GetGamePad(PlayerIndex playerIndex)
        {
            return gamePads[(int) playerIndex];
        }

        public PlayerMouse GetMouse()
        {
            return mouse;
        }

        public bool IsKeyPressed(Keys key, PlayerIndex? playerIndex, out PlayerIndex result)
        {
            if (playerIndex.HasValue)
            {
                result = playerIndex.Value;
                return keyboards[(int) playerIndex.Value].IsKeyPressed(key);
            }

            return IsKeyPressed(key, PlayerIndex.One, out result) ||
                IsKeyPressed(key, PlayerIndex.Two, out result) ||
                IsKeyPressed(key, PlayerIndex.Three, out result) ||
                IsKeyPressed(key, PlayerIndex.Four, out result);
        }

        public bool IsButtonPressed(Buttons button, PlayerIndex? playerIndex, out PlayerIndex result)
        {
            if (playerIndex.HasValue)
            {
                result = playerIndex.Value;
                return gamePads[(int) playerIndex.Value].IsButtonPressed(button);
            }

            return IsButtonPressed(button, PlayerIndex.One, out result) ||
                IsButtonPressed(button, PlayerIndex.Two, out result) ||
                IsButtonPressed(button, PlayerIndex.Three, out result) ||
                IsButtonPressed(button, PlayerIndex.Four, out result);
        }

        #endregion
    }
}
