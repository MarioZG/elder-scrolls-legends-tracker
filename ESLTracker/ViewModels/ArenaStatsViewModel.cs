using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ESLTracker.DataModel.Enums;
using ESLTracker.Utils;

namespace ESLTracker.ViewModels
{
    public class ArenaStatsViewModel : Game.GameFilterViewModel
    {
        public override IEnumerable<GameType> GameTypeSeletorValues
        {
            get
            {
                return new List<GameType>()
                {
                    GameType.VersusArena,
                    GameType.SoloArena
                };
            }
        }


        public ArenaStatsViewModel() : this(TrackerFactory.DefaultTrackerFactory)
        {

        }

        public ArenaStatsViewModel(ITrackerFactory trackerFactory) : base(trackerFactory)
        {
            this.gameType = GameType.VersusArena;
        }

        public override dynamic GetDataSet()
        {
            var groupby = typeof(DataModel.Deck).GetProperty("Class");

            var result = trackerFactory.GetTracker().Decks
            .Where(d => d.GetDeckGames().Any( g => g.Type == GameType)
                     && ((filterDateFrom == null) || (d.CreatedDate.Date >= filterDateFrom.Value.Date))
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
                Max = new RewardsTotal(
                        ds.SelectMany(d => d.GetArenaRewards().GroupBy(r => new { r.Type, r.ArenaDeckId }).Select(rg => new { rg.Key, Qty = rg.Sum(r => r.Quantity) }))
                        .GroupBy(r => new { r.Key.Type })
                        .Select(rg => new ESLTracker.DataModel.Reward() { Type = rg.Key.Type, Quantity = (int)rg.Max(r => r.Qty) })
                        )//,
                //TotalGold = ds.Sum(d => d.GetArenaRewards().Where(r => r.Type == ESLTracker.DataModel.Enums.RewardType.Gold).Sum(r => r.Quantity)),
                //TotalGems = ds.Sum(d => d.GetArenaRewards().Where(r => r.Type == ESLTracker.DataModel.Enums.RewardType.SoulGem).Sum(r => r.Quantity)),
                //TotalPacks = ds.Sum(d => d.GetArenaRewards().Where(r => r.Type == ESLTracker.DataModel.Enums.RewardType.Pack).Sum(r => r.Quantity)),
                //TotalCards = ds.Sum(d => d.GetArenaRewards().Where(r => r.Type == ESLTracker.DataModel.Enums.RewardType.Card).Sum(r => r.Quantity))
            }).ToList();

            //mark best averages
            //result.Where(r => Math.Abs(r.AvgWins - result.Max(rm => rm.AvgWins)) < 0.01).All(r => { r.MarkAvgWins = true; return true; });
            result.Where(r => Math.Abs(r.Avg.Gold - result.Max(rm => rm.Avg.Gold)) < 0.01).All(r => { r.Avg.MarkGold = true; return true; });
            result.Where(r => Math.Abs(r.Avg.SoulGem - result.Max(rm => rm.Avg.SoulGem)) < 0.01).All(r => { r.Avg.MarkSoulGem = true; return true; });
            result.Where(r => Math.Abs(r.Avg.Pack - result.Max(rm => rm.Avg.Pack)) < 0.01).All(r => { r.Avg.MarkPack = true; return true; });
            result.Where(r => Math.Abs(r.Avg.Card - result.Max(rm => rm.Avg.Card)) < 0.01).All(r => { r.Avg.MarkCard = true; return true; });

            result.Add(new
            {
                Class = "TOTAL" as object,
                NumberOfRuns = result.Sum(r => r.NumberOfRuns),
                AvgWins = result.Average(d => d.AvgWins),
                Best = result.Max(d => d.Best),
                Total = result.Aggregate(new RewardsTotal { },
                    (accumulator, it) =>
                        new RewardsTotal
                        {
                            Card = accumulator.Card + it.Total.Card,
                            Pack = accumulator.Pack + it.Total.Pack,
                            Gold = accumulator.Gold + it.Total.Gold,
                            SoulGem = accumulator.SoulGem + it.Total.SoulGem
                        }
                ),
                Avg = new RewardsTotal
                {
                    Card = result.Average(r => r.Avg.Card),
                    Pack = result.Average(r => r.Avg.Pack),
                    Gold = result.Average(r => r.Avg.Gold),
                    SoulGem = result.Average(r => r.Avg.SoulGem)
                },
                Max = new RewardsTotal
                {
                    Card = result.Max(r => r.Max.Card),
                    Pack = result.Max(r => r.Max.Pack),
                    Gold = result.Max(r => r.Max.Gold),
                    SoulGem = result.Max(r => r.Max.SoulGem)
                }
            });

            return result;
        }
    }

    public class RewardsTotal
    {
        public double Gold { get; set; }
        public double SoulGem { get; set; }
        public double Pack { get; set; }
        public double Card { get; set; }

        public bool MarkGold { get; set; }
        public bool MarkSoulGem { get; set; }
        public bool MarkPack { get; set; }
        public bool MarkCard { get; set; }

        public RewardsTotal()
        {

        }

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
}
