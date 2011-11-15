#region Using

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Willcraftia.Xna.Framework.Physics;
using Willcraftia.Xna.Foundation.Cube.Scenes;

#endregion

namespace Willcraftia.Xna.Foundation.Cube.Physics
{
    public interface ICubeHeightmapCollisionShape : ICollisionShape
    {
        CubeHeightmap Heightmap { get; set; }
        Vector3 Position { get; set; }
    }
}
