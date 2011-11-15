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
using Willcraftia.Xna.Framework.Physics;

#endregion

namespace Willcraftia.Xna.Foundation.Scenes.Factories
{
    public sealed class SlopeActorCollisionShapeFactory : MeshActorCollisionShapeFactory
    {
        public override Type CollisionShapeType
        {
            get { return typeof(ISlopeCollisionShape); }
        }
    }
}
