using ESLTracker.BusinessLogic.Cards;
using ESLTracker.BusinessLogic.DataFile;
using ESLTracker.BusinessLogic.Decks;
using ESLTracker.BusinessLogic.GameClient;
using ESLTracker.BusinessLogic.General;
using ESLTracker.BusinessLogic.Rewards;
using ESLTracker.Controls;
using ESLTracker.DataModel;
using ESLTracker.Properties;
using ESLTracker.Services;
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
         //   Options.PropertySelectionBehavior = new ImportPropertySelectionBehavior();

            // Register your types, for instance:
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
            Register<ICardImageService, CardImageService>();
            Register<IDateTimeProvider, DateTimeProvider>();
            Register<IAddSingleRewardViewModelFactory, AddSingleRewardViewModelFactory>();
            Register<PathManager, PathManager>(Lifestyle.Singleton);
            Register<IVersionService, VersionService>();
            Register<IHTTPService, HTTPService>();
            Register<IApplicationService, ApplicationService>();
            Register<IDeckImporter, DeckImporter>();
            Register<IRewardFactory, RewardFactory>();
            Register<IResourcesService, ResourcesService>();
            Register<IGuidProvider, GuidProvider>();
        //    Register<IDeckVersionFactory, DeckVersionFactory>();
            


            Collection.Register<OverlayWindowBase>(typeof(App).Assembly);
            Collection.Register<ViewModelBase>(typeof(App).Assembly);
            Collection.Register<UpdateBase>(typeof(App).Assembly);

            
            // Register your windows and view models:
            //     Register<MainWindow>();
            //     Register<MainWindowViewModel>();


            // Verify();
        }
    }
}
