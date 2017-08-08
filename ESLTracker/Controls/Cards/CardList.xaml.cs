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
using ESLTracker.Utils.Extensions;
using System.ComponentModel;

namespace ESLTracker.Controls.Cards
{
    /// <summary>
    /// Interaction logic for CardList.xaml
    /// </summary>
    public partial class CardList : UserControl
    {

        public ICommand MouseLeftClick
        {
            get { return (ICommand)GetValue(MouseLeftClickProperty); }
            set { SetValue(MouseLeftClickProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MouseLeftClick.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MouseLeftClickProperty =
            DependencyProperty.Register("MouseLeftClick", typeof(ICommand), typeof(CardList), new PropertyMetadata(null));



        public string SortingProperties
        {
            get { return (string)GetValue(SortingPropertiesProperty); }
            set { SetValue(SortingPropertiesProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SortingOrder.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SortingPropertiesProperty =
            DependencyProperty.Register("SortingProperties", typeof(string), typeof(CardList), new PropertyMetadata("Card.Cost-Card.Name"));



        public string SortingDirections
        {
            get { return (string)GetValue(SortingDirectionsProperty); }
            set { SetValue(SortingDirectionsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SortingDirections.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SortingDirectionsProperty =
            DependencyProperty.Register("SortingDirections", typeof(string), typeof(CardList), new PropertyMetadata("A-A"));





        public CardList()
        {
            InitializeComponent();
        }

        private void ItemsControl_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {            
            ScrollViewer scv = ((DependencyObject)sender).FindParent<ScrollViewer>();
            if (scv != null)
            {
                scv.ScrollToVerticalOffset(scv.VerticalOffset - e.Delta);
                e.Handled = true;
            }
        }

        private void itemsList_Loaded(object sender, RoutedEventArgs e)
        {
            ICollectionView defaultView = CollectionViewSource.GetDefaultView(itemsList.ItemsSource);
            if ((defaultView != null) && (! String.IsNullOrWhiteSpace(SortingProperties)))
            {
                defaultView.SortDescriptions.Clear();
                var sortingDirections = SortingDirections.Split(new char[] { '-' });
                int i = 0;
                foreach (var sort in SortingProperties.Split(new char[] { '-' }))
                {
                    var sortDirection = ListSortDirection.Ascending;
                    if (sortingDirections.Length > i)
                    {
                        var sdValue = sortingDirections[i++];
                        if ((sdValue .ToUpper() == "D") || (sdValue == "1"))
                        {
                            sortDirection = ListSortDirection.Descending;
                        }
                    }
                    defaultView.SortDescriptions.Add(new SortDescription(sort, sortDirection));
                }
            }
        }
    }
}
