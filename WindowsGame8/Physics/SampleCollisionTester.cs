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

#endregion

namespace WindowsGame8.Physics
{
    public sealed class SampleCollisionTester : ICollisionTester
    {
        #region ICollisionTester

        public bool OnCollided(ICollisionBounds bounds0, ICollisionBounds bounds1)
        {
            var actor0 = bounds0.Entity as Actor;
            var actor1 = bounds1.Entity as Actor;

            if (actor0 is TerrainActor || actor1 is TerrainActor)
            {
                return true;
            }

            return false;
        }

        #endregion
    }
}
