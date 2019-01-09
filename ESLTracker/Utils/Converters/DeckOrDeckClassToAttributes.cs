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
    public class DeckOrDeckClassToAttributes : MarkupConverter<DeckOrDeckClassToAttributes>, IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Deck)
            {
                Deck deck = (Deck)value;
                if (deck.Class.HasValue)
                {
                    return ClassAttributesHelper.Classes[deck.Class.Value].ImageSources;
                }
                else
                {
                    return new DeckAttributes();
                }
            }
            else if (value is DeckClass)
            {
                return ClassAttributesHelper.Classes[(DeckClass)value].ImageSources;
            }
            else if (value is string)
            {
                return String.Empty;
            }
            else if (value == null)
            {
                return String.Empty;
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
