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
    public class BoolToVisibilty : ToVisibilityConverter<bool>
    {
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            if (converter == null)
            {
                converter = new BoolToVisibilty();
            }
            return converter;
        }

        protected override bool Condition(object value)
        {
            return (bool)value;
        }
    }
}
