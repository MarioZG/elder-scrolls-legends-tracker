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
using Enums = TESLTracker.DataModel.Enums;

namespace ESLTracker.Controls.Decks
{
    /// <summary>
    /// Interaction logic for PlayerRank.xaml
    /// </summary>
    public partial class ArenaRank : UserControl
    {


        public Enums.ArenaRank? SelectedRank
        {
            get { return (Enums.ArenaRank?)GetValue(SelectedRankProperty); }
            set { SetValue(SelectedRankProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SelectedRank.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SelectedRankProperty =
            DependencyProperty.Register("SelectedRank", typeof(Enums.ArenaRank?), typeof(ArenaRank), new PropertyMetadata(null));






        public ArenaRank()
        {
            InitializeComponent();

        }
    }
}
