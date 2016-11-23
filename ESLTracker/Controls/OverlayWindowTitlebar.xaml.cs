using System;
using System.Collections.Generic;
using Drawing = System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ESLTracker.Utils;

namespace ESLTracker.Controls
{
    /// <summary>
    /// Interaction logic for OverlayWindowTitlebar.xaml
    /// </summary>
    public partial class OverlayWindowTitlebar : UserControl
    {

        public OverlayWindowTitlebar()
        {
            InitializeComponent();
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Window.GetWindow(this).Close();
        }

        private void btnCollapse_Click(object sender, RoutedEventArgs e)
        {
            ToggleButton tb = sender as ToggleButton;
            if (tb != null)
            {
                Utils.WindowExtensions.FindVisualChildren<TabControl>(this);
                ((System.Windows.Controls.StackPanel)Window.GetWindow(this).Content).Children[1].Visibility =
                    tb.IsChecked.Value ?  Visibility.Hidden : Visibility.Visible;
                ((System.Windows.Controls.StackPanel)Window.GetWindow(this).Content).Background =
                    tb.IsChecked.Value ? null : SystemColors.ControlBrush;
            }
            e.Handled = false;
        }

        private void btnScreenShot_Click(object sender, RoutedEventArgs e)
        {
            FileManager.SaveScreenShot(this);
        }

    }
}
