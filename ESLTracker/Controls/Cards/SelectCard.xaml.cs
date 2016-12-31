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

namespace ESLTracker.Controls.Cards
{
    /// <summary>
    /// Interaction logic for SelectCard.xaml
    /// </summary>
    public partial class SelectCard : UserControl
    {
        public CardInstance CardInstance
        {
            get { return (CardInstance)GetValue(CardProperty); }
            set { SetValue(CardProperty, value);  }
        }

        // Using a DependencyProperty as the backing store for Card.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CardProperty =
            DependencyProperty.Register("CardInstance", typeof(CardInstance), typeof(SelectCard), new PropertyMetadata(null, CardInstanceChanged));

        private static void CardInstanceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            d.SetValue(CardNameProperty, ((CardInstance)e.NewValue)?.Card?.Name);
        }

        public string CardName
        {
            get { return (string)GetValue(CardNameProperty); }
            set { SetValue(CardNameProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CardName.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CardNameProperty =
            DependencyProperty.Register("CardName", typeof(string), typeof(SelectCard), new PropertyMetadata(String.Empty, CardNameChanged));

        private static void CardNameChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != null)
            {
                if (((SelectCard)d).CardInstance == null)
                {
                    ((SelectCard)d).CardInstance = new CardInstance(CardsDatabase.Default.FindCardByName(e.NewValue?.ToString()));
                }
                else if (((SelectCard)d).CardInstance.Card?.Name != e.NewValue.ToString())
                {
                    ((SelectCard)d).CardInstance.Card = CardsDatabase.Default.FindCardByName(e.NewValue?.ToString());
                }
            }
        }

        public bool ShowIsGolden
        {
            get { return (bool)GetValue(ShowIsGoldenProperty); }
            set { SetValue(ShowIsGoldenProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ShowIsGolden.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ShowIsGoldenProperty =
            DependencyProperty.Register("ShowIsGolden", typeof(bool), typeof(SelectCard), new PropertyMetadata(false));


        public IEnumerable<string> CardNameAutocomplete
        {
            get { return (IEnumerable<string>)GetValue(CardNameAutocompleteProperty); }
            set { SetValue(CardNameAutocompleteProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CardNameAutocomplete.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CardNameAutocompleteProperty =
            DependencyProperty.Register("CardNameAutocomplete", typeof(IEnumerable<string>), typeof(SelectCard), new PropertyMetadata(null));

        public bool HasFocus
        {
            get { return (bool)GetValue(HasFocusProperty); }
            set { SetValue(HasFocusProperty, value); }
        }

        // Using a DependencyProperty as the backing store for HasFocus.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HasFocusProperty =
            DependencyProperty.Register("HasFocus", typeof(bool), typeof(SelectCard), new PropertyMetadata(false, FocusUpdate));

        private static void FocusUpdate(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
          //  throw new NotImplementedException();
        }

        public bool ReadOnly
        {
            get { return (bool)GetValue(ReadOnlyProperty); }
            set { SetValue(ReadOnlyProperty, value); }
        }

        public bool ReadOnlyNot
        {
            get { return ! (bool)GetValue(ReadOnlyProperty); }
        }

        // Using a DependencyProperty as the backing store for ReadOnly.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ReadOnlyProperty =
            DependencyProperty.Register("ReadOnly", typeof(bool), typeof(SelectCard), new PropertyMetadata(false));



        public SelectCard()
        {
            InitializeComponent();
            LayoutRoot.DataContext = this;
        }
    }
}
