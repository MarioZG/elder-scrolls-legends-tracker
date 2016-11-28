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

namespace ESLTracker.Controls.Decks
{
    /// <summary>
    /// Interaction logic for PlayerRank.xaml
    /// </summary>
    public partial class ArenaRank : UserControl
    {


        public DataModel.Enums.ArenaRank? SelectedRank
        {
            get { return (DataModel.Enums.ArenaRank?)GetValue(SelectedRankProperty); }
            set { SetValue(SelectedRankProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SelectedRank.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SelectedRankProperty =
            DependencyProperty.Register("SelectedRank", typeof(DataModel.Enums.ArenaRank?), typeof(ArenaRank), new PropertyMetadata(null));






        public ArenaRank()
        {
            InitializeComponent();

        }
    }
}
