#region Using

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
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
    public sealed class Vector3TypeConverter : TypeConverter
    {
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            if (destinationType == typeof(Vector3))
            {
                return true;
            }
            
            return base.CanConvertTo(context, destinationType);
        }

        public override object ConvertTo(
            ITypeDescriptorContext context,
            CultureInfo culture,
            object value,
            Type destinationType)
        {
            if (value is Vector3)
            {
                if (destinationType == typeof(string))
                {
                    var vector = (Vector3) value;
                    return vector.X + " " + vector.Y + " " + vector.Z;
                }
            }

            return base.ConvertTo(context, culture, value, destinationType);
        }

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (sourceType == typeof(string))
            {
                return true;
            }

            return base.CanConvertFrom(context, sourceType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value is string)
            {
                var elements = value.ToString().Split(new char[] { ' ', ',', ':' }, 3);
                if (elements.Length == 0)
                {
                    return Vector3.Zero;
                }
                else if (elements.Length == 1)
                {
                    return new Vector3(float.Parse(elements[0]));
                }
                else if (elements.Length == 2)
                {
                    var result = new Vector3();
                    result.X = float.Parse(elements[0]);
                    result.Y = float.Parse(elements[1]);
                    return result;
                }
                else
                {
                    var result = new Vector3();
                    result.X = float.Parse(elements[0]);
                    result.Y = float.Parse(elements[1]);
                    result.Z = float.Parse(elements[2]);
                    return result;
                }
            }

            return base.ConvertFrom(context, culture, value);
        }
    }
}
