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

        public static char[] ParameterSeparator =  { '-' };

        /// <summary>
        /// Method to check for other paramaters passed to converter. inversion ('not') is checkd in this class
        /// </summary>
        protected virtual void CheckForOtherParameters(object parameter)
        {
            //do nothing
        }

        /// <summary>
        /// check conditon of value
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        protected abstract bool Condition(object value);

        protected abstract TReturn ReturnWhenTrue { get; }

        protected abstract TReturn ReturnWhenFalse { get; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool not = false;
            not = CheckInversion(parameter);
            CheckForOtherParameters(parameter);
            if (value != null && value is TValue)
            {
                return not ^ Condition(value) ? ReturnWhenTrue : ReturnWhenFalse;
            }
            return not  ? ReturnWhenTrue : ReturnWhenFalse;
        }

        private bool CheckInversion(object parameter)
        {
            return ((parameter != null)
                        && (parameter.ToString().ToLowerInvariant().Split(ParameterSeparator)
                                    .Any(s => s == "not")
                    ));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }
}