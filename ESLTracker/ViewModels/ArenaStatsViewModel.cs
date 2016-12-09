using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ESLTracker.DataModel.Enums;
using ESLTracker.Utils;

namespace ESLTracker.ViewModels
{
    public class ArenaStatsViewModel : ViewModelBase
    {
        private DateTime? filterDateFrom;
        public DateTime? FilterDateFrom
        {
            get { return filterDateFrom; }
            set { filterDateFrom = value; RaisePropertyChangedEvent("DisplayDataSource"); }
        }

        private DateTime? filterTo;
        public DateTime? FilterDateTo
        {
            get { return filterTo; }
            set { filterTo = value; RaisePropertyChangedEvent("DisplayDataSource"); }
        }

        private DeckType deckType;
        public DeckType DeckType
        {
            get { return deckType; }
            set { deckType = value; RaisePropertyChangedEvent("DisplayDataSource"); }
        }

        public dynamic DisplayDataSource
        {
            get { return GetArenaRunStatistics(); }
        }

        public IEnumerable<DeckType> ArenaTypeSeletorValues
        {
            get
            {
                return new List<DeckType>()
                {
                    DeckType.VersusArena,
                    DeckType.SoloArena
                };
            }
        }


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
            .Where(d => d.Type == DeckType 
                     && ((filterDateFrom== null) || (d.CreatedDate.Date >= filterDateFrom.Value.Date))
                     && ((FilterDateTo == null) || (d.CreatedDate.Date <= FilterDateTo.Value.Date)))
            .GroupBy(d => groupby.GetValue(d))
            .Select(ds => new
            {
                Class = ds.Key,
                NumberOfRuns = ds.Count(),
                AvgWins = ds.Average(d => d.Victories),
                Best = ds.Max(d => d.Victories),
                Total = new RewardsTotal(ds.SelectMany(d => d.GetArenaRewards()).GroupBy(r => r.Type).Select(rg => new ESLTracker.DataModel.Reward { Type = rg.Key, Quantity = rg.Sum(r => r.Quantity) }).ToList()),

                Avg = new RewardsTotal(
                        ds.SelectMany(d => d.GetArenaRewards().GroupBy(r => new { r.Type, r.ArenaDeckId }).Select(rg => new { rg.Key, Qty = rg.Sum(r => r.Quantity) }))
                        .GroupBy(r => new { r.Key.Type })
                        .Select(rg => new ESLTracker.DataModel.Reward() { Type = rg.Key.Type, Quantity = (int)rg.Average(r => r.Qty) })
                        ),
                AvgOld = new RewardsTotal(ds.SelectMany(d => d.GetArenaRewards()).GroupBy(r => r.Type).Select(rg => new ESLTracker.DataModel.Reward { Type = rg.Key, Quantity = (int)rg.Average(r => r.Quantity) }).ToList()),
                Max = new RewardsTotal(
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

    public class RewardsTotal
    {
        public int Gold { get; set; }
        public int SoulGem { get; set; }
        public int Pack { get; set; }
        public int Card { get; set; }

        public RewardsTotal(IEnumerable<DataModel.Reward> rewards)
        {
            foreach (DataModel.Reward r in rewards)
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

    public enum DateFilter
    {
        All,
        ThisWeek,
        ThisMonth,
        PreviousMonth
    }
}
