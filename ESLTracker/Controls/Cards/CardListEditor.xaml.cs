using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using ESLTracker.ViewModels.Cards;

namespace ESLTracker.Controls.Cards
{
    /// <summary>
    /// Interaction logic for CardListEditor.xaml
    /// </summary>
    public partial class CardListEditor : UserControl
    {
        public ObservableCollection<CardInstance> CardCollection
        {
            get { return (ObservableCollection<CardInstance>)GetValue(CardCollectionProperty); }
            set { SetValue(CardCollectionProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CardCollection.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CardCollectionProperty =
            DependencyProperty.Register("CardCollection", typeof(ObservableCollection<CardInstance>), typeof(CardListEditor), new PropertyMetadata(new ObservableCollection<CardInstance>(), CardCollectionChanged));

        private static void CardCollectionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((CardListEditor)d).DataContext.CardsCollection = (ObservableCollection<CardInstance>)e.NewValue;
        }

        public bool LimitCardCount
        {
            get { return (bool)GetValue(LimitCardCountProperty); }
            set { SetValue(LimitCardCountProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MaxCardQuantity.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LimitCardCountProperty =
            DependencyProperty.Register(nameof(LimitCardCount), typeof(bool), typeof(CardListEditor), new PropertyMetadata(false, LimitCardCountChanged));

        private static void LimitCardCountChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((CardListEditor)d).DataContext.LimitCardCount = (bool)e.NewValue;
        }

        new public CardListEditorViewModel DataContext
        {
            get
            {
                return (CardListEditorViewModel)base.DataContext;
            }
            set
            {
                base.DataContext = value;
            }
        }       

        public CardListEditor()
        {
            InitializeComponent();
        }
    }
}
