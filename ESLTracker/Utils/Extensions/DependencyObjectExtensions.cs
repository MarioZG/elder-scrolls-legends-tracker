using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace ESLTracker.Utils.Extensions
{
    public static class DependencyObjectExtensions
    {
        //http://www.infragistics.com/community/blogs/blagunas/archive/2013/05/29/find-the-parent-control-of-a-specific-type-in-wpf-and-silverlight.aspx
        public static T FindParent<T>(this DependencyObject child) where T : DependencyObject
        {
            //get parent item
            DependencyObject parentObject = VisualTreeHelper.GetParent(child);

            //we've reached the end of the tree
            if (parentObject == null) return null;

            //check if the parent matches the type we're looking for
            T parent = parentObject as T;
            if (parent != null)
                return parent;
            else
                return FindParent<T>(parentObject);
        }
    }
}
