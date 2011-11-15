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

#endregion

namespace Willcraftia.Xna.Foundation.Scenes
{
    /// <summary>
    /// ActoryFactory を管理する環境へのインタフェースです。
    /// </summary>
    public interface IActorFactoryContext
    {
        /// <summary>
        /// ActorFactory で使用できる ContentManager を取得します。
        /// </summary>
        ContentManager Content { get; }
    }
}
