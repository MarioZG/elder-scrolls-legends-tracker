using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ESLTracker.Utils.Extensions;

namespace ESLTracker.Controls.Decks
{
    /// <summary>
    /// Interaction logic for DeckHistory.xaml
    /// </summary>
    public partial class DeckHistory : UserControl
    {
        public DeckHistory()
        {
            InitializeComponent();
        }

        private void ItemsControl_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            ScrollViewer scv = ((DependencyObject)sender).FindParent<ScrollViewer>();
            scv.ScrollToVerticalOffset(scv.VerticalOffset - e.Delta);
            e.Handled = true;
        }
    }
}
