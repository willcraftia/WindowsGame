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
using Willcraftia.Xna.Foundation.Scenes;
using Willcraftia.Xna.Foundation.Cube.Physics;

#endregion

namespace Willcraftia.Xna.Foundation.Cube.Scenes.Factories
{
    public sealed class CubeHeightmapActorCollisionShapeFactory : ActorCollisionShapeFactory
    {
        public override Type CollisionShapeType
        {
            get { return typeof(ICubeHeightmapCollisionShape); }
        }

        protected override void InitializeProperties(ICollisionShape instance, Actor actor, CollisionShapeConfig config)
        {
            var concreteShape = instance as ICubeHeightmapCollisionShape;
            concreteShape.Position = actor.Position;

            var concreteActor = actor as TerrainActor;
            var actorModel = concreteActor.ActorModel as CubeTerrainActorModel;
            concreteShape.Heightmap = actorModel.Terrain.Heightmap;

            base.InitializeProperties(instance, actor, config);
        }
    }
}
