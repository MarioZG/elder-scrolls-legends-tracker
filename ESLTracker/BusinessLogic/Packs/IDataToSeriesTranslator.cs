using System.Collections.Generic;
using System.Windows.Media;
using LiveCharts.Wpf;

namespace ESLTracker.BusinessLogic.Packs
{
    public interface IDataToSeriesTranslator
    {
        object CreateSeries(string title, Brush fill, decimal value);
        object CreateSeriesCollection(IEnumerable<object> data);
    }
}