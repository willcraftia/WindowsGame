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

#endregion

namespace Willcraftia.Xna.Foundation.Scenes
{
    /// <summary>
    /// Actor の設定を保持するクラスです。
    /// </summary>
    public class ActorConfig
    {
        /// <summary>
        /// ActorConfig から Actor を生成する際に使用する ActoryFactory の型名です。
        /// </summary>
        [ContentSerializer(AllowNull = false)]
        public string Factory;

        /// <summary>
        /// Actor の名前です。
        /// </summary>
        [ContentSerializer(AllowNull = false)]
        public string Name;

        /// <summary>
        /// Actor の初期位置ベクトルです。
        /// </summary>
        [DefaultValue(typeof(Vector3), "0, 0, 0")]
        public Vector3 Position = Vector3.Zero;

        /// <summary>
        /// Actor の初期回転ベクトルです。
        /// </summary>
        [DefaultValue(typeof(Vector3), "0, 0, 0")]
        public Vector3 Orientation = Vector3.Zero;

        /// <summary>
        /// Actor のスケールです。
        /// </summary>
        [DefaultValue(typeof(Vector3), "1, 1, 1")]
        public Vector3 Scale = Vector3.One;

        /// <summary>
        /// Actor が更新されるかどうかを示します。
        /// true (更新される場合)、false (それ以外の場合)。
        /// </summary>
        [DefaultValue(true)]
        public bool Enabled = true;

        /// <summary>
        /// Actor が表示されるかどうかを示します。
        /// true (表示される場合)、false (それ以外の場合)。
        /// </summary>
        [DefaultValue(true)]
        public bool Visible = true;
    }
}
