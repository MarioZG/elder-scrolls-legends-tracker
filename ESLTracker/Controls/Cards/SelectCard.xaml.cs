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

namespace ESLTracker.Controls.Cards
{
    /// <summary>
    /// Interaction logic for SelectCard.xaml
    /// </summary>
    public partial class SelectCard : UserControl
    {



        public string CardName
        {
            get { return (string)GetValue(CardNameProperty); }
            set { SetValue(CardNameProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CardName.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CardNameProperty =
            DependencyProperty.Register("CardName", typeof(string), typeof(SelectCard), new PropertyMetadata(String.Empty));



        public bool ShowIsGolden
        {
            get { return (bool)GetValue(ShowIsGoldenProperty); }
            set { SetValue(ShowIsGoldenProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ShowIsGolden.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ShowIsGoldenProperty =
            DependencyProperty.Register("ShowIsGolden", typeof(bool), typeof(SelectCard), new PropertyMetadata(false));



        public bool IsGolden
        {
            get { return (bool)GetValue(IsGoldenProperty); }
            set { SetValue(IsGoldenProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsGolden.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsGoldenProperty =
            DependencyProperty.Register("IsGolden", typeof(bool), typeof(SelectCard), new PropertyMetadata(false));



        public IEnumerable<string> CardNameAutocomplete
        {
            get { return (IEnumerable<string>)GetValue(CardNameAutocompleteProperty); }
            set { SetValue(CardNameAutocompleteProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CardNameAutocomplete.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CardNameAutocompleteProperty =
            DependencyProperty.Register("CardNameAutocomplete", typeof(IEnumerable<string>), typeof(SelectCard), new PropertyMetadata(null));




        public SelectCard()
        {
            InitializeComponent();
            LayoutRoot.DataContext = this;
        }
    }
}
