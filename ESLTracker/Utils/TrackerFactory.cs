using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ESLTracker.DataModel;
using ESLTracker.Properties;

namespace ESLTracker.Utils
{
    class TrackerFactory : ITrackerFactory
    {
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
    }
}
