using System;
using System.Collections.Generic;
using System.Globalization;
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
using ESLTracker.DataModel.Enums;
using ESLTracker.Utils;
using ESLTracker.ViewModels.Rewards;

namespace ESLTracker.Controls.Rewards
{
    /// <summary>
    /// Interaction logic for Reward.xaml
    /// </summary>
    public partial class RewardSet : UserControl
    {

        public RewardSet()
        {
            InitializeComponent();

            //this is ugly - need to research proper binding!
            this.rewardCard.ParentDataContext = this.DataContext as RewardSetViewModel;
            this.rewardGold.ParentDataContext = this.DataContext as RewardSetViewModel;
            this.rewardPack.ParentDataContext = this.DataContext as RewardSetViewModel;
            this.rewardSoulGem.ParentDataContext = this.DataContext as RewardSetViewModel;

        }

    }
}
