using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;
using TESLTracker.DataModel.Enums;

namespace ESLTracker.Utils.Converters
{
    public class ClassToAttributesIcons : MarkupConverter<ClassToAttributesIcons>, IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null && value is DeckClass)
            {
                return ClassAttributesHelper.Classes[(DeckClass)value].ImageSources;
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }
}
