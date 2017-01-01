using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace ESLTracker.Utils.Converters
{
    public class StringNonEmptyToVisibiltyCollapsedConverter : InvertibleConverter<Visibility, string>
    {
        protected override Visibility ReturnWhenFalse
        {
            get
            {
                return Visibility.Collapsed;
            }
        }

        protected override Visibility ReturnWhenTrue
        {
            get
            {
                return Visibility.Visible;
            }
        }

        protected override bool Condition(object value)
        {
            return !String.IsNullOrWhiteSpace((String)value);
        }
    }
}
