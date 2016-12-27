using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ESLTracker.DataModel;
using ESLTracker.Properties;

namespace ESLTracker.Utils
{
    public class TrackerFactory : ITrackerFactory
    {
        public static ITrackerFactory DefaultTrackerFactory { get; set; } = new TrackerFactory();

        public ITracker GetTracker()
        {
            return Tracker.Instance;
        }

        public IMessenger GetMessanger()
        {
            return Messenger.Default;
        }

        public DateTime GetDateTimeNow()
        {
            return DateTime.Now;
        }

        public ISettings GetSettings()
        {
            return Properties.Settings.Default;
        }

        public IWinAPI GetWinAPI()
        {
            return new WinAPI();
        }

        public ICardsDatabase GetCardsDatabase()
        {
            return CardsDatabase.Default;
        }
    }
}
