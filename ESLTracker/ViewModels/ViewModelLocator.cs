using ESLTracker.Utils.SimpleInjector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESLTracker.ViewModels
{
    public class ViewModelLocator
    {
        public ViewModelLocator()
        {

        }

        public object this[string key]
        {
            get
            {
                return MasserContainer.Container.GetInstance(Type.GetType("ESLTracker.ViewModels."+key));
            }
        }
    }
}
