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

namespace ESLTracker.Controls.Rewards
{
    /// <summary>
    /// Interaction logic for RewardItemList.xaml
    /// </summary>
    public partial class RewardItemList : UserControl
    {

        public Orientation PanelOrientation
        {
            get { return (Orientation)GetValue(PanelOrientationProperty); }
            set { SetValue(PanelOrientationProperty, value); }
        }

        // Using a DependencyProperty as the backing store for PanelOrientation.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PanelOrientationProperty =
            DependencyProperty.Register("PanelOrientation", typeof(Orientation), typeof(RewardItemList), new PropertyMetadata(Orientation.Vertical));
            


        public RewardItemList()
        {
            InitializeComponent();
        }
    }
}
