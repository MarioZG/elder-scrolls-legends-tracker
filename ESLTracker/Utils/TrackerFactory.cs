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
    public class TrackerFactory : ITrackerFactory
    {
        public static ITrackerFactory DefaultTrackerFactory { get; set; } = new TrackerFactory();

        public T GetService<T>() where T: class
        {
            Type type = typeof(T);
            if (type == typeof(IHTTPService)) {
                return new HTTPService(this) as T;
            }
            else if (type == typeof(IApplicationService))
            {
                return new ApplicationService() as T;
            }
            else if (type == typeof(IFileWrapper))
            {
                return new FileWrapper() as T;
            }
            else if (type == typeof(ICardsDatabase))
            {
#pragma warning disable CS0618 // Type or member is obsolete
                return CardsDatabase.Default as T;
#pragma warning restore CS0618 // Type or member is obsolete
            }
            else {
                throw new NotImplementedException(typeof(T).Name);
            }
        }


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
            return WinAPI.Default;
        }

        public ICardsDatabase GetCardsDatabase()
        {
            return CardsDatabase.Default;
        }

        public IWrapperProvider GetWrapperProvider()
        {
           return WrapperProvider.Instance;
        }

        IFileManager fileManager;
        public IFileManager GetFileManager()
        {
            if (fileManager == null)
            {
                //cannot use singleton, as we need to pass this ref
                fileManager = new FileManager(this);
            }
            return fileManager;
        }

        public Guid GetNewGuid()
        {
            return Guid.NewGuid();
        }
    }
}
