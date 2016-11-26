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
    }
}
