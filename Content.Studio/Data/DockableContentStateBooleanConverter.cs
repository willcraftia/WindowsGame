#region Using

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Data;

#endregion

namespace Willcraftia.Content.Studio.Data
{
    public sealed class DockableContentStateBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is AvalonDock.DockableContentState)) return null;

            var state = (AvalonDock.DockableContentState) value;
            return state != AvalonDock.DockableContentState.Hidden;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
}
