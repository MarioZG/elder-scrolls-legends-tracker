using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Markup;

namespace ESLTracker.Utils.Converters
{
    public abstract class MarkupConverter<T> : MarkupExtension where T : IValueConverter, new()
    {
        private static T converter = default(T);
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            if (converter == null)
            {                
                converter = new T();
            }
            return converter;
        }
    }
}
