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

namespace ESLTracker.Controls
{
    /// <summary>
    /// Interaction logic for PlayerRank.xaml
    /// </summary>
    public partial class PlayerRank : UserControl
    {

        public DataModel.Enums.PlayerRank? SelectedRank
        {
            get { return (DataModel.Enums.PlayerRank?)GetValue(SelectedItemProperty); }
            set { SetValue(SelectedItemProperty, value); }
        }

        // Using a DependencyProperty as the backing store for PlayerRank.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SelectedItemProperty =
            DependencyProperty.Register("SelectedRank", typeof(DataModel.Enums.PlayerRank?), typeof(PlayerRank), new PropertyMetadata(null));


        public int? LegendRank
        {
            get { return (SelectedRank == DataModel.Enums.PlayerRank.TheLegend) ?
                        (int?)GetValue(LegendRankProperty) : null; }
            set { SetValue(LegendRankProperty, value); }
        }

        // Using a DependencyProperty as the backing store for LegendRank.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LegendRankProperty =
            DependencyProperty.Register("LegendRank", typeof(int?), typeof(PlayerRank), new PropertyMetadata(null));

        public string DataContextPath
        {
            get { return (string)GetValue(DataContextPropertyProperty); }
            set { SetValue(DataContextPropertyProperty, value); }
        }

        // Using a DependencyProperty as the backing store for DataContextProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DataContextPropertyProperty =
            DependencyProperty.Register("DataContextPath", typeof(string), typeof(PlayerRank), new PropertyMetadata(null, Changed));

        private static void Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            //TODO: Find a way to move it to xaml!
            if (((PlayerRank)d).DataContextPath != null)
            {
                var nameOfPropertyInVm = ((PlayerRank)d).DataContextPath.Split(new char[] { ',' })[0];
                var binding = new Binding(nameOfPropertyInVm) { Mode = BindingMode.TwoWay };
                ((PlayerRank)d).SetBinding(SelectedItemProperty, binding);

                nameOfPropertyInVm = ((PlayerRank)d).DataContextPath.Split(new char[] { ',' })[1];
                binding = new Binding(nameOfPropertyInVm) { Mode = BindingMode.TwoWay };
                ((PlayerRank)d).SetBinding(LegendRankProperty, binding);
            }
        }

        public PlayerRank()
        {
            InitializeComponent();

        }

        private string typedChars = String.Empty;
        private void cbPlayerRank_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            var input = (char)KeyInterop.VirtualKeyFromKey(e.Key);
            typedChars += input;
            DataModel.Enums.PlayerRank rank;
            if (IsValidPlayerRank(typedChars, out rank))
            {
                ((ComboBox)sender).SelectedItem = rank;
            }
            else
            {
                typedChars = input.ToString(); //try just latest pressed key
                if (IsValidPlayerRank(typedChars, out rank))
                {
                    ((ComboBox)sender).SelectedItem = rank;
                }
                else
                {
                    typedChars = String.Empty;
                }
            }
        }

        private bool IsValidPlayerRank(string value, out DataModel.Enums.PlayerRank rank)
        {
            return Enum.TryParse(value, out rank) && Enum.IsDefined(typeof(DataModel.Enums.PlayerRank), rank);
        }

        private void txtPlayerLegendRank_GotFocus(object sender, RoutedEventArgs e)
        {
            ((TextBox)sender).SelectAll();
        }
    }
}
