using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace ESLTracker.Utils.Converters
{
    public class BoolToVisibiltyCollapsedConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool not = false;
            if ((parameter != null) && (parameter.ToString().ToUpper() == "NOT"))
            {
                not = true;
            }
            if (value != null && value is bool)
            {
                return not ^ (bool)value ? Visibility.Visible : Visibility.Collapsed;
            }
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }
}
