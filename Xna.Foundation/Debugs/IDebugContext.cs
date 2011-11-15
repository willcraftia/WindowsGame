#region Using

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Willcraftia.Xna.Framework.Input;

#endregion

namespace Willcraftia.Xna.Foundation.Debugs
{
    public interface IDebugContext : IServiceProvider
    {
        /// <summary>
        /// デバッグ用に作られたデフォルトの SpriteBatch を取得します。
        /// </summary>
        SpriteBatch SpriteBatch { get; }

        /// <summary>
        /// デバッグ用に作られたデフォルトの SpriteFont を取得します。
        /// </summary>
        SpriteFont Font { get; }

        /// <summary>
        /// デバッグ用に作られたデフォルトの ContentManager を取得します。
        /// </summary>
        ContentManager Content { get; }

        /// <summary>
        /// デバッグ用に作られた背景塗り潰し用テクスチャを取得します。
        /// </summary>
        Texture2D FillTexture { get; }
    }
}
