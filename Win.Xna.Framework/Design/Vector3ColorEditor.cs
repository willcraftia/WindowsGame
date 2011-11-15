#region Using

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

#endregion

namespace Willcraftia.Win.Xna.Framework.Design
{
    using DrawingColor = System.Drawing.Color;
    using XnaColor = Microsoft.Xna.Framework.Color;

    public sealed class Vector3ColorEditor : ColorEditor
    {
        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            if (value is Vector3)
            {
                var drawingColor = ((Vector3) value).ToDrawingColor();
                ResolveKnownColor(ref drawingColor, out drawingColor);

                drawingColor = (DrawingColor) base.EditValue(context, provider, drawingColor);
                return drawingColor.ToVector3();
            }
            else
            {
                return base.EditValue(context, provider, value);
            }
        }

        public override void PaintValue(PaintValueEventArgs e)
        {
            if (e.Value is Vector3)
            {
                var drawingColor = ((Vector3) e.Value).ToDrawingColor();
                var solidBrush = new SolidBrush(drawingColor);
                e.Graphics.FillRectangle(solidBrush, e.Bounds);
            }
            else
            {
                var solidBrush = new SolidBrush((DrawingColor) e.Value);
                e.Graphics.FillRectangle(solidBrush, e.Bounds);
            }
        }

        void ResolveKnownColor(ref DrawingColor color, out DrawingColor result)
        {
            DrawingColor? tryColor;

            // try to resolve a web color
            ResolveKnownColor(ref color, KnownColor.Transparent, KnownColor.YellowGreen, out tryColor);
            if (tryColor.HasValue)
            {
                result = tryColor.Value;
                return;
            }

            // try to resolve a system color
            ResolveKnownColor(ref color, KnownColor.ActiveBorder, KnownColor.WindowText, out tryColor);
            if (tryColor.HasValue)
            {
                result = tryColor.Value;
                return;
            }

            ResolveKnownColor(ref color, KnownColor.ButtonFace, KnownColor.MenuHighlight, out tryColor);
            result = tryColor ?? color;
        }

        void ResolveKnownColor(ref DrawingColor color, KnownColor start, KnownColor end, out DrawingColor? result)
        {
            for (KnownColor i = start; i <= end; i++)
            {
                var colorFromKnownColor = DrawingColor.FromKnownColor(i);
                if (color.ToArgb() == colorFromKnownColor.ToArgb())
                {
                    result = colorFromKnownColor;
                    return;
                }
            }
            result = null;
        }
    }
}
