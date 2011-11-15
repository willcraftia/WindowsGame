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
    /// Actor そのものを保持する ActorConfig です。
    /// </summary>
    public sealed class ActualActorConfig : ActorConfig
    {
        /// <summary>
        /// この設定で使用する Actor です。
        /// </summary>
        public Actor Actor;
    }
}
