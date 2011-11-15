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
    /// 衝突形状の設定を管理するクラスです。
    /// </summary>
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class CollisionShapeConfig
    {
        string factory;

        [Description("The class name of factory, creates CollisionShape.")]
        [ContentSerializer(AllowNull = false)]
        public string Factory
        {
            get { return factory; }
            set { factory = value; }
        }

        float staticFriction = 0.5f;

        [Description("The static friction of CollisionShape.")]
        [DefaultValue(0.5f)]
        public float StaticFriction
        {
            get { return staticFriction; }
            set { staticFriction = value; }
        }

        float dynamicFriction = 0.5f;

        [Description("The dynamic friction of CollisionShape.")]
        [DefaultValue(0.5f)]
        public float DynamicFriction
        {
            get { return dynamicFriction; }
            set { dynamicFriction = value; }
        }

        float restitution = 0.5f;

        [Description("The restitution of CollisionShape.")]
        [DefaultValue(0.5f)]
        public float Restitution
        {
            get { return restitution; }
            set { restitution = value; }
        }
    }
}
