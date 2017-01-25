using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESLTracker.Utils.Converters
{
    class CollectionCountToVisibility : ToVisibilityConverter<IList>
    {
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            if (converter == null)
            {
                converter = new CollectionCountToVisibility();
            }
            return converter;
        }

        protected override bool Condition(object value)
        {
            return ((IList)value).Count > 0;
        }
    }
}
