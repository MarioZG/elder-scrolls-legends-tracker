using ESLTracker.BusinessLogic.Packs;
using ESLTracker.DataModel;
using ESLTracker.ViewModels.Packs;
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

namespace ESLTracker.Controls.Packs
{
    /// <summary>
    /// Interaction logic for SetsDropdown.xaml
    /// </summary>
    public partial class SetsDropdown : UserControl
    {
              
        public CardSet SelectedSet
        {
            get { return (CardSet)GetValue(SelectedSetProperty); }
            set { SetValue(SelectedSetProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SelectedSet.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SelectedSetProperty =
            DependencyProperty.Register(nameof(SelectedSet), typeof(CardSet), typeof(SetsDropdown), new PropertyMetadata(null));

        public bool ShowAllOption
        {
            get { return (bool)GetValue(ShowAllOptionProperty); }
            set { SetValue(ShowAllOptionProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ShowAllOption.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ShowAllOptionProperty =
            DependencyProperty.Register(nameof(ShowAllOption), typeof(bool), typeof(SetsDropdown), new PropertyMetadata(false, ShowAllOptionChanged));

        private static void ShowAllOptionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d != null && d is SetsDropdown)
            {
                FrameworkElement vmProvider = ((SetsDropdown)d).vmProvider;
                var castedDC = (SetsDropdownViewModel)vmProvider?.DataContext;
                d.SetValue(SetsListProperty, castedDC?.GetCardSetList((bool)e.NewValue));
            }
        }

        public SetsDropdown()
        {
            InitializeComponent();
            ShowAllOptionChanged(this, new DependencyPropertyChangedEventArgs(ShowAllOptionProperty, null, false));
        }
               
        public IEnumerable<CardSet> SetsList
        {
            get { return (IEnumerable<CardSet>)GetValue(SetsListProperty); }
            set { SetValue(SetsListProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SetsList.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SetsListProperty =
            DependencyProperty.Register(nameof(SetsList), typeof(IEnumerable<CardSet>), typeof(SetsDropdown), new PropertyMetadata(null));



    }
}
