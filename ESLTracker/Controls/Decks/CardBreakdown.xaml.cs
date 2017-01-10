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
using ESLTracker.ViewModels.Decks;

namespace ESLTracker.Controls.Decks
{
    /// <summary>
    /// Interaction logic for CardBreakdown.xaml
    /// </summary>
    public partial class CardBreakdown : UserControl
    {
        public ObservableCollection<CardInstance> CardCollection
        {
            get { return (ObservableCollection<CardInstance>)GetValue(CardCollectionProperty); }
            set { SetValue(CardCollectionProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CardCollection.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CardCollectionProperty =
            DependencyProperty.Register("CardCollection", typeof(ObservableCollection<CardInstance>), typeof(CardBreakdown), new PropertyMetadata(new ObservableCollection<CardInstance>(), CardCollectionChanged));

        private static void CardCollectionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((CardBreakdown)d).DataContext.CardCollection = (ObservableCollection<CardInstance>)e.NewValue;
        }

        new public CardBreakdownViewModel DataContext
        {
            get
            {
                return (CardBreakdownViewModel)base.DataContext;
            }
            set
            {
                base.DataContext = value;
            }
        }

        public CardBreakdown()
        {
            InitializeComponent();
        }
    }
}
