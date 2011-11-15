#region Using

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

#endregion

namespace Willcraftia.Win.Xna.Framework
{
    using DrawingColor = System.Drawing.Color;
    using MediaColor = System.Windows.Media.Color;
    using XnaColor = Microsoft.Xna.Framework.Color;

    public static class Vector3Extension
    {
        #region Extensions

        public static DrawingColor ToDrawingColor(this Vector3 vector)
        {
            DrawingColor result;
            ToDrawingColor(vector, out result);
            return result;
        }

        public static void ToDrawingColor(this Vector3 vector, out DrawingColor result)
        {
            var xnaColor = new XnaColor(vector);
            result = DrawingColor.FromArgb(xnaColor.R, xnaColor.G, xnaColor.B);
        }

        public static MediaColor ToMediaColor(this Vector3 vector)
        {
            MediaColor result;
            ToMediaColor(vector, out result);
            return result;
        }

        public static void ToMediaColor(this Vector3 vector, out MediaColor result)
        {
            var xnaColor = new XnaColor(vector);
            result = MediaColor.FromRgb(xnaColor.R, xnaColor.G, xnaColor.B);
        }

        #endregion
    }
}
