using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using ESLTracker.DataModel.Enums;
using ESLTracker.Utils;
using LiveCharts;
using LiveCharts.Defaults;
using LiveCharts.Wpf;

namespace ESLTracker.ViewModels.Game
{
    public class RankedProgressChartViewModel : GameFilterViewModel
    {

        static int RankJump = 10;

        public override IEnumerable<GameType> GameTypeSeletorValues
        {
            get
            {
                return new List<GameType>()
                {
                    GameType.PlayRanked
                };
            }
        }

        public SeriesCollection SeriesCollection {
            get { return GetDataSet(); }
        }

        private IList<string> labels = new List<string>();
        public IList<string> Labels
        {
            get {
                return labels;
            }
        }

        public Func<double, string> Formatter { get; set; }

        public RankedProgressChartViewModel() : this(TrackerFactory.DefaultTrackerFactory)
        {

        }

        public RankedProgressChartViewModel(ITrackerFactory trackerFactory) : base(trackerFactory)
        {
            this.gameType = GameType.PlayRanked;
            Formatter = value => ((PlayerRank)(12 - (((int)value +2 ) / RankJump))).ToString();
        }

        public static int GetPlayerRankStartValue(PlayerRank pr)
        {
            return (12 - (int)pr) * RankJump;
        }

        public override dynamic GetDataSet()
        {
            var games = trackerFactory.GetTracker()
                 .Games
                 .Where(g => g.Type == GameType.PlayRanked 
                        &&  ((filterDateFrom == null) ||  (g.Date >= filterDateFrom))
                        && ((filterDateTo == null) || (g.Date <= filterDateTo))
                        )
                 .OrderBy(g => g.Date);

            int currval = 0;
            PlayerRank? currentPR = null;
            Dictionary<string, int> val = new Dictionary<string, int>();
            Dictionary<DateTime, Tuple<int, int, int>> chartData = new Dictionary<DateTime, Tuple<int, int, int>>();
            foreach (dynamic r in games)
            {
                if (currentPR != r.PlayerRank)
                {
                    //rank up!
                    currentPR = r.PlayerRank;
                    currval = 0;
                };
                if (r.Outcome == GameOutcome.Victory)
                {
                    currval += r.BonusRound ? 2 : 1;
                }
                else
                {
                    currval--;
                    if (currval < -2)
                    { //min serpent - cannot drop more
                        currval = -2;
                    }
                }
                var value = currval + (12 - (int)currentPR) * RankJump;
                val.Add(r.PlayerRank + "" + r.Outcome + r.Date + Guid.NewGuid().ToString(), value);
                if (chartData.ContainsKey(r.Date.Date))
                {
                    if (chartData[r.Date.Date].Item1 > value)
                    {
                        chartData[r.Date.Date] = new Tuple<int, int, int>(value, chartData[r.Date.Date].Item2, chartData[r.Date.Date].Item3);
                    }
                    if (chartData[r.Date.Date].Item3 < value)
                    {
                        chartData[r.Date.Date] = new Tuple<int, int, int>(chartData[r.Date.Date].Item1, chartData[r.Date.Date].Item2, value);
                    }
                    //curr(last) value
                    chartData[r.Date.Date] = new Tuple<int, int, int>(chartData[r.Date.Date].Item1, value, chartData[r.Date.Date].Item3);
                }
                else
                {
                    chartData.Add(r.Date.Date, new Tuple<int, int, int>(value, value, value));
                }
            }

            OhlcSeries os = new OhlcSeries();
            os.Values = new ChartValues<OhlcPoint>();

            LineSeries ls = new LineSeries();
            ls.Values = new ChartValues<int>();
            foreach (var row in chartData.Values)
            {
                os.Values.Add(new OhlcPoint(row.Item2, row.Item3, row.Item1, row.Item2));
                ls.Values.Add(row.Item2);
            }

            //foreach (var row in val.Values)
            //{
            //    //os.Values.Add(row);
            //    ls.Values.Add(row);
            //}

            foreach (var key in chartData.Keys)
            {
                labels.Add(key.Date.Date.ToString("d"));
            }

            SeriesCollection sc = new SeriesCollection();
            sc.Add(os);
            sc.Add(ls);

            RaisePropertyChangedEvent("Labels");

            return sc;
        }
    }
}
