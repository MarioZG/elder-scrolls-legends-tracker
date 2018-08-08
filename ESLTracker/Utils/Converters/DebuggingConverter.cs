using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using ESLTracker.DataModel;

namespace ESLTracker.Utils.Converters
{
    public class DebuggingConverter : MarkupConverter<DebuggingConverter>, IMultiValueConverter, IValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (System.Diagnostics.Debugger.IsAttached)
            {
                System.Diagnostics.Debugger.Log(1, "",
                    string.Format("value={0}; TT={1}; param={2}" + Environment.NewLine,
                    string.Join("~", values),
                    targetType,
                    parameter));
                System.Diagnostics.Debugger.Break();
            }
            return values;
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(System.Diagnostics.Debugger.IsAttached)
            {
                System.Diagnostics.Debugger.Log(1, "", 
                    string.Format("value={0}; TT={1}; param={2}"+Environment.NewLine,
                    value, 
                    targetType,
                    parameter));
                System.Diagnostics.Debugger.Break();
            }
            //return System.Windows.Visibility.Collapsed;
            return value;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
