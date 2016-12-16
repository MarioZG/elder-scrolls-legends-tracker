using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ESLTracker.DataModel.Enums;
using ESLTracker.Utils;

namespace ESLTracker.ViewModels
{
    public class GameStatisticsViewModel : Game.GameFilterViewModel
    {

        public int Victories
        {
            get
            {
                return GamesList.Where(g => g.Outcome == GameOutcome.Victory).Count();
            }
        }

        public int Defeats
        {
            get
            {
                return GamesList.Where(g => g.Outcome == GameOutcome.Defeat).Count();
            }
        }

        public int OrderOfPlayFirst
        {
            get
            {
                return GamesList.Where(g => g.OrderOfPlay == OrderOfPlay.First).Count();
            }
        }

        public int OrderOfPlaySecond
        {
            get
            {
                return GamesList.Where(g => g.OrderOfPlay == OrderOfPlay.Second).Count();
            }
        }

        public override IEnumerable<GameType> GameTypeSeletorValues
        {
            get
            {
                return new List<GameType>()
                {
                    GameType.PlayRanked,
                    GameType.PlayCasual
                };
            }
        }

        public GameStatisticsViewModel() : this(TrackerFactory.DefaultTrackerFactory)
        {

        }

        public GameStatisticsViewModel(ITrackerFactory trackerFactory) : base(trackerFactory)
        {
            this.gameType = GameType.PlayRanked;
        }

        protected override void RaiseDataPropertyChange()
        {
            base.RaiseDataPropertyChange();
            RaisePropertyChangedEvent("Victories");
            RaisePropertyChangedEvent("Defeats");
            RaisePropertyChangedEvent("OrderOfPlayFirst");
            RaisePropertyChangedEvent("OrderOfPlaySecond");
            RaisePropertyChangedEvent("GamesList"); 
        }


        private IEnumerable<DataModel.Game> GamesList
        {
            get
            {
                return trackerFactory.GetTracker()
                    .Games
                    .Where(g => (g.OpponentClass.HasValue)
                        && (g.Type == this.gameType)
                        && ((filterDateFrom == null) || (g.Date.Date >= filterDateFrom.Value.Date))
                        && ((FilterDateTo == null) || (g.Date.Date <= FilterDateTo.Value.Date))
                        );
            }
        }

        public override dynamic GetDataSet()
        {



            var result = GamesList
                .GroupBy(g => new { D = g.Deck, OC = g.OpponentClass })
                        .Select(d => new
                        {
                            Deck = d.Key.D,
                            Opp = ClassAttributesHelper.Classes[d.Key.OC.Value].ToString(),
                            Win = d.Where(d2 => d2.Outcome == GameOutcome.Victory).Count() +
                                "-" + d.Where(d2 => d2.Outcome == GameOutcome.Defeat).Count(),
                            //WinPerc = d.Key.D.WinRatio,
                            //WinAll = d.Key.D.Victories +//.Where(d2 => d2.Outcome == GameOutcome.Victory).Count()+
                            //     "-" + d.Key.D.Defeats,
                        });
            //union totoal for deck
            result = result.Union(
                            GamesList
                                .GroupBy(g => new { D = g.Deck })
                                .Select(d => new
                                {
                                    Deck = d.Key.D,
                                    Opp = "Total",
                                    Win = d.Where(d2 => d2.Outcome == GameOutcome.Victory).Count() +
                                        "-" + d.Where(d2 => d2.Outcome == GameOutcome.Defeat).Count(),

                                })
                        );

            //add total row for all decks
            DataModel.Deck totalDeck = new DataModel.Deck() { Name = "TOTAL" };
            var total = GamesList
                                .GroupBy(g => g.OpponentClass)
                                .Select(d => new
                                {
                                    Deck = totalDeck,
                                    Opp = ClassAttributesHelper.Classes[d.Key.Value].ToString(),
                                    Win = d.Where(d2 => d2.Outcome == GameOutcome.Victory).Count() +
                                            "-" + d.Where(d2 => d2.Outcome == GameOutcome.Defeat).Count(),
                                });
            result = result.Union(
                          total 
                        )
                        ;

            return result.ToPivotTable(
                            item => item.Opp,
                            item =>item.Deck,
                            items => items.Any() ? items.First().Win : "");

        }
    }
}
