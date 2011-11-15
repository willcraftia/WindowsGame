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

#endregion

namespace Willcraftia.Xna.Foundation
{
    /// <summary>
    /// 描画処理の開始と終了に対するコールバック インタフェースです。
    /// </summary>
    public interface IDrawCallback
    {
        /// <summary>
        /// 描画処理の開始で呼び出されます。
        /// </summary>
        /// <param name="gameTime">前回の Update が呼び出されてからの経過時間。</param>
        void OnDrawStarted(GameTime gameTime);

        /// <summary>
        /// 描画処理の終了で呼び出されます。
        /// </summary>
        /// <param name="gameTime">前回の Update が呼び出されてからの経過時間。</param>
        void OnDrawCompleted(GameTime gameTime);
    }
}
