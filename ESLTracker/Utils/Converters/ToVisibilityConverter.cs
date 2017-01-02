using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ESLTracker.Utils.Converters
{
    public abstract class ToVisibilityConverter<TValue> : InvertibleConverter<Visibility, TValue>
    {

        Visibility returnWhenFalse = Visibility.Collapsed;

        protected override Visibility ReturnWhenFalse
        {
            get
            {
                return returnWhenFalse;
            }
        }

        protected override Visibility ReturnWhenTrue
        {
            get
            {
                return Visibility.Visible;
            }
        }

        protected override void CheckForOtherParameters(object parameter)
        {
            bool falseIsCollapsed = ((parameter != null)
                        && (parameter.ToString().ToLowerInvariant().Split(ParameterSeparator)
                            .Any(s => s == Visibility.Collapsed.ToString().ToLowerInvariant())
                        ));
            bool falseIsHidden = ((parameter != null)
                        && (parameter.ToString().ToLowerInvariant().Split(ParameterSeparator)
                            .Any(s => s == Visibility.Hidden.ToString().ToLowerInvariant())
            ));
            if (falseIsCollapsed && falseIsHidden)
            {
                throw new ArgumentException("Only one of Hidden and Collpased can be passed");
            }
            if (falseIsCollapsed)
            {
                returnWhenFalse = Visibility.Collapsed;
            }
            if (falseIsHidden)
            {
                returnWhenFalse = Visibility.Hidden;
            }
        }
    }
}
