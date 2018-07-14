using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace ESLTracker.Utils.Converters
{
    public class EnumToIntConverter : MarkupConverter<ClassToAttributesIcons>, IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(! (value is Enum))
            {
                throw new ArgumentException(nameof(EnumToIntConverter) + " can accept only enums as parameters");
            }
            return (int)value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }
}
