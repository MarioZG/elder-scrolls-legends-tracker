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
    public class RankedProgressChartViewModel : ViewModelBase
    {
        private ITrackerFactory trackerFactory;

        static int RankJump = 10;

        public SeriesCollection SeriesCollection {
            get { return GetData(); }
        }

        private IList<string> labels = new List<string>();
        public IList<string> Labels
        {
            get {
                return labels;
            }
        }

        public string[] LabelsY
        {
            get
            {
                return new[] { "aa", "bb" };// Enum.GetValues(typeof(PlayerRank)).OfType<string>().ToArray();
            }
        }

        public Func<double, string> Formatter { get; set; }

        public RankedProgressChartViewModel() : this(TrackerFactory.DefaultTrackerFactory)
        {

        }

        public RankedProgressChartViewModel(ITrackerFactory defaultTrackerFactory)
        {
            this.trackerFactory = defaultTrackerFactory;
            Formatter = value => ((PlayerRank)(12 - (((int)value +2 ) / RankJump))).ToString();
        }

        public static int GetPlayerRankStartValue(PlayerRank pr)
        {
            return (12 - (int)pr) * RankJump;
        }

        public SeriesCollection GetData()
        {
            var games = trackerFactory.GetTracker()
                .Games
                .Where(g => g.Type == GameType.PlayRanked && g.Date > DateTime.Now.AddDays(-27))
                .OrderBy(g => g.Date);

            int currval = 0;
            PlayerRank? cPR = null;
            Dictionary<string, int> val = new Dictionary<string, int>();
            Dictionary<DateTime, Tuple<int, int, int>> chartData = new Dictionary<DateTime, Tuple<int, int, int>>();
            foreach (dynamic r in games)
            {
                if (cPR != r.PlayerRank)
                {
                    //rank up!
                    cPR = r.PlayerRank;
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
                    { //serpent
                        currval = -2;
                    }
                }
                var value = currval + (12 - (int)cPR) * RankJump;
                val.Add(r.PlayerRank + "" + r.Outcome + r.Date, value);
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
