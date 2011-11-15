#region Using

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Willcraftia.Xna.Framework.Physics;

#endregion

namespace Willcraftia.Xna.Foundation.Scenes
{
    /// <summary>
    /// 剛体の設定を管理するクラスです。
    /// </summary>
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public sealed class RigidBodyConfig
    {
        bool enabled = true;

        [DefaultValue(true)]
        public bool Enabled
        {
            get { return enabled; }
            set { enabled = value; }
        }

        bool autoDisabled = true;

        [DefaultValue(true)]
        public bool AutoDisabled
        {
            get { return autoDisabled; }
            set { autoDisabled = value; }
        }

        bool immovable = false;

        [DefaultValue(false)]
        public bool Immovable
        {
            get { return immovable; }
            set { immovable = value; }
        }

        bool inertiaTensorEnabled = true;

        [DefaultValue(true)]
        public bool InertiaTensorEnabled
        {
            get { return inertiaTensorEnabled; }
            set { inertiaTensorEnabled = value; }
        }

        float massDensity = 1.0f;

        [DefaultValue(1.0f)]
        public float MassDensity
        {
            get { return massDensity; }
            set { massDensity = value; }
        }
    }
}
