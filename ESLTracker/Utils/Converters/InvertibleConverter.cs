using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace ESLTracker.Utils.Converters
{
    public abstract class InvertibleConverter<TReturn, TValue> : IValueConverter
    {
        protected abstract bool Condition(object value);

        protected abstract TReturn ReturnWhenTrue { get; }

        protected abstract TReturn ReturnWhenFalse { get; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool not = false;
            if ((parameter != null) && (parameter.ToString().ToLowerInvariant() == "not"))
            {
                not = true;
            }
            if (value != null && value is TValue)
            {
                return not ^ Condition(value) ? ReturnWhenTrue : ReturnWhenFalse;
            }
            return not  ? ReturnWhenTrue : ReturnWhenFalse;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }
}