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
using ESLTracker.Utils;
using ESLTracker.Utils.Messages;
using ESLTracker.Utils.SimpleInjector;
using ESLTracker.ViewModels.Decks;

namespace ESLTracker.Controls.Decks
{
    /// <summary>
    /// Interaction logic for DeckItem.xaml
    /// </summary>
    public partial class DeckItem : UserControl
    {

        new public DeckItemViewModel DataContext
        {
            get
            {
                return (DeckItemViewModel)base.DataContext;
            }
            set
            {
                base.DataContext = value;
            }
        }

        public Deck Deck
        {
            get { return (Deck)GetValue(DeckProperty); }
            set { SetValue(DeckProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Deck.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DeckProperty =
            DependencyProperty.Register("Deck", typeof(Deck), typeof(DeckItem), new PropertyMetadata(DeckPropertyUpdated));

        private static void DeckPropertyUpdated(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((DeckItem)d).DataContext.Deck = (Deck)e.NewValue;
        }

        public DeckItem()
        {
            InitializeComponent();
        }
    }
}
