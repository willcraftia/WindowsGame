#region Using

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

#endregion

namespace Willcraftia.Xna.Foundation
{
    /// <summary>
    /// Game 環境にアクセスするためのインタフェースです。
    /// </summary>
    public interface IGameContext : IServiceProvider
    {
        /// <summary>
        /// 現在の GraphicsDevice を取得します。
        /// </summary>
        GraphicsDevice GraphicsDevice { get; }

        /// <summary>
        /// Game の ResetElapsedTime() を呼び出します。
        /// </summary>
        void ResetElapsedTime();
    }
}
