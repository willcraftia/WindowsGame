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

namespace Willcraftia.Xna.Foundation
{
    public static class MathExtension
    {
        public static T Clamp<T>(T value, T min, T max) where T : IComparable
        {
            value = 0 < value.CompareTo(max) ? max : value;
            value = value.CompareTo(min) < 0 ? min : value;
            return value;
        }
    }
}
