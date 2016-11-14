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

namespace ESLTracker.Controls.Rewards
{
    /// <summary>
    /// Interaction logic for Reward.xaml
    /// </summary>
    public partial class RewardSet : UserControl
    {
        List<DataModel.Reward> Rewards = new List<DataModel.Reward>();

        List<AddSingleReward> rewardControls;

        public RewardSet()
        {
            InitializeComponent();

            rewardControls = new List<AddSingleReward>()
            {
                rewardCard,rewardGold, rewardPack,this.rewardSoulGem
            };

            this.dataGrid.ItemsSource = Rewards;
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            var newRewards = Rewards.Where(r => !DataModel.Tracker.Instance.Rewards.Contains(r));
            DataModel.Tracker.Instance.Rewards.AddRange(newRewards);
            Rewards = new List<DataModel.Reward>();
            this.dataGrid.ItemsSource = Rewards;
            this.cbReason.SelectedItem = null;
        }

        private void AddSingleReward_ControlClicked(object sender, EventArgs e)
        {
            foreach(AddSingleReward asr in rewardControls)
            {
                if (sender != asr)
                {
                    asr.Visibility = Visibility.Hidden;
                    asr.Reset();
                }
                else
                {
                    asr.SetGuildSelection(
                        ((asr.Type == RewardType.Gold) &&
                        ((RewardReason)this.cbReason.SelectedItem == RewardReason.Quest))
                        ? Visibility.Visible : Visibility.Hidden);

                    asr.Margin = new Thickness(asr.ActualWidth/2, asr.ActualHeight/2, asr.ActualWidth / 2, asr.ActualHeight / 2);
                    int col = (int)asr.GetValue(Grid.ColumnProperty);
                    int row = (int)asr.GetValue(Grid.RowProperty);

                    //hide opposite row/column
                    this.grid.ColumnDefinitions[(col + 1) % 2].Width = new GridLength(0);
                    this.grid.RowDefinitions[(row + 1) % 2].Height = new GridLength(0);
                }
            }
        }

        private void rewardGold_NewReward(object sender, NewRewardEventArgs e)
        {
            if (e.Reward != null)
            {
                e.Reward.Reason = (RewardReason)this.cbReason.SelectedItem;
                Rewards.Add(e.Reward);
            }
            else
            {
                //close - do nothing
            }
            this.dataGrid.Items.Refresh();

            ResetRewardsGrid();

        }

        private void cbReason_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ResetRewardsGrid();
        }

        private void ResetRewardsGrid()
        {
            foreach (AddSingleReward asr in rewardControls)
            {
                asr.Visibility = Visibility.Visible;
                asr.Reset();
            }
            this.grid.ColumnDefinitions[0].Width = new GridLength(5, GridUnitType.Star);
            this.grid.ColumnDefinitions[1].Width = new GridLength(5, GridUnitType.Star);
            this.grid.RowDefinitions[0].Height = new GridLength(5, GridUnitType.Star);
            this.grid.RowDefinitions[1].Height = new GridLength(5, GridUnitType.Star);
        }
    }
}
