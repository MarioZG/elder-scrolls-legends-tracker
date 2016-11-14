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
    /// Interaction logic for EditDeck.xaml
    /// </summary>
    public partial class EditDeck : UserControl
    {
        public EditDeck()
        {
            InitializeComponent();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            DataModel.Deck deck = new DataModel.Deck()
            {
                Name = this.txtName.Text,
                Class = deckClass.SelectedClass,
                Attributes = new DataModel.DeckAttributes(),
                Type = (DataModel.Enums.DeckType)this.cbDeckType.SelectedItem 
                
            };
            deck.Attributes.AddRange(deckClass.SelectedClassAttributes);
            DataModel.Tracker.Instance.Decks.Add(deck);
            this.Visibility = Visibility.Collapsed;
        }
    }
}
