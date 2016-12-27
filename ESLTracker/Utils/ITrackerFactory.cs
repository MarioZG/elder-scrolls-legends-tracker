using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ESLTracker.DataModel;
using ESLTracker.Properties;

namespace ESLTracker.Utils
{
    public interface ITrackerFactory
    {
        ITracker GetTracker();
        IMessenger GetMessanger();
        DateTime GetDateTimeNow();
        ISettings GetSettings();
        IWinAPI GetWinAPI();
        ICardsDatabase GetCardsDatabase();
    }
}
