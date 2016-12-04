using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ESLTracker.DataModel;

namespace ESLTracker.Utils
{
    class TrackerFactory : ITrackerFactory
    {
        public ITracker GetTracker()
        {
            return Tracker.Instance;
        }
    }
}
