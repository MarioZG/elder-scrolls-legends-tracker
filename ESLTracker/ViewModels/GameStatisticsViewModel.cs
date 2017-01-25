using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;
using ESLTracker.DataModel;
using ESLTracker.DataModel.Enums;
using ESLTracker.Utils;
using LiveCharts;
using LiveCharts.Defaults;
using LiveCharts.Wpf;

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

        private ChartValues<HeatPoint> opponentClassHeatMap;
        public ChartValues<HeatPoint> OpponentClassHeatMap
        {
            get { return opponentClassHeatMap; }
        }

        //formatters for gauge
        public Func<double, string> Formatter { get; set; }
        public Func<double, string> FormatterFirst { get; set; }
        public Func<double, string> FormatterSecond { get; set; }
        public Func<ChartPoint, string> HeatLabelPoint { get; set; }
       

        public GameStatisticsViewModel() : this(TrackerFactory.DefaultTrackerFactory)
        {

        }

        public GameStatisticsViewModel(ITrackerFactory trackerFactory) : base(trackerFactory)
        {
            this.gameType = GameType.PlayRanked;
            Formatter = x => TotalGames > 0 ? Math.Round((double)x / TotalGames * 100, 0) + " %" : "- %";
            FormatterFirst = x => OrderOfPlayFirst > 0 ? Math.Round((double)x / OrderOfPlayFirst * 100, 0) + " %" : "- %";
            FormatterSecond = x => OrderOfPlaySecond > 0 ? Math.Round((double)x / OrderOfPlaySecond * 100, 0) + " %" : "- %";
            HeatLabelPoint = (x => ClassAttributesHelper.FindClassByAttribute(
                                      new DeckAttribute[]{ (DeckAttribute)x.X, (DeckAttribute)x.Y
                                    }).First() + " : " + x.Weight + " % of games");
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
            RaisePropertyChangedEvent("OpponentClassHeatMap"); 
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

        public SerializableVersion TOTAL_ROW_VERSION { get; } = new SerializableVersion(0, 0);

        public override dynamic GetDataSet()
        {
            
            //breakdwon by deck
            var result = GamesList
                .GroupBy(g => new { D = GetPropertyValue(g, GroupBy), VS = g.DeckVersion.Version, OC = g.OpponentClass })
                        .Select(d => new
                        {
                            Deck = d.Key.D,
                            DeckVersion = d.Key.VS,
                            Opp = ClassAttributesHelper.Classes[d.Key.OC.Value].ToString(),
                            Win = d.Where(d2 => d2.Outcome == GameOutcome.Victory).Count() +
                                "-" + d.Where(d2 => d2.Outcome == GameOutcome.Defeat).Count(),
                            WinPerc =  (double)d.Where( g=> g.Outcome == GameOutcome.Victory).Count()/ d.Count() * 100,
                        });
            //add totoal for deck
            result = result.Union(
                GamesList.GroupBy(g => new { D = GetPropertyValue(g, GroupBy), VS = TOTAL_ROW_VERSION, OC = g.OpponentClass })
                        .Select(d => new
                        {
                            Deck = d.Key.D,
                            DeckVersion = d.Key.VS,
                            Opp = ClassAttributesHelper.Classes[d.Key.OC.Value].ToString(),
                            Win = d.Where(d2 => d2.Outcome == GameOutcome.Victory).Count() +
                                "-" + d.Where(d2 => d2.Outcome == GameOutcome.Defeat).Count(),
                            WinPerc = (double)d.Where(g => g.Outcome == GameOutcome.Victory).Count() / d.Count() * 100,
                        })
                        );
            //union totoal for all deck versoons
            result = result.Union(
                            GamesList
                                .GroupBy(g => new { D = GetPropertyValue(g, GroupBy), VS = TOTAL_ROW_VERSION })
                                .Select(d => new
                                {
                                    Deck = d.Key.D,
                                    DeckVersion = d.Key.VS,
                                    Opp = "Total",
                                    Win = d.Where(d2 => d2.Outcome == GameOutcome.Victory).Count() +
                                        "-" + d.Where(d2 => d2.Outcome == GameOutcome.Defeat).Count(),
                                    WinPerc = (double)d.Where(g => g.Outcome == GameOutcome.Victory).Count() / d.Count() * 100
                                })
                        );

            //union totoal for each deck version
            result = result.Union(
                            GamesList
                                .GroupBy(g => new { D = GetPropertyValue(g, GroupBy), VS = g.DeckVersion.Version })
                                .Select(d => new
                                {
                                    Deck = d.Key.D,
                                    DeckVersion = d.Key.VS,
                                    Opp = "Total",
                                    Win = d.Where(d2 => d2.Outcome == GameOutcome.Victory).Count() +
                                        "-" + d.Where(d2 => d2.Outcome == GameOutcome.Defeat).Count(),
                                    WinPerc = (double)d.Where(g => g.Outcome == GameOutcome.Victory).Count() / d.Count() * 100
                                })
                        );

            #region first-second ration and wins for deck versions
            //add % of gaames you went first
            result = result.Union(
                            GamesList
                                .GroupBy(g => new { D = GetPropertyValue(g, GroupBy), VS = g.DeckVersion.Version })
                                .Select(d => new
                                {
                                    Deck = d.Key.D,
                                    DeckVersion = d.Key.VS,
                                    Opp = "First-Second",
                                    Win = d.Where(d2 => d2.OrderOfPlay == OrderOfPlay.First).Count() +
                                        "-" + d.Where(d2 => d2.OrderOfPlay == OrderOfPlay.Second).Count(),
                                    WinPerc = (double)d.Where(g => g.OrderOfPlay == OrderOfPlay.First).Count() / d.Count() * 100
                                })
                        );

            //add % of wins  you went first
            result = result.Union(
                            GamesList
                                .GroupBy(g => new { D = GetPropertyValue(g, GroupBy), VS = g.DeckVersion.Version })
                                .Select(d => new
                                {
                                    Deck = d.Key.D,
                                    DeckVersion = d.Key.VS,
                                    Opp = "FirstWin",
                                    Win = d.Where(d2 => d2.Outcome == GameOutcome.Victory && d2.OrderOfPlay == OrderOfPlay.First).Count() +
                                        "-" + d.Where(d2 => d2.Outcome == GameOutcome.Defeat && d2.OrderOfPlay == OrderOfPlay.First).Count(),
                                    WinPerc = (double)d.Where(g => g.Outcome == GameOutcome.Victory && g.OrderOfPlay == OrderOfPlay.First).Count() / d.Where(g => g.OrderOfPlay == OrderOfPlay.First).Count() * 100
                                })
                        );
            //add % of wins you went second
            result = result.Union(
                            GamesList
                                .GroupBy(g => new { D = GetPropertyValue(g, GroupBy), VS = g.DeckVersion.Version })
                                .Select(d => new
                                {
                                    Deck = d.Key.D,
                                    DeckVersion = d.Key.VS,
                                    Opp = "SecondWin",
                                    Win = d.Where(d2 => d2.Outcome == GameOutcome.Victory && d2.OrderOfPlay == OrderOfPlay.Second).Count() +
                                        "-" + d.Where(d2 => d2.Outcome == GameOutcome.Defeat && d2.OrderOfPlay == OrderOfPlay.Second).Count(),
                                    WinPerc = (double)d.Where(g => g.Outcome == GameOutcome.Victory && g.OrderOfPlay == OrderOfPlay.Second).Count() / d.Where( g=> g.OrderOfPlay == OrderOfPlay.Second).Count() * 100
                                })
                        );
            #endregion

            #region first-second ration and wins for deck total
            //add % of gaames you went first
            result = result.Union(
                            GamesList
                                .GroupBy(g => new { D = GetPropertyValue(g, GroupBy), VS = TOTAL_ROW_VERSION })
                                .Select(d => new
                                {
                                    Deck = d.Key.D,
                                    DeckVersion = d.Key.VS,
                                    Opp = "First-Second",
                                    Win = d.Where(d2 => d2.OrderOfPlay == OrderOfPlay.First).Count() +
                                        "-" + d.Where(d2 => d2.OrderOfPlay == OrderOfPlay.Second).Count(),
                                    WinPerc = (double)d.Where(g => g.OrderOfPlay == OrderOfPlay.First).Count() / d.Count() * 100
                                })
                        );

            //add % of wins  you went first
            result = result.Union(
                            GamesList
                                .GroupBy(g => new { D = GetPropertyValue(g, GroupBy), VS = TOTAL_ROW_VERSION })
                                .Select(d => new
                                {
                                    Deck = d.Key.D,
                                    DeckVersion = d.Key.VS,
                                    Opp = "FirstWin",
                                    Win = d.Where(d2 => d2.Outcome == GameOutcome.Victory && d2.OrderOfPlay == OrderOfPlay.First).Count() +
                                        "-" + d.Where(d2 => d2.Outcome == GameOutcome.Defeat && d2.OrderOfPlay == OrderOfPlay.First).Count(),
                                    WinPerc = (double)d.Where(g => g.Outcome == GameOutcome.Victory && g.OrderOfPlay == OrderOfPlay.First).Count() / d.Where(g => g.OrderOfPlay == OrderOfPlay.First).Count() * 100
                                })
                        );
            //add % of wins you went second
            result = result.Union(
                            GamesList
                                .GroupBy(g => new { D = GetPropertyValue(g, GroupBy), VS = TOTAL_ROW_VERSION })
                                .Select(d => new
                                {
                                    Deck = d.Key.D,
                                    DeckVersion = d.Key.VS,
                                    Opp = "SecondWin",
                                    Win = d.Where(d2 => d2.Outcome == GameOutcome.Victory && d2.OrderOfPlay == OrderOfPlay.Second).Count() +
                                        "-" + d.Where(d2 => d2.Outcome == GameOutcome.Defeat && d2.OrderOfPlay == OrderOfPlay.Second).Count(),
                                    WinPerc = (double)d.Where(g => g.Outcome == GameOutcome.Victory && g.OrderOfPlay == OrderOfPlay.Second).Count() / d.Where(g => g.OrderOfPlay == OrderOfPlay.Second).Count() * 100
                                })
                        );
            #endregion

            result = result.OrderBy(r => r.Deck).ThenBy(r => r.DeckVersion);

            //add total row for all decks
            object totalDeck = Deck.CreateNewDeck(trackerFactory, "TOTAL");
            var total = GamesList
                                .GroupBy(g => g.OpponentClass)
                                .Select(d => new
                                {
                                    Deck = totalDeck,
                                    DeckVersion = TOTAL_ROW_VERSION,
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
                                    DeckVersion = TOTAL_ROW_VERSION,
                                    Opp = "Total",
                                    Win = GamesList.Where(d2 => d2.Outcome == GameOutcome.Victory).Count() +
                                        "-" + GamesList.Where(d2 => d2.Outcome == GameOutcome.Defeat).Count(),
                                    WinPerc = (double)GamesList.Where(g => g.Outcome == GameOutcome.Victory).Count() / GamesList.Count() * 100
                                })
                        );

            //add % of opponents decks
            //add total row for all decks
            object totalOpponentDeck = Deck.CreateNewDeck(trackerFactory, "Opponent class %");
            var totalOpponents = GamesList
                                .GroupBy(g => g.OpponentClass)
                                .Select(d => new
                                {
                                    Deck = totalOpponentDeck,
                                    DeckVersion = TOTAL_ROW_VERSION,
                                    Opp = ClassAttributesHelper.Classes[d.Key.Value].ToString(),
                                    Win = Math.Round((double)d.Count() / GamesList.Count() * 100).ToString() ,
                                    WinPerc = (double)d.Count() / GamesList.Count() * 100,
                                });
            result = result.Union(totalOpponents);

            opponentClassHeatMap = new ChartValues<HeatPoint>();

            foreach (var r in totalOpponents)
            {
                DeckClass dc = (DeckClass)Enum.Parse(typeof(DeckClass), r.Opp);
                DataModel.DeckAttributes da = ClassAttributesHelper.Classes[dc];
                DeckAttribute da1 = da[0];
                DeckAttribute da2 = (da.Count > 1 ? da[1] : da[0]);
                opponentClassHeatMap.Add(new HeatPoint((int)da1, (int)da2, Int32.Parse(r.Win)));
            }

            //new ListCollectionView

            return result.ToPivotTable2(
                            item => item.Opp,
                            item => new { item.Deck, item.DeckVersion },
                            items => items.Any() ? 
                                    (valueToShow == "Win" ? 
                                            items.First().Win : 
                                            (! double.IsNaN(items.First().WinPerc) ? Math.Round(items.First().WinPerc,0).ToString() : "")
                                    )
                                 : "");

        }
    }
}
