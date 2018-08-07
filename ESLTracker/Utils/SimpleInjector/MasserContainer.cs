using ESLTracker.BusinessLogic.Cards;
using ESLTracker.BusinessLogic.DataFile;
using ESLTracker.BusinessLogic.Decks;
using ESLTracker.BusinessLogic.GameClient;
using ESLTracker.BusinessLogic.Games;
using ESLTracker.BusinessLogic.General;
using ESLTracker.BusinessLogic.Rewards;
using ESLTracker.Controls;
using ESLTracker.DataModel;
using ESLTracker.Properties;
using ESLTracker.Utils.DiagnosticsWrappers;
using ESLTracker.Utils.FileUpdaters;
using ESLTracker.Utils.IOWrappers;
using ESLTracker.ViewModels;
using SimpleInjector;

namespace ESLTracker.Utils.SimpleInjector
{
    public class MasserContainer : Container
    {
        public static MasserContainer Container { get; private set; }

        public MasserContainer()
        {
            Container = this;

            Bootstrap();
        }

        private void Bootstrap()
        {
            Register<ITrackerFactory, TrackerFactory>();
            Register<IFileSaver, FileSaver>();
            Register<ITracker>(() => GetInstance<TrackerFactory>().GetTrackerInstance());
            Register<IMessenger, Messenger>(Lifestyle.Singleton);
            Register<IDeckService, DeckService>();
            Register<ICardsDatabaseFactory, CardsDatabaseFactory>(Lifestyle.Singleton); 
            Register<ICardInstanceFactory, CardInstanceFactory>(Lifestyle.Singleton); 
            Register<ICardsDatabase>(() => GetInstance<ICardsDatabaseFactory>().GetCardsDatabase());  // can be reloaded, singelton is not good here 
            Register<ISettings>(() => Settings.Default, Lifestyle.Singleton);
            Register<IWinAPI, WinAPI>();
            Register<IProcessWrapper, ProcessWrapper>();
            Register<IPathWrapper, PathWrapper>();
            Register<IFileWrapper, FileWrapper>(Lifestyle.Singleton);
            Register<IDirectoryWrapper, DirectoryWrapper>();
            Register<ILauncherService, LauncherService>();
            Register<ICardImage, CardImage>();
            Register<IDateTimeProvider, DateTimeProvider>();
            Register<IAddSingleRewardViewModelFactory, AddSingleRewardViewModelFactory>();
            Register<PathManager, PathManager>(Lifestyle.Singleton);
            Register<IVersionService, VersionService>();
            Register<IHTTPService, HTTPService>();
            Register<IApplicationInfo, ApplicationInfo>();
            Register<IDeckImporter, DeckImporter>();
            Register<IRewardFactory, RewardFactory>();
            Register<IResources, Resources>();
            Register<IGuidProvider, GuidProvider>();
            Register<IGameFactory, GameFactory>();
            Register<IScreenShot, ScreenShot>();
            Register<IWinDialogs, WinDialogs>();

            Collection.Register<OverlayWindowBase>(typeof(App).Assembly);  //overlay windows
            Collection.Register<ViewModelBase>(typeof(App).Assembly);  //all view models, not needed but allows to verify all view models when Verify() is called
            Collection.Register<UpdateBase>(typeof(App).Assembly); //file updates
        }
    }
}
