using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;
using TESLTracker.DataModel.Enums;
using ESLTracker.Utils.Extensions;

namespace ESLTracker.Utils.Converters
{
    public class ColorToBrushConverter : MarkupConverter<ColorToBrushConverter>, IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null && value is Color)
            {
                var convertedValue = (Color)value;
                return convertedValue != Color.Transparent ? convertedValue.ToMediaBrush() : null;
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }
}
