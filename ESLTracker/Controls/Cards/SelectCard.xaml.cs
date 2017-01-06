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
using System.Windows.Threading;
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

        public bool ShowIsPremium
        {
            get { return (bool)GetValue(ShowIsPremiumProperty); }
            set { SetValue(ShowIsPremiumProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ShowIsPremium.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ShowIsPremiumProperty =
            DependencyProperty.Register("ShowIsPremium", typeof(bool), typeof(SelectCard), new PropertyMetadata(false));



        public bool ShowQuantity
        {
            get { return (bool)GetValue(ShowQuantityProperty); }
            set { SetValue(ShowQuantityProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ShowQuantity.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ShowQuantityProperty =
            DependencyProperty.Register("ShowQuantity", typeof(bool), typeof(SelectCard), new PropertyMetadata(false));



        public IEnumerable<string> CardNameAutocomplete
        {
            get { return (IEnumerable<string>)GetValue(CardNameAutocompleteProperty); }
            set { SetValue(CardNameAutocompleteProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CardNameAutocomplete.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CardNameAutocompleteProperty =
            DependencyProperty.Register("CardNameAutocomplete", typeof(IEnumerable<string>), typeof(SelectCard), new PropertyMetadata(null));

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
            this.IsVisibleChanged += SelectCard_IsVisibleChanged;
        }
  
        //https://www.codeproject.com/Tips/478376/Setting-focus-to-a-control-inside-a-usercontrol-in
        //http://stackoverflow.com/questions/9535784/setting-default-keyboard-focus-on-loading-a-usercontrol
        private void SelectCard_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (this.Focusable && ((bool)e.NewValue == true))
            {
                Dispatcher.BeginInvoke(
                DispatcherPriority.ContextIdle,
                new Action(delegate ()
                {
                    txtName.Focus();
                }));
            }
        }
    }
}
