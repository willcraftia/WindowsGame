#region Using

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content.Pipeline;

#endregion

namespace Willcraftia.Win.Xna.Framework.Design
{
    public sealed class ExternalReferenceConverter<T> : TypeConverter
    {
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            if (destinationType == typeof(ExternalReference<T>))
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
            if (value is ExternalReference<T>)
            {
                if (destinationType == typeof(string))
                {
                    var reference = value as ExternalReference<T>;
                    return reference.Filename;
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
                var filename = value as string;
                var result = new ExternalReference<T>();
                result.Filename = filename;
                return result;
            }

            return base.ConvertFrom(context, culture, value);
        }
    }
}
