#region Using

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

#endregion

namespace Willcraftia.Xna.Framework.Input
{
    /// <summary>
    /// 入力デバイスにアクセスするためのインタフェースです。
    /// </summary>
    public interface IInputDevice
    {
        /// <summary>
        /// 入力デバイスが有効かどうかを判定します。
        /// </summary>
        /// <value>true (入力デバイスが有効な場合)、false (それ以外の場合)。</value>
        bool Enabled { get; set; }

        /// <summary>
        /// PlayerKeyboard を取得します。
        /// </summary>
        /// <param name="playerIndex">プレイヤのインデックス。</param>
        /// <returns>PlayerKeyboard。</returns>
        PlayerKeyboard GetKeybord(PlayerIndex playerIndex);

        /// <summary>
        /// PlayerGamePad を取得します。
        /// </summary>
        /// <param name="playerIndex">プレイヤのインデックス。</param>
        /// <returns>PlayerGamePad。</returns>
        PlayerGamePad GetGamePad(PlayerIndex playerIndex);

        /// <summary>
        /// PlayerMouse を取得します。
        /// </summary>
        /// <returns>PlayerMouse。</returns>
        PlayerMouse GetMouse();

        /// <summary>
        /// 指定されたプレイヤが指定のキーを押下しているかどうかを判定します。
        /// </summary>
        /// <param name="key">判定するキー。</param>
        /// <param name="playerIndex">
        /// 判定するプレイヤのインデックス。
        /// null を指定した場合は全てのプレイヤの入力を判定し、
        /// 押下しているプレイヤが存在するならば、そのプレイヤのインデックスを result に設定します。
        /// </param>
        /// <param name="result">キーを押下しているプレイヤのインデックス。</param>
        /// <returns>true (キーを押下している場合)、false (それ以外の場合)。</returns>
        bool IsKeyPressed(Keys key, PlayerIndex? playerIndex, out PlayerIndex result);
        
        /// <summary>
        /// 指定されたプレイヤが指定のボタンを押下しているかどうかを判定します。
        /// </summary>
        /// <param name="button">判定するボタン。</param>
        /// <param name="playerIndex">
        /// 判定するプレイヤのインデックス。
        /// null を指定した場合は全てのプレイヤの入力を判定し、
        /// 押下しているプレイヤが存在するならば、そのプレイヤのインデックスを result に設定します。
        /// </param>
        /// <param name="result">ボタンを押下しているプレイヤのインデックス。</param>
        /// <returns>true (ボタンを押下している場合)、false (それ以外の場合)。</returns>
        bool IsButtonPressed(Buttons button, PlayerIndex? playerIndex, out PlayerIndex result);
    }
}
