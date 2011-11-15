﻿#region Using

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
    /// Actor をアセットとしてロードする ActorConfig です。
    /// </summary>
    public sealed class AssetActorConfig : ActorConfig
    {
        /// <summary>
        /// Actor のアセット名です。
        /// </summary>
        [ContentSerializer(AllowNull = false)]
        public string AssetName;
    }
}
