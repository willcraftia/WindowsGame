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

namespace Willcraftia.Xna.Foundation.Graphics
{
    [Flags]
    public enum LayoutAlignment
    {
        None = 0,

        Left = 1,
        Right = 2,
        HorizontalCenter = 4,

        Top = 8,
        Bottom = 16,
        VerticalCenter = 32,

        TopLeft = Top | Left,
        TopRight = Top | Right,
        TopCenter = Top | HorizontalCenter,

        BottomLeft = Bottom | Left,
        BottomRight = Bottom | Right,
        BottomCenter = Bottom | HorizontalCenter,

        CenterLeft = VerticalCenter | Left,
        CenterRight = VerticalCenter | Right,
        Center = VerticalCenter | HorizontalCenter
    }
}
