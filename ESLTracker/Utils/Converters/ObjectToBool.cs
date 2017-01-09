using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESLTracker.Utils.Converters
{
    public class ObjectToBool : InvertibleConverter<bool, object>
    {

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            if (converter == null)
            {
                converter = new ObjectToBool();
            }
            return converter;
        }

        protected override bool ReturnWhenFalse
        {
            get
            {
                return false;
            }
        }

        protected override bool ReturnWhenTrue
        {
            get
            {
               return true;
            }
        }

        protected override bool Condition(object value)
        {
            return value != null;
        }
    }
}
