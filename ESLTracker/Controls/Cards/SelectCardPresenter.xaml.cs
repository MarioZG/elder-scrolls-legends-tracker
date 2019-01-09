using TESLTracker.DataModel;
using ESLTracker.Utils.Extensions;
using ESLTracker.ViewModels.Cards;
using ESLTracker.Windows;
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

namespace ESLTracker.Controls.Cards
{
    /// <summary>
    /// Interaction logic for PresentCard.xaml
    /// </summary>
    public partial class SelectCardPresenter : UserControl
    {
        public CardInstance CardInstance
        {
            get { return (CardInstance)GetValue(CardProperty); }
            set { SetValue(CardProperty, value); }
        }

        public static readonly DependencyProperty CardProperty =
            DependencyProperty.Register("CardInstance", typeof(CardInstance), typeof(SelectCardPresenter), new PropertyMetadata(null, CardInstanceChanged));

        private static void CardInstanceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((SelectCardViewModel)((SelectCardPresenter)d).LayoutRoot.DataContext).CardInstance = e.NewValue as CardInstance;
        }

        public bool ShowIsPremium
        {
            get { return (bool)GetValue(ShowIsPremiumProperty); }
            set { SetValue(ShowIsPremiumProperty, value); }
        }

        public static readonly DependencyProperty ShowIsPremiumProperty =
            DependencyProperty.Register("ShowIsPremium", typeof(bool), typeof(SelectCardPresenter), new PropertyMetadata(false));



        public bool ShowQuantity
        {
            get { return (bool)GetValue(ShowQuantityProperty); }
            set { SetValue(ShowQuantityProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ShowQuantity.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ShowQuantityProperty =
            DependencyProperty.Register("ShowQuantity", typeof(bool), typeof(SelectCardPresenter), new PropertyMetadata(false));



        public IEnumerable<string> CardNameAutocomplete
        {
            get { return (IEnumerable<string>)GetValue(CardNameAutocompleteProperty); }
            set { SetValue(CardNameAutocompleteProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CardNameAutocomplete.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CardNameAutocompleteProperty =
            DependencyProperty.Register("CardNameAutocomplete", typeof(IEnumerable<string>), typeof(SelectCardPresenter), new PropertyMetadata(null));

        public bool ReadOnly
        {
            get { return (bool)GetValue(ReadOnlyProperty); }
            set { SetValue(ReadOnlyProperty, value); }
        }

        public bool ReadOnlyNot
        {
            get { return !(bool)GetValue(ReadOnlyProperty); }
        }

        // Using a DependencyProperty as the backing store for ReadOnly.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ReadOnlyProperty =
            DependencyProperty.Register("ReadOnly", typeof(bool), typeof(SelectCardPresenter), new PropertyMetadata(false));



        public ICommand MouseLeftClick
        {
            get { return (ICommand)GetValue(MouseLeftClickProperty); }
            set { SetValue(MouseLeftClickProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MouseLeftClick.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MouseLeftClickProperty =
            DependencyProperty.Register("MouseLeftClick", typeof(ICommand), typeof(SelectCardPresenter), new PropertyMetadata(null));

        public SelectCardPresenter()
        {
            InitializeComponent();
        }

        protected override void OnPreviewMouseDown(MouseButtonEventArgs e)
        {
            base.OnPreviewMouseDown(e);
            DeckOverlay window = this.FindParent<DeckOverlay>();
            if ((window != null) && (e.LeftButton == MouseButtonState.Pressed))
            {
                window.DragMove();
            }
        }
    }
}
