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

namespace Willcraftia.Xna.Foundation.Scenes
{
    /// <summary>
    /// ボリュームを表す Actor です。
    /// </summary>
    public class VolumeActor : Actor
    {
        #region Fields and Properties

        // TODO: 他の形状もサポートしたい。
        BoundingBoxVolumeShape shape = new BoundingBoxVolumeShape();
        public BoundingBoxVolumeShape Shape
        {
            get { return shape; }
        }

        #endregion

        public void GetBoundingBox(out BoundingBox result)
        {
            Shape.GetBoundingBox(out result);
        }
    }
}
