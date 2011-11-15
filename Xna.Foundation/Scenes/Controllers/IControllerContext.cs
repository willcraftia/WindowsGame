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

namespace Willcraftia.Xna.Foundation.Scenes
{
    /// <summary>
    /// Controller 実行環境のインタフェースです。
    /// </summary>
    public interface IControllerContext : IServiceProvider
    {
        /// <summary>
        /// Scene。
        /// </summary>
        Scene Scene { get; }

        /// <summary>
        /// IInputDevice。
        /// </summary>
        IInputDevice InputDevice { get; }
    }
}
