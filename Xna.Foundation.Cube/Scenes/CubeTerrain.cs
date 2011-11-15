#region Using

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

#endregion

namespace Willcraftia.Xna.Foundation.Cube.Scenes
{
    public sealed class CubeTerrain
    {
        public Model Model { get; set; }

        public CubeHeightmap Heightmap { get; set; }
    }
}
