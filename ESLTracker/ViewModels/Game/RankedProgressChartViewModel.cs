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

        private bool connectMin;

        public bool ConnectMin
        {
            get { return connectMin; }
            set {
                connectMin = value;
                ShowSeries();
                RaisePropertyChangedEvent(); }
        }

        private bool connectMax;

        public bool ConnectMax
        {
            get { return connectMax; }
            set {
                connectMax = value;
                ShowSeries();
                RaisePropertyChangedEvent(); }
        }

        private bool connectLast = true;

        public bool ConnectLast
        {
            get { return connectLast; }
            set {
                connectLast = value;
                ShowSeries();
                RaisePropertyChangedEvent(); }
        }

        private bool showMaxMin = true;
        private LineSeries lsLastInDay;
        private LineSeries lsMinInDay;
        private LineSeries lsMaxInDay;
        private LineSeries lsAll;
        private SeriesCollection seriesCollection;
        private OhlcSeries ohlcMinMax;

        public bool ShowMaxMin
        {
            get { return showMaxMin; }
            set {
                showMaxMin = value;
                ShowSeries();
                RaisePropertyChangedEvent(); }
        }

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
                        && ((filterDateFrom == null) || (g.Date >= filterDateFrom))
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

            ohlcMinMax = new OhlcSeries();
            ohlcMinMax.Values = new ChartValues<OhlcPoint>();

            lsLastInDay = new LineSeries();
            lsLastInDay.Values = new ChartValues<int>();
            lsMinInDay = new LineSeries();
            lsMinInDay.Values = new ChartValues<int>();
            lsMaxInDay = new LineSeries();
            lsMaxInDay.Values = new ChartValues<int>();
            lsAll = new LineSeries();
            lsAll.Values = new ChartValues<int>();
            foreach (var row in chartData.Values)
            {
                ohlcMinMax.Values.Add(new OhlcPoint(row.Item2, row.Item3, row.Item1, row.Item2));
                lsMinInDay.Values.Add(row.Item1);
                lsLastInDay.Values.Add(row.Item2);
                lsMaxInDay.Values.Add(row.Item3);
            }
            foreach (var row in val.Values)
            {
                lsAll.Values.Add(row);
            }


            foreach (var key in chartData.Keys)
            {
                labels.Add(key.Date.Date.ToString("d"));
            }

            seriesCollection = new SeriesCollection();
            ShowSeries();

            RaisePropertyChangedEvent("Labels");

            return seriesCollection;
        }

        private void ShowSeries()
        {
            seriesCollection.Clear();
            if (showMaxMin)
            {
                seriesCollection.Add(ohlcMinMax);
                if (connectLast)
                {
                    seriesCollection.Add(lsLastInDay);
                }
                if (connectMin)
                {
                    seriesCollection.Add(lsMinInDay);
                }
                if (connectMax)
                {
                    seriesCollection.Add(lsMaxInDay);
                }
            }
            else
            {
                seriesCollection.Add(lsAll);
            }
        }
    }
}
