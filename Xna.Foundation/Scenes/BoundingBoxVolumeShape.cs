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
    public sealed class BoundingBoxVolumeShape : IVolumeShape
    {
        #region Fields and Properties

        public BoundingBox BoundingBox = BoundingBoxExtension.Empty;

        #endregion

        #region IVolumeShape

        public bool Contains(ref Vector3 position)
        {
            return BoundingBox.Contains(position) == ContainmentType.Contains;
        }

        public void GetBoundingBox(out BoundingBox result)
        {
            result = BoundingBox;
        }

        #endregion
    }
}
