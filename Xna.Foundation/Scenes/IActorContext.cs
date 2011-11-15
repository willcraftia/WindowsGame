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
    /// Actor の環境へのインタフェースです。
    /// </summary>
    public interface IActorContext : IGameContext
    {
        /// <summary>
        /// Actor で使用できる ContentManager。
        /// </summary>
        ContentManager Content { get; }

        /// <summary>
        /// Scene の SceneSettings。
        /// </summary>
        SceneSettings SceneSettings { get; }
        
        /// <summary>
        /// シーン内でアクティブな CameraActor。
        /// </summary>
        CameraActor ActiveCamera { get; }
    }
}
