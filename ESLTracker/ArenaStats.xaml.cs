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

namespace ESLTracker
{
    /// <summary>
    /// Interaction logic for ArenaStats.xaml
    /// </summary>
    public partial class ArenaStats : Window
    {
        public ArenaStats()
        {
            InitializeComponent();

            DataModel.ITracker tracker = new ESLTracker.Utils.TrackerFactory().GetTracker();

            var groupby = typeof(DataModel.Deck).GetProperty("Class");

            Func<Func<DataModel.Deck, int>, int> calc;

            //this.dataGrid.Data

            this.dataGrid.ItemsSource =
                new ESLTracker.Utils.TrackerFactory().GetTracker().Decks
                .Where(d => d.Type == DataModel.Enums.DeckType.VersusArena)
                .GroupBy(d => groupby.GetValue(d))
                .Select(ds => new
                {
                    Class = ds.Key,
                    NumberOfRuns = ds.Count(),
                    AvgWins = ds.Average(d => d.Victories),
                    Best = ds.Max(d => d.Victories),
                    Total = new ESLTracker.RewardsTotal(ds.SelectMany(d => d.GetArenaRewards()).GroupBy(r => r.Type).Select(rg => new ESLTracker.DataModel.Reward { Type = rg.Key, Quantity = rg.Sum(r => r.Quantity) }).ToList()),

                    AvgCorrectForMorethanOneSameTypeinonerune = new ESLTracker.RewardsTotal(
        ds.SelectMany(d => d.GetArenaRewards().GroupBy(r => new { r.Type, r.ArenaDeckId }).Select(rg => new { rg.Key, Qty = rg.Sum(r => r.Quantity) }))
        .GroupBy(r => new { r.Key.Type })
        .Select(rg => new ESLTracker.DataModel.Reward() { Type = rg.Key.Type, Quantity = (int)rg.Average(r => r.Qty) })
                            ),
                    Avg = new ESLTracker.RewardsTotal(ds.SelectMany(d => d.GetArenaRewards()).GroupBy(r => r.Type).Select(rg => new ESLTracker.DataModel.Reward { Type = rg.Key, Quantity = (int)rg.Average(r => r.Quantity) }).ToList()),
                    Max = new ESLTracker.RewardsTotal(ds.SelectMany(d => d.GetArenaRewards()).GroupBy(r => r.Type).Select(rg => new ESLTracker.DataModel.Reward { Type = rg.Key, Quantity = rg.Max(r => r.Quantity) }).ToList()),
                    //Avg = ds.SelectMany(d => d.GetArenaRewards()).GroupBy(r => r.Type).Select(rg => new { rg.Key, Qty = rg.Average(r => r.Quantity) }),
                    //Max = ds.SelectMany(d => d.GetArenaRewards()).GroupBy(r => r.Type).Select(rg => new { rg.Key, Qty = rg.Max(r => r.Quantity) }),
                    TotalGold = ds.Sum(d => d.GetArenaRewards().Where(r => r.Type == ESLTracker.DataModel.Enums.RewardType.Gold).Sum(r => r.Quantity)),
                    TotalGems = ds.Sum(d => d.GetArenaRewards().Where(r => r.Type == ESLTracker.DataModel.Enums.RewardType.SoulGem).Sum(r => r.Quantity)),
                    TotalPacks = ds.Sum(d => d.GetArenaRewards().Where(r => r.Type == ESLTracker.DataModel.Enums.RewardType.Pack).Sum(r => r.Quantity)),
                    TotalCards = ds.Sum(d => d.GetArenaRewards().Where(r => r.Type == ESLTracker.DataModel.Enums.RewardType.Card).Sum(r => r.Quantity))
                 });

//            this.dataGrid.bind
  //          this.dataGrid.Columns[0].Header = groupby.Name;
        }
    }

    public class RewardsTotal
    {
        public int Gold { get; set; }
        public int SoulGem { get; set; }
        public int Pack { get; set; }
        public int Card { get; set; }

        public RewardsTotal(IEnumerable<DataModel.Reward> rewards)
        {
            foreach(DataModel.Reward r in rewards)
            {
                switch (r.Type)
                {
                    case DataModel.Enums.RewardType.Gold:
                        Gold += r.Quantity;
                        break;
                    case DataModel.Enums.RewardType.SoulGem:
                        SoulGem += r.Quantity;
                        break;
                    case DataModel.Enums.RewardType.Pack:
                        Pack += r.Quantity;
                        break;
                    case DataModel.Enums.RewardType.Card:
                        Card += r.Quantity;
                        break;
                    default:
                        break;
                }
            }
        }

        public override string ToString()
        {
            return string.Format("{0,5} / {1,5} / {2,3} / {3,3}", Gold, SoulGem, Pack, Card);
        }
    }
}
