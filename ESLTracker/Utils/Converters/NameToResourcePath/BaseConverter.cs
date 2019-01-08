using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Data;

namespace ESLTracker.Utils.Converters.NameToResourcePath
{
    public abstract class BaseConverter<T> : MarkupConverter<T>, IValueConverter
        where T : new()
    {

        protected abstract string Path { get; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            ValidateValue(value);
            string castedValue = value?.ToString();
            if (ShouldConvert(castedValue))
            {
                Regex rgx = new Regex("[^a-zA-Z0-9]");
                var name = rgx.Replace(castedValue, "");
                return $"pack://application:,,,/{Path}/{name}.png";
            }
            else
            {
                return null;// Binding.DoNothing;
            }
        }

        protected abstract void ValidateValue(object value);

        protected abstract bool ShouldConvert(string castedValue);

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }
}
