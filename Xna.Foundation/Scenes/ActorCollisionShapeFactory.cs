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

namespace Willcraftia.Xna.Foundation.Scenes
{
    public abstract class ActorCollisionShapeFactory
    {
        IActorCollisionShapeFactoryContext actorCollisionShapeFactoryContext;
        public IActorCollisionShapeFactoryContext ActorCollisionShapeFactoryContext
        {
            get { return actorCollisionShapeFactoryContext; }
            set { actorCollisionShapeFactoryContext = value; }
        }

        public abstract Type CollisionShapeType { get; }

        public ICollisionShape CreateCollisionShape(Actor actor, CollisionShapeConfig config)
        {
            var service = actorCollisionShapeFactoryContext.GetRequiredService<IPhysicsService>();
            var instance = service.CollisionShapeFactory.CreateCollisionShape(CollisionShapeType);
            InitializeProperties(instance, actor, config);
            return instance;
        }

        protected virtual void InitializeProperties(ICollisionShape instance, Actor actor, CollisionShapeConfig config)
        {
            instance.StaticFriction = config.StaticFriction;
            instance.DynamicFriction = config.DynamicFriction;
            instance.Restitution = config.Restitution;
        }
    }
}
