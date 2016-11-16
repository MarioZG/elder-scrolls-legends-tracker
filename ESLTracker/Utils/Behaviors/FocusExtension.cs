using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ESLTracker.Utils.Behaviors
{
    /// <summary>
    /// http://stackoverflow.com/questions/1356045/set-focus-on-textbox-in-wpf-from-view-model-c
    /// answer http://stackoverflow.com/a/31272370/1250796
    /// </summary>
    public static class FocusExtension
    {
        public static bool GetIsFocused(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsFocusedProperty);
        }

        public static void SetIsFocused(DependencyObject obj, bool value)
        {
            obj.SetValue(IsFocusedProperty, value);
        }


        public static readonly DependencyProperty IsFocusedProperty =
            DependencyProperty.RegisterAttached(
             "IsFocused", typeof(bool), typeof(FocusExtension),
             new UIPropertyMetadata(false, null, OnCoerceValue));


        public static bool GetSelectAllOnFocus(DependencyObject obj)
        {
            return (bool)obj.GetValue(SelectAllOnFocus);
        }

        public static void SetSelectAllOnFocus(DependencyObject obj, bool value)
        {
            obj.SetValue(SelectAllOnFocus, value);
        }

        public static readonly DependencyProperty SelectAllOnFocus =
            DependencyProperty.RegisterAttached(
             "SelectAllOnFocus", typeof(bool), typeof(FocusExtension),
             new UIPropertyMetadata(false));

        private static object OnCoerceValue(DependencyObject d, object baseValue)
        {
            if ((bool)baseValue)
            {
                ((UIElement)d).Focus();
            }
            else if (((UIElement)d).IsFocused)
            {
                Keyboard.ClearFocus();
            }
            if ((d is TextBox) && GetSelectAllOnFocus(d))
            {
                ((System.Windows.Controls.TextBox)d).SelectAll();
            }
            return ((bool)baseValue);
        }
    }
}
