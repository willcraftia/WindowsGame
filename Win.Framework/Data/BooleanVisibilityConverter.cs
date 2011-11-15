#region Using

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;

#endregion

namespace Willcraftia.Win.Framework.Data
{
    public sealed class BooleanVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var visibility = Visibility.Collapsed;
            try
            {
                var booleanValue = bool.Parse(value.ToString());
                if (booleanValue)
                {
                    visibility = Visibility.Visible;
                }
            }
            catch { }
            return visibility;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
}
