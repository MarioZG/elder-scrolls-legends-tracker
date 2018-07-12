using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;
using ESLTracker.Utils;
using ESLTracker.DataModel;
using ESLTracker.DataModel.Enums;
using ESLTracker.Properties;
using LiveCharts;
using LiveCharts.Defaults;
using LiveCharts.Wpf;
using NLog;
using ESLTracker.BusinessLogic.Decks;

namespace ESLTracker.ViewModels.Windows
{
    public class GameStatisticsViewModel : Game.GameFilterViewModel
    {
        private static Logger Logger = LogManager.GetCurrentClassLogger();

        public int Victories
        {
            get
            {
                Logger.ConditionalTrace("Get victories");
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

        private string gropuByOpponent = "class";

        public string GroupByOpponent
        {
            get { return gropuByOpponent; }
            set { SetProperty(ref gropuByOpponent, value, String.Empty, UpdateHeaders); }
        }

        private void UpdateHeaders()
        {

        }

        private string valueToShow = "Win";

        public string ValueToShow
        {
            get { return valueToShow; }
            set { valueToShow = value; RaiseDataPropertyChange(); }
        }

        private bool includeHiddenDecks;

        public bool IncludeHiddenDecks
        {
            get { return includeHiddenDecks; }
            set { includeHiddenDecks = value; RaiseDataPropertyChange(); }
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

        IMessenger messenger;
        ITracker tracker;
        IDeckService deckService;

        public GameStatisticsViewModel(
            ISettings settings, 
            IDateTimeProvider dateTimeProvider,
            ITracker tracker,
            IMessenger messenger,
            IDeckService deckService) : base(settings, dateTimeProvider)
        {

            this.messenger = messenger;
            this.tracker = tracker;
            this.deckService = deckService;

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
                return tracker
                    .Games
                    .Where(g => (g.OpponentClass.HasValue)
                        && (g.Type == this.gameType)
                        && ((filterDateFrom == null) || (g.Date.Date >= filterDateFrom.Value.Date))
                        && ((FilterDateTo == null) || (g.Date.Date <= FilterDateTo.Date))
                        && ((includeHiddenDecks) || (! g.Deck.IsHidden))
                        );
            }
        }

        public SerializableVersion TOTAL_ROW_VERSION { get; } = new SerializableVersion(0, 0);

        public override dynamic GetDataSet()
        {
            var task = new NotifyTaskCompletion<dynamic>(new Task<dynamic>(GetDataSetExecute));
            task.Task.Start();
            return task;
        }
        private dynamic GetDataSetExecute()
        {
            Logger.ConditionalTrace("GetDataSet START");

            Logger.ConditionalTrace("breakdawn  by deck");
            var result = GetBreakDownByDeck(
                            (g) => GetPropertyValue(g, GroupBy),
                            (g) => g.DeckVersion.Version,
                            (g) => GetOpponentGroupMethod(g));

            Logger.ConditionalTrace("add totoal for deck");
            result = result.Union(
                        GetBreakDownByDeck(
                            (g) => GetPropertyValue(g, GroupBy),
                            (g) => TOTAL_ROW_VERSION,
                           (g) => GetOpponentGroupMethod(g)));

            Logger.ConditionalTrace("union totoal for all deck versoons");
            result = result.Union(
                         GetBreakDownByDeck(
                            (g) => GetPropertyValue(g, GroupBy),
                            (g) => TOTAL_ROW_VERSION,
                            (g) => "Total"));

            Logger.ConditionalTrace("union totoal for each deck version");
            result = result.Union(
                         GetBreakDownByDeck(
                            (g) => GetPropertyValue(g, GroupBy),
                            (g) => g.DeckVersion.Version,
                            (g) => "Total"));

            result = FirstSecondStats(
                         result,
                         (g) => g.DeckVersion.Version);

            Logger.ConditionalTrace("add % of games you went first");
            result = FirstSecondStats(
                         result,
                         (g) => TOTAL_ROW_VERSION);

            result = result.OrderBy(r => r.Deck).ThenBy(r => r.DeckVersion);

            Logger.ConditionalTrace("add total row for all decks");
            object totalDeck = deckService.CreateNewDeck("TOTAL");

            result = result.Union(
                         GetBreakDownByDeck(
                            (g) => totalDeck,
                            (g) => TOTAL_ROW_VERSION,
                            (g) => GetOpponentGroupMethod(g)));

            Logger.ConditionalTrace("totoal for toal row");
            result = result.Union(
                        GetBreakDownByDeck(
                            (g) => totalDeck,
                            (g) => TOTAL_ROW_VERSION,
                            (g) => "Total"));

            //add % of opponents decks
            Logger.ConditionalTrace("add total row for all decks");
            object totalOpponentDeck = deckService.CreateNewDeck("Opponent class %");

            var totalOpponents = GetBreakDownByOpponentClass(
                                    (g) => totalOpponentDeck,
                                    (g) => TOTAL_ROW_VERSION,
                                    (g) => GetOpponentGroupMethod(g));

            result = result.Union(totalOpponents);

            Logger.ConditionalTrace("create heat map data");
            CreateOpponentHeatMapData(totalOpponents);

            Logger.ConditionalTrace("select tags");
            var tags = result.Select(r => r.Opp).Where(s =>
                       s != "Total" && s != "First_Second"
                       && s != "FirstWin" && s != "SecondWin"
                    ).Distinct();

            messenger.Send(new Utils.Messages.GameStatsOpponentGroupByChanged()
            {
                OpponentGroupBy = gropuByOpponent,
                Tags = tags
            });

            Logger.ConditionalTrace("final group by results");
            var returnList = result.GroupBy(r => new { r.Deck, r.DeckVersion })
                .Select(r =>
                {
                    var ret = new GameStatisticsDeckRow
                    {
                        Deck = r.Key.Deck,
                        DeckVersion = r.Key.DeckVersion,
                        Neutral = GetResultToShow(r, "Neutral"),
                        Strength = GetResultToShow(r, "Strength"),
                        Inteligence = GetResultToShow(r, "Inteligence"),
                        Willpower = GetResultToShow(r, "Willpower"),
                        Agility = GetResultToShow(r, "Agility"),
                        Endurance = GetResultToShow(r, "Endurance"),
                        Archer = GetResultToShow(r, "Archer"),
                        Assassin = GetResultToShow(r, "Assassin"),
                        Battlemage = GetResultToShow(r, "Battlemage"),
                        Crusader = GetResultToShow(r, "Crusader"),
                        Mage = GetResultToShow(r, "Mage"),
                        Monk = GetResultToShow(r, "Monk"),
                        Scout = GetResultToShow(r, "Scout"),
                        Sorcerer = GetResultToShow(r, "Sorcerer"),
                        Spellsword = GetResultToShow(r, "Spellsword"),
                        Warrior = GetResultToShow(r, "Warrior"),
                        Redoran = GetResultToShow(r, DeckClass.Redoran.ToString()),
                        Telvanni = GetResultToShow(r, DeckClass.Telvanni.ToString()),
                        Hlaalu = GetResultToShow(r, DeckClass.Hlaalu.ToString()),
                        Tribunal = GetResultToShow(r, DeckClass.Tribunal.ToString()),
                        Dagoth = GetResultToShow(r, DeckClass.Dagoth.ToString()),
                        Total = GetResultToShow(r, "Total"),
                        First_Second = GetResultToShow(r, "First_Second"),
                        FirstWin = GetResultToShow(r, "FirstWin"),
                        SecondWin = GetResultToShow(r, "SecondWin")
                    };
                    foreach(string t in tags)
                    {
                        ret.Tags.Add(t, GetResultToShow(r, t));
                    }
                    return ret;
                });

            Logger.ConditionalTrace("excute query");
            returnList = returnList.ToList();
            Logger.ConditionalTrace("FINISHED");
            return returnList;
        }

        private string GetResultToShow(
            IGrouping<dynamic, DeckStatsDataRecord> rows,
            string header)
        {
            return valueToShow == "Win" ? rows.Where(gg => gg.Opp == header).FirstOrDefault().Win :
                                            (!double.IsNaN(rows.Where(gg => gg.Opp == header).FirstOrDefault().WinPerc) ? Math.Round(rows.Where(gg => gg.Opp == header).FirstOrDefault().WinPerc, 0).ToString() : "");
        }

        private void CreateOpponentHeatMapData(IEnumerable<DeckStatsDataRecord> totalOpponents)
        {
            opponentClassHeatMap = new ChartValues<HeatPoint>();

            if (gropuByOpponent == "class")
            {
                foreach (var r in totalOpponents)
                {
                    DeckClass dc = (DeckClass)Enum.Parse(typeof(DeckClass), r.Opp);
                    DataModel.DeckAttributes da = ClassAttributesHelper.Classes[dc];
                    DeckAttribute da1 = da[0];
                    DeckAttribute da2 = (da.Count > 1 ? da[1] : da[0]);
                    opponentClassHeatMap.Add(new HeatPoint((int)da1, (int)da2, Int32.Parse(r.Win)));
                }
            }
        }

        private IEnumerable<DeckStatsDataRecord> FirstSecondStats(
            IEnumerable<DeckStatsDataRecord> result,
            Func<DataModel.Game, SerializableVersion> groupByVersion)
        {
            //add % of gaames you went first
            result = result.Union(
                         GetFirstSecondData(
                            groupByVersion,
                            (g) => "First_Second"));

            //add % of wins  you went first
            result = result.Union(
                         GetFirstSecondWinrateData(
                             OrderOfPlay.First,
                            groupByVersion,
                            (g) => "FirstWin"));

            //add % of wins you went second
            result = result.Union(
                         GetFirstSecondWinrateData(
                             OrderOfPlay.Second,
                            groupByVersion,
                            (g) => "SecondWin"));
            return result;
        }

        [DebuggerDisplay("Deck={Deck};Ver={DeckVersion};Opp={Opp};Win={Win};WinPerc={WinPerc}")]
        private struct DeckStatsDataRecord
        {
            public object Deck;
            public SerializableVersion DeckVersion;
            public string Opp;
            public string Win;
            public double WinPerc;
        }

        private IEnumerable<DeckStatsDataRecord> GetBreakDownByDeck(
            Func<DataModel.Game, object> groupByDeck,
            Func<DataModel.Game,SerializableVersion> groupByVersion,
            Func<DataModel.Game, string> groupByOpponnetClass)
        {
            return GamesList
                .GroupBy(g => new { D = groupByDeck(g), VS = groupByVersion(g), OC = groupByOpponnetClass(g) })
                        .Select(d => new DeckStatsDataRecord
                        {
                            Deck = d.Key.D,
                            DeckVersion = d.Key.VS,
                            Opp = d.Key.OC,
                            Win = d.Where(d2 => d2.Outcome == GameOutcome.Victory).Count() +
                                "-" + d.Where(d2 => d2.Outcome == GameOutcome.Defeat).Count(),
                            WinPerc = (double)d.Where(g => g.Outcome == GameOutcome.Victory).Count() / d.Count() * 100,
                        });
        }

        private IEnumerable<DeckStatsDataRecord> GetBreakDownByOpponentClass(
            Func<DataModel.Game, object> groupByDeck,
            Func<DataModel.Game, SerializableVersion> groupByVersion,
            Func<DataModel.Game, string> groupByOpponnetClass)
        {
            return GamesList
                .GroupBy(g => new { D = groupByDeck(g), VS = groupByVersion(g), OC = groupByOpponnetClass(g) })
                        .Select(d => new DeckStatsDataRecord
                        {
                            Deck = d.Key.D,
                            DeckVersion = d.Key.VS,
                            Opp = d.Key.OC,
                            Win = Math.Round((double)d.Count() / GamesList.Count() * 100).ToString(),
                            WinPerc = (double)d.Count() / GamesList.Count() * 100,
                        });
        }


        private IEnumerable<DeckStatsDataRecord> GetFirstSecondData(
           Func<DataModel.Game, SerializableVersion> groupByVersion,
           Func<DataModel.Game, string> groupByOpponnetClass)
        {
            return GamesList
                .GroupBy(g => new { D = GetPropertyValue(g, GroupBy), VS = groupByVersion(g), OC = groupByOpponnetClass(g) })
                        .Select(d => new DeckStatsDataRecord
                        {
                            Deck = d.Key.D,
                            DeckVersion = d.Key.VS,
                            Opp = d.Key.OC,
                            Win = d.Where(d2 => d2.OrderOfPlay == OrderOfPlay.First).Count() +
                                        "-" + d.Where(d2 => d2.OrderOfPlay == OrderOfPlay.Second).Count(),
                            WinPerc = (double)d.Where(g => g.OrderOfPlay == OrderOfPlay.First).Count() / d.Count() * 100
                        });
        }

        private IEnumerable<DeckStatsDataRecord> GetFirstSecondWinrateData(
                OrderOfPlay orderOfPlay,
                Func<DataModel.Game, SerializableVersion> groupByVersion,
                Func<DataModel.Game, string> groupByOpponnetClass)
        {
            return GamesList
                .GroupBy(g => new { D = GetPropertyValue(g, GroupBy), VS = groupByVersion(g), OC = groupByOpponnetClass(g) })
                        .Select(d => new DeckStatsDataRecord
                        {
                            Deck = d.Key.D,
                            DeckVersion = d.Key.VS,
                            Opp = d.Key.OC,
                            Win = d.Where(d2 => d2.Outcome == GameOutcome.Victory && d2.OrderOfPlay == orderOfPlay).Count() +
                                        "-" + d.Where(d2 => d2.Outcome == GameOutcome.Defeat && d2.OrderOfPlay == orderOfPlay).Count(),
                            WinPerc = (double)d.Where(g => g.Outcome == GameOutcome.Victory && g.OrderOfPlay == orderOfPlay).Count() / d.Where(g => g.OrderOfPlay == OrderOfPlay.First).Count() * 100
                        });
        }

        private string GetOpponentGroupMethod(DataModel.Game g)
        {
            if (this.gropuByOpponent == "opponentDeckTag")
            {
                return g.OpponentDeckTag == null ? "<none>" : g.OpponentDeckTag;
            }
            else
            {
                return ClassAttributesHelper.Classes[g.OpponentClass.Value].ToString();
            }
        }

        public class GameStatisticsDeckRow
        {
            public string Agility { get; set; }
            public string Archer { get; set; }
            public string Assassin { get; set; }
            public string Battlemage { get; set; }
            public string Crusader { get; set; }
            public object Deck { get; set; }
            public SerializableVersion DeckVersion { get; set; }
            public string Endurance { get; set; }
            public string FirstWin { get; set; }
            public string First_Second { get; set; }
            public string Inteligence { get; set; }
            public string Mage { get; set; }
            public string Monk { get; set; }
            public string Neutral { get; set; }
            public string Scout { get; set; }
            public string SecondWin { get; set; }
            public string Sorcerer { get; set; }
            public string Spellsword { get; set; }
            public string Strength { get; set; }
            public string Total { get; set; }
            public string Warrior { get; set; }
            public string Willpower { get; set; }
            public string Redoran { get; set; }
            public string Telvanni { get; set; }
            public string Hlaalu { get; set; }
            public string Tribunal { get; set; }
            public string Dagoth { get; set; }
            public Dictionary<string, string> Tags { get; set; } = new Dictionary<string, string>();
        }
    }
}
