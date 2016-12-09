using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ESLTracker.Utils;

namespace ESLTracker.ViewModels
{
    public class ArenaStatsViewModel
    {
        private ITrackerFactory trackerFactory;

        public ArenaStatsViewModel() : this(TrackerFactory.DefaultTrackerFactory)
        {

        }

        public ArenaStatsViewModel(ITrackerFactory trackerFactory)
        {
            this.trackerFactory = trackerFactory;
        }

        public dynamic GetArenaRunStatistics()
        {
            var groupby = typeof(DataModel.Deck).GetProperty("Class");

            return trackerFactory.GetTracker().Decks
            .Where(d => d.Type == DataModel.Enums.DeckType.VersusArena)
            .GroupBy(d => groupby.GetValue(d))
            .Select(ds => new
            {
                Class = ds.Key,
                NumberOfRuns = ds.Count(),
                AvgWins = ds.Average(d => d.Victories),
                Best = ds.Max(d => d.Victories),
                Total = new ESLTracker.RewardsTotal(ds.SelectMany(d => d.GetArenaRewards()).GroupBy(r => r.Type).Select(rg => new ESLTracker.DataModel.Reward { Type = rg.Key, Quantity = rg.Sum(r => r.Quantity) }).ToList()),

                Avg = new ESLTracker.RewardsTotal(
                        ds.SelectMany(d => d.GetArenaRewards().GroupBy(r => new { r.Type, r.ArenaDeckId }).Select(rg => new { rg.Key, Qty = rg.Sum(r => r.Quantity) }))
                        .GroupBy(r => new { r.Key.Type })
                        .Select(rg => new ESLTracker.DataModel.Reward() { Type = rg.Key.Type, Quantity = (int)rg.Average(r => r.Qty) })
                        ),
                AvgOld = new ESLTracker.RewardsTotal(ds.SelectMany(d => d.GetArenaRewards()).GroupBy(r => r.Type).Select(rg => new ESLTracker.DataModel.Reward { Type = rg.Key, Quantity = (int)rg.Average(r => r.Quantity) }).ToList()),
                Max = new ESLTracker.RewardsTotal(
                        ds.SelectMany(d => d.GetArenaRewards().GroupBy(r => new { r.Type, r.ArenaDeckId }).Select(rg => new { rg.Key, Qty = rg.Sum(r => r.Quantity) }))
                        .GroupBy(r => new { r.Key.Type })
                        .Select(rg => new ESLTracker.DataModel.Reward() { Type = rg.Key.Type, Quantity = (int)rg.Max(r => r.Qty) })
                        ),
                TotalGold = ds.Sum(d => d.GetArenaRewards().Where(r => r.Type == ESLTracker.DataModel.Enums.RewardType.Gold).Sum(r => r.Quantity)),
                TotalGems = ds.Sum(d => d.GetArenaRewards().Where(r => r.Type == ESLTracker.DataModel.Enums.RewardType.SoulGem).Sum(r => r.Quantity)),
                TotalPacks = ds.Sum(d => d.GetArenaRewards().Where(r => r.Type == ESLTracker.DataModel.Enums.RewardType.Pack).Sum(r => r.Quantity)),
                TotalCards = ds.Sum(d => d.GetArenaRewards().Where(r => r.Type == ESLTracker.DataModel.Enums.RewardType.Card).Sum(r => r.Quantity))
            }).ToList();
        }
    }
}
