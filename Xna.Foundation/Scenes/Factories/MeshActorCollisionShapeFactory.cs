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
    public class MeshActorCollisionShapeFactory : ActorCollisionShapeFactory
    {
        public override Type CollisionShapeType
        {
            get { return typeof(IMeshCollisionShape); }
        }

        protected override void InitializeProperties(ICollisionShape instance, Actor actor, CollisionShapeConfig config)
        {
            var concreteShape = instance as IMeshCollisionShape;

            var modelActorModel = actor.ActorModel as ModelActorModel;
            if (modelActorModel == null)
            {
                throw new InvalidOperationException(
                    "IMeshCollisionShape must be bound to the actor with ModelActorModel.");
            }
            concreteShape.CreateMesh(modelActorModel.Model);

            base.InitializeProperties(instance, actor, config);
        }
    }
}
