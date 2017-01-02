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
    public class StringNonEmptyToVisibiltyCollapsedConverter : ToVisibilityConverter<string>
    {
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            if (converter == null)
            {
                converter = new StringNonEmptyToVisibiltyCollapsedConverter();
            }
            return converter;
        }

        protected override bool Condition(object value)
        {
            return !String.IsNullOrWhiteSpace((String)value);
        }
    }
}
