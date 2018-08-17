using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESLTracker.Utils
{
    public class DebugTraceListener : TraceListener
    {
        public override void Write(string message)
        {
        }

        public override void WriteLine(string message)
        {
            string[] errorFilter = { "{DisconnectedItem}'" ,
            "Cannot find source for binding with reference 'RelativeSource FindAncestor, AncestorType='System.Windows.Controls.ItemsControl', AncestorLevel='1''. BindingExpression:Path=VerticalContentAlignment; DataItem=null; target element is 'MenuItem' (Name=''); target property is 'VerticalContentAlignment' (type 'VerticalAlignment')",
            "Cannot find source for binding with reference 'RelativeSource FindAncestor, AncestorType='System.Windows.Controls.ItemsControl', AncestorLevel='1''. BindingExpression:Path=HorizontalContentAlignment; DataItem=null; target element is 'MenuItem' (Name=''); target property is 'HorizontalContentAlignment' (type 'HorizontalAlignment')",
            "Cannot find source for binding with reference 'RelativeSource FindAncestor, AncestorType='System.Windows.Controls.StackPanel', AncestorLevel='1''. BindingExpression:Path=ActualHeight; DataItem=null; target element is 'OverlayWindowTitlebarButton' (Name=''); target property is 'Height' (type 'Double')",
            "Cannot find source for binding with reference 'RelativeSource FindAncestor, AncestorType='System.Windows.Window', AncestorLevel='1''. BindingExpression:Path=CreateScreenshot; DataItem=null; target element is 'OverlayWindowTitlebarButton' (Name=''); target property is 'Command' (type 'ICommand')"
            };
            if (!errorFilter.Any(e => message.Contains(e)))
            {
               Debugger.Break();
            }
        }
    }
}
