#region Using

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;

#endregion

namespace Willcraftia.Content.Studio.Models
{
    public sealed class ContentImporterConverter : StringConverter
    {
        const string NoValue = "(None)";

        static StandardValuesCollection standardValues;

        public static void Initialize(ICollection<string> importers)
        {
            var list = new List<string>(importers);
            list.Add(NoValue);
            list.Sort();
            standardValues = new StandardValuesCollection(list);
        }

        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            return true;
        }

        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            return standardValues;
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            var s = value as string;
            if (s == NoValue) return null;

            return base.ConvertFrom(context, culture, value);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType != typeof(string))
            {
                return base.ConvertTo(context, culture, value, destinationType);
            }

            if (value == null) return NoValue;

            var s = value.ToString();
            if (string.IsNullOrEmpty(s)) return NoValue;

            return base.ConvertTo(context, culture, value, destinationType);
        }
    }
}
