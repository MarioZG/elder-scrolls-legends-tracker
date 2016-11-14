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

namespace ESLTracker.Controls.Decks
{
    /// <summary>
    /// Interaction logic for DeckList.xaml
    /// </summary>
    public partial class DeckList : UserControl
    {
        public DeckList()
        {
            InitializeComponent();
            this.listBox.ItemsSource = DataModel.Tracker.Instance.Decks;
            DataModel.Tracker.Instance.Games.CollectionChanged += Games_CollectionChanged;
        }

        private void Games_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            this.listBox.Items.Refresh();
        }

        private void listBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DataModel.Tracker.Instance.ActiveDeck = (DataModel.Deck)((ListBox)e.Source).SelectedItem;
        }
    }
}
