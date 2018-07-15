using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace ESLTracker.Utils.Behaviors
{
    public static class DragBehavior
    {
        public static void SetStartDragCommand(this UIElement element, ICommand command)
        {
            element.SetValue(StartDragCommandProperty, command);
        }

        public static ICommand GetStartDragCommand(UIElement element)
        {
            return (ICommand)element.GetValue(StartDragCommandProperty);
        }

        // Using a DependencyProperty as the backing store for Drop.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty StartDragCommandProperty =
            DependencyProperty.RegisterAttached("StartDragCommand", typeof(ICommand), typeof(DragBehavior), new PropertyMetadata(StartDragCommandPropertyChanged));

        private static void StartDragCommandPropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs eventArgs)
        {
            UIElement uiElement = dependencyObject as UIElement;
            if (null == uiElement) return;

            uiElement.MouseLeftButtonDown += (sender, args) =>
            {
                GetStartDragCommand(uiElement).Execute(
                    new DragDropEventInfo() {
                        Source = dependencyObject as UIElement,
                        Data = GetStartDragCommandParameter(uiElement) }
                    );
                args.Handled = true;
            };
        }

        public static void SetStartDragCommandParameter(this UIElement element, object command)
        {
            element.SetValue(StartDragCommandParameterProperty, command);
        }

        public static object GetStartDragCommandParameter(UIElement element)
        {
            return element.GetValue(StartDragCommandParameterProperty);
        }

        public static readonly DependencyProperty StartDragCommandParameterProperty =
            DependencyProperty.RegisterAttached("StartDragCommandParameter", typeof(object), typeof(DragBehavior), new PropertyMetadata(null));

    }
}
