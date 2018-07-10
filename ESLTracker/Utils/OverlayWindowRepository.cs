using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using ESLTracker.Controls;
using ESLTracker.Utils.SimpleInjector;

namespace ESLTracker.Utils
{
    public class OverlayWindowRepository : PropertiesObservableCollection<OverlayWindowBase>
    {

        public OverlayWindowRepository(IEnumerable<OverlayWindowBase> overlayWindows) : base(overlayWindows)
        {

        }

        internal T GetWindowByType<T>() where T: IOverlayWindow
        {
            return (T)GetWindowByType(typeof(T));
        }

        internal object GetWindowByType(Type obj)
        {
            return this.Where(w => w.GetType() == obj).FirstOrDefault();
        }

        internal bool IsAnyActive()
        {
            return this.Any(ow => ((Window)ow).IsActive);
        }
    }
}
