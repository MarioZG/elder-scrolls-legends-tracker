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
using ESLTracker.DataModel;
using ESLTracker.ViewModels;
using ESLTracker.ViewModels.Decks;

namespace ESLTracker.Controls.Decks
{
    /// <summary>
    /// Interaction logic for EditDeck.xaml
    /// </summary>
    public partial class EditDeck : UserControl
    {
        public Deck Deck
        {
            get { return (Deck)GetValue(DeckProperty); }
            set { SetValue(DeckProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Deck.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DeckProperty =
            DependencyProperty.Register(nameof(Deck), typeof(Deck), typeof(EditDeck), new PropertyMetadata(null, DeckChanged));

        private static void DeckChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((EditDeck)d).DataContext.Deck = (Deck)e.NewValue;
        }

        public new EditDeckViewModel DataContext
        {
            get
            {
                return base.DataContext as EditDeckViewModel;
            }
            set
            {
                base.DataContext = value;
            }
        }

        public EditDeck()
        {
            InitializeComponent();

            this.DataContext.DeckClassModel = this.deckClass.DataContext;
        }

    }
}
