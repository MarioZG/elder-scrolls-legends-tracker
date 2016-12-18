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

        public int TotalGames
        {
            get
            {
                return GamesList.Count();
            }
        }

        public Func<double, string> Formatter { get; set; }
        public Func<double, string> FormatterFirst { get; set; }
        public Func<double, string> FormatterSecond { get; set; }

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

        public int OrderOfPlayFirstVictories
        {
            get
            {
                return GamesList.Where(g => g.OrderOfPlay == OrderOfPlay.First && g.Outcome == GameOutcome.Victory).Count();
            }
        }

        public int OrderOfPlaySecondVictories
        {
            get
            {
                return GamesList.Where(g => g.OrderOfPlay == OrderOfPlay.Second && g.Outcome == GameOutcome.Victory).Count();
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

        private string groupBy = "Deck";

        public string GroupBy
        {
            get { return groupBy; }
            set { groupBy = value; RaiseDataPropertyChange(); }
        }

        private string valueToShow = "Win";

        public string ValueToShow
        {
            get { return valueToShow; }
            set { valueToShow = value; RaiseDataPropertyChange(); }
        }

        public GameStatisticsViewModel() : this(TrackerFactory.DefaultTrackerFactory)
        {

        }

        public GameStatisticsViewModel(ITrackerFactory trackerFactory) : base(trackerFactory)
        {
            this.gameType = GameType.PlayRanked;
            Formatter = x => Math.Round((double)x / TotalGames * 100, 0) + " %";
            FormatterFirst = x => Math.Round((double)x / OrderOfPlayFirst * 100, 0) + " %";
            FormatterSecond = x => Math.Round((double)x / OrderOfPlaySecond * 100, 0) + " %";
        }

        protected override void RaiseDataPropertyChange()
        {
            base.RaiseDataPropertyChange();
            RaisePropertyChangedEvent("Victories");
            RaisePropertyChangedEvent("TotalGames");
            RaisePropertyChangedEvent("OrderOfPlayFirst");
            RaisePropertyChangedEvent("OrderOfPlaySecond");
            RaisePropertyChangedEvent("OrderOfPlayFirstVictories");
            RaisePropertyChangedEvent("OrderOfPlaySecondVictories");
            RaisePropertyChangedEvent("GamesList"); 
        }


        public IEnumerable<DataModel.Game> GamesList
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

        private static object GetPropertyValue(object obj, string propertyName)
        {
            object ret = obj;
            foreach (string prop in propertyName.Split(new char[] { '.'}))
            {
                ret = ret.GetType().GetProperty(prop).GetValue(ret, null);
            }
            return ret;
        }

        public override dynamic GetDataSet()
        {
            var result = GamesList
                .GroupBy(g => new { D = GetPropertyValue(g, GroupBy) , OC = g.OpponentClass })
                        .Select(d => new
                        {
                            Deck = d.Key.D,
                            Opp = ClassAttributesHelper.Classes[d.Key.OC.Value].ToString(),
                            Win = d.Where(d2 => d2.Outcome == GameOutcome.Victory).Count() +
                                "-" + d.Where(d2 => d2.Outcome == GameOutcome.Defeat).Count(),
                            WinPerc =  (double)d.Where( g=> g.Outcome == GameOutcome.Victory).Count()/ d.Count() * 100,
                        });
            //union totoal for deck
            result = result.Union(
                            GamesList
                                .GroupBy(g => new { D = GetPropertyValue(g, GroupBy) })
                                .Select(d => new
                                {
                                    Deck = d.Key.D,
                                    Opp = "Total",
                                    Win = d.Where(d2 => d2.Outcome == GameOutcome.Victory).Count() +
                                        "-" + d.Where(d2 => d2.Outcome == GameOutcome.Defeat).Count(),
                                    WinPerc = (double)d.Where(g => g.Outcome == GameOutcome.Victory).Count() / d.Count() * 100
                                })
                        );

            //add total row for all decks
            object totalDeck = new DataModel.Deck() { Name = "TOTAL", Notes = "SUMMARYROW" };
            var total = GamesList
                                .GroupBy(g => g.OpponentClass)
                                .Select(d => new
                                {
                                    Deck = totalDeck,
                                    Opp = ClassAttributesHelper.Classes[d.Key.Value].ToString(),
                                    Win = d.Where(d2 => d2.Outcome == GameOutcome.Victory).Count() +
                                            "-" + d.Where(d2 => d2.Outcome == GameOutcome.Defeat).Count(),
                                    WinPerc = (double)d.Where(g => g.Outcome == GameOutcome.Victory).Count() / d.Count() * 100
                                });
            result = result.Union(
                          total 
                        )
                        //totoal for toal row
                        .Union(
                            GamesList
                                .Select(d => new
                                {
                                    Deck = totalDeck,
                                    Opp = "Total",
                                    Win = GamesList.Where(d2 => d2.Outcome == GameOutcome.Victory).Count() +
                                        "-" + GamesList.Where(d2 => d2.Outcome == GameOutcome.Defeat).Count(),
                                    WinPerc = (double)GamesList.Where(g => g.Outcome == GameOutcome.Victory).Count() / GamesList.Count() * 100
                                })
                        );

            //add % of opponents decks
            //add total row for all decks
            object totalOpponentDeck = new DataModel.Deck() { Name = "Games vs class %", Notes = "SUMMARYROW" };
            var totalOpponents = GamesList
                                .GroupBy(g => g.OpponentClass)
                                .Select(d => new
                                {
                                    Deck = totalOpponentDeck,
                                    Opp = ClassAttributesHelper.Classes[d.Key.Value].ToString(),
                                    Win = Math.Round((double)d.Count() / GamesList.Count() * 100).ToString() ,
                                    WinPerc = (double)d.Count() / GamesList.Count() * 100,
                                });
            result = result.Union(totalOpponents);

            return result.ToPivotTable(
                            item => item.Opp,
                            item =>item.Deck,
                            items => items.Any() ? 
                                    (valueToShow == "Win" ? items.First().Win : Math.Round(items.First().WinPerc,0).ToString() )
                                 : "");

        }
    }
}
