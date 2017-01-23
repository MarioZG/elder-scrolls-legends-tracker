using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ESLTracker.DataModel;
using ESLTracker.Properties;
using ESLTracker.Utils.IOWrappers;

namespace ESLTracker.Utils
{
    public interface ITrackerFactory
    {
        T GetService<T>() where T : class;
        ITracker GetTracker();
        IMessenger GetMessanger();
        DateTime GetDateTimeNow();
        ISettings GetSettings();
        IWinAPI GetWinAPI();
        ICardsDatabase GetCardsDatabase();
        IWrapperProvider GetWrapperProvider();
        IFileManager GetFileManager();
        Guid GetNewGuid();
    }
}
