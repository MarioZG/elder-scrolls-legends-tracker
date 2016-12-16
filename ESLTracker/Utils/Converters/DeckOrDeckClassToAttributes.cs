using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using ESLTracker.DataModel;
using ESLTracker.DataModel.Enums;

namespace ESLTracker.Utils.Converters
{
    public class DeckOrDeckClassToAttributes : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Deck)
            {
                return ((Deck)value).Attributes.ImageSources;
            }
            else if (value is DeckClass)
            {
                return ClassAttributesHelper.Classes[(DeckClass)value].ImageSources;
            }
            else
            {
                throw new ArgumentOutOfRangeException("DeckOrDeckClassToAttributes received unexpetced type" + value.GetType().Name);
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
