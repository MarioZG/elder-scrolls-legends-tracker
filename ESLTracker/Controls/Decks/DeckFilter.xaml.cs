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
using ESLTracker.ViewModels.Decks;

namespace ESLTracker.Controls.Decks
{
    /// <summary>
    /// Interaction logic for DeckFilter.xaml
    /// </summary>
    public partial class DeckFilter : UserControl
    {
        public DeckFilter()
        {
            InitializeComponent();
        }

        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            DeckListViewModel model = ((DeckListViewModel)this.DataContext);
            model.DeckTextSearchEntered = true;
        }

        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            DeckListViewModel model = ((DeckListViewModel)this.DataContext);
            if (String.IsNullOrWhiteSpace(model.DeckTextSearch))
            {
                model.DeckTextSearchEntered = false;
            }
        }
    }
}
