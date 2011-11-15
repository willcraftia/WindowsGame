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
    public sealed class BoxActorCollisionShapeFactory : ActorCollisionShapeFactory
    {
        public override Type CollisionShapeType
        {
            get { return typeof(IBoxCollisionShape); }
        }

        protected override void InitializeProperties(ICollisionShape instance, Actor actor, CollisionShapeConfig config)
        {
            var box = instance as IBoxCollisionShape;
            var boundingBox = actor.ActorModel.LocalBoundingBox;

            box.Position = boundingBox.Min;
            box.SideLengths = boundingBox.GetSideLengths();

            base.InitializeProperties(instance, actor, config);
        }
    }
}
