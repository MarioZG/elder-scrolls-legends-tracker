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
using System.Windows.Shapes;
using ESLTracker.DataModel.Enums;

namespace ESLTracker
{
    /// <summary>
    /// Interaction logic for RewardsSummary.xaml
    /// </summary>
    public partial class RewardsSummary : Window
    {
        public RewardsSummary()
        {
            InitializeComponent();

            StringBuilder summary = new StringBuilder();
            DataModel.Tracker t = DataModel.Tracker.Instance;

            summary.AppendFormat("Gold: {0} in {1} rewards"+Environment.NewLine, 
                t.GetRewardsSummaryByType(RewardType.Gold).Sum(r=> r.Quantity),
                t.GetRewardsSummaryByType(RewardType.Gold).Count()
                );

            summary.AppendFormat("SoulGems: {0} in {1} rewards" + Environment.NewLine,
    t.GetRewardsSummaryByType(RewardType.SoulGem).Sum(r => r.Quantity),
    t.GetRewardsSummaryByType(RewardType.SoulGem).Count()
    );

            summary.AppendFormat("Cards: {0} in {1} rewards" + Environment.NewLine,
    t.GetRewardsSummaryByType(RewardType.Card).Sum(r => r.Quantity),
    t.GetRewardsSummaryByType(RewardType.Card).Count()
    );

            summary.AppendFormat("Packs: {0} in {1} rewards" + Environment.NewLine,
    t.GetRewardsSummaryByType(RewardType.Pack).Sum(r => r.Quantity),
    t.GetRewardsSummaryByType(RewardType.Pack).Count()
    );


            this.textBlock.Text = summary.ToString();
        }

        private void OnTabSelected(object sender, RoutedEventArgs e)
        {
            var tab = sender as TabItem;
            if (tab != null)
            {
                // this tab is selected!
                this.rewardsList.DataContext = DataModel.Tracker.Instance.Rewards
                    .GroupBy(r=> new { r.Date.Date, r.Type })
                    .Select(rs=> new DataModel.Reward() {
                        Type = rs.Key.Type,
                        Date = rs.Key.Date,
                        Quantity = rs.Where(r=> r.Type == rs.Key.Type).Sum(r=> r.Quantity)
                    });
            }
        }
    }
}
