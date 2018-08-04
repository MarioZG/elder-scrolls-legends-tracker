using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace ESLTracker.Utils.Converters
{
    public class ShowOverlayWindowToBool : MarkupConverter<ShowOverlayWindowToBool>, IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
           if (value != null && value is IEnumerable<IOverlayWindow> && parameter != null)
            {
                var window = ((IEnumerable<IOverlayWindow>)value).Where(w => w.GetType() == (Type)parameter).FirstOrDefault();
                var retValue = window?.ShowOnScreen;
                return retValue;
            }
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }
}
