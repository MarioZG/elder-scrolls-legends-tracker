using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using TESLTracker.DataModel;
using TESLTracker.DataModel.Enums;

namespace ESLTracker.Utils.Converters
{
    public class DeckOrDeckClassToName : MarkupConverter<DeckOrDeckClassToName>,IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Deck)
            {
                return ((Deck)value).Name;
            }
            else if (value is DeckClass)
            {
                return ClassAttributesHelper.Classes[(DeckClass)value].ToString();
            }
            else if (value is string)
            {
                return value as string;
            }
            else if (value == null)
            {
                return "<no tag>";
            }
            else
            {
                throw new ArgumentOutOfRangeException("DeckOrDeckClassToName received unexpetced type" + value.GetType().Name);
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
