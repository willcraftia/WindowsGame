#region Using

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

#endregion

namespace Willcraftia.Xna.Foundation.Scenes
{
    public class PointLight
    {
        public Color Ambient { get; set; }
        public Color Diffuse { get; set; }
        public Color Specular { get; set; }
        public Color Shadow { get; set; }
        public Vector3 Position { get; set; }
        public bool Enabled { get; set; }

        public PointLight()
        {
            Ambient = Color.Black;
            Diffuse = Color.White;
            Specular = Color.Black;
            Shadow = Color.Black;
            Position = Vector3.Zero;
            Enabled = true;
        }
    }
}
