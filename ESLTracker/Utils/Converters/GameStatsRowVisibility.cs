using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using ESLTracker.DataModel;
using ESLTracker.ViewModels;

namespace ESLTracker.Utils.Converters
{
    /// <summary>
    /// this converter is used to control row visibily in game stats. initally hides deck version rows, then shows/hides tham when deck is selected on grid
    /// </summary>
    public class GameStatsRowVisibility : MarkupConverter<GameStatsRowVisibility>, IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            SerializableVersion selectedVersion = (values[0] as DataRowView)?.Row[1] as SerializableVersion;
            Deck selectedDeck = (values[0] as GameStatisticsViewModel.GameStatisticsDeckRow)?.Deck as Deck;

            Deck currentDeck = values[1] as Deck;
            SerializableVersion currentVersion = values[2] as SerializableVersion;
            
            SerializableVersion totalRowMarker = values[3] as SerializableVersion;

            if (selectedDeck != null)
            {
                return currentDeck.DeckId == selectedDeck.DeckId || currentVersion == totalRowMarker;
            }
            else
            {
                //nothing yet selected - show only deck totoals
                return currentVersion == totalRowMarker;
            }
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
