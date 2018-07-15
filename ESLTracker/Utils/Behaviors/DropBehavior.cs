using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace ESLTracker.Utils.Behaviors
{
    public static class DropBehavior
    {
        public static void SetDropCommand(this UIElement element, ICommand command)
        {
            element.SetValue(DropCommandProperty, command);
        }

        public static ICommand GetDropCommand(UIElement element)
        {
            return (ICommand)element.GetValue(DropCommandProperty);
        }

        // Using a DependencyProperty as the backing store for Drop.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DropCommandProperty =
            DependencyProperty.RegisterAttached("DropCommand", typeof(ICommand), typeof(DropBehavior), new PropertyMetadata(DropCommandPropertyChanged));

        private static void DropCommandPropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs eventArgs)
        {
            UIElement uiElement = dependencyObject as UIElement;
            if (null == uiElement) return;

            uiElement.Drop += (sender, args) =>
            {
                GetDropCommand(uiElement).Execute(args.Data);
                args.Handled = true;
            };
        }
    }
}
