#region Using

using Microsoft.Xna.Framework;

#endregion

namespace Willcraftia.Win.Xna.Framework
{
    using DrawingColor = System.Drawing.Color;
    using MediaColor = System.Windows.Media.Color;
    using XnaColor = Microsoft.Xna.Framework.Color;

    public static class DrawingColorExtension
    {
        public static XnaColor ToXnaColor(this DrawingColor color)
        {
            XnaColor result;
            ToXnaColor(color, out result);
            return result;
        }

        public static void ToXnaColor(this DrawingColor color, out XnaColor result)
        {
            result = new XnaColor(color.R, color.G, color.B, color.A);
        }

        public static Vector3 ToVector3(this DrawingColor color)
        {
            Vector3 result;
            ToVector3(color, out result);
            return result;
        }

        public static void ToVector3(this DrawingColor color, out Vector3 result)
        {
            var xnaColor = new XnaColor(color.R, color.G, color.B);
            result = xnaColor.ToVector3();
        }

        public static MediaColor ToMediaColor(this DrawingColor color)
        {
            MediaColor result;
            ToMediaColor(color, out result);
            return result;
        }

        public static void ToMediaColor(this DrawingColor color, out MediaColor result)
        {
            result = MediaColor.FromArgb(color.A, color.R, color.G, color.B);
        }
    }
}
