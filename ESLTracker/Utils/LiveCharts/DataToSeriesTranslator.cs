using ESLTracker.BusinessLogic.Cards;
using ESLTracker.BusinessLogic.Packs;
using TESLTracker.DataModel.Enums;
using ESLTracker.Utils;
using ESLTracker.Utils.Extensions;
using LiveCharts;
using LiveCharts.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace ESLTracker.Utils.LiveCharts
{
    //extract livecharts dpendecies from PacksChartsDataCalculator, move to seprate project later on
    public class DataToSeriesTranslator : IDataToSeriesTranslator
    {

        public DataToSeriesTranslator()
        {
        }

        public object CreateSeries(string title, Brush fill, decimal value)
        {
            return new PieSeries
            {
                Title = title,
                Fill = fill,
                Values = new ChartValues<decimal>() { value },
                DataLabels = false,
                LabelPoint = chartPoint => string.Format("{0:P}", chartPoint.Participation)
            };
        }

        public object CreateSeriesCollection(IEnumerable<object> data)
        {
            SeriesCollection sc = null;
            if (data.Count() > 0)
            {
                sc = new SeriesCollection();
                sc.AddRange(data);
            }
            return sc;
        }
    }
}
