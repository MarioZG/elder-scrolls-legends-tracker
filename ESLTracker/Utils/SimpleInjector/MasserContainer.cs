using ESLTracker.BusinessLogic.Cards;
using ESLTracker.BusinessLogic.DataFile;
using ESLTracker.BusinessLogic.Decks;
using ESLTracker.BusinessLogic.Decks.DeckExports;
using ESLTracker.BusinessLogic.Decks.DeckImports;
using ESLTracker.BusinessLogic.GameClient;
using ESLTracker.BusinessLogic.Games;
using ESLTracker.BusinessLogic.General;
using ESLTracker.BusinessLogic.Packs;
using ESLTracker.BusinessLogic.Rewards;
using ESLTracker.Controls;
using TESLTracker.DataModel;
using ESLTracker.Properties;
using ESLTracker.Utils.DiagnosticsWrappers;
using ESLTracker.Utils.FileUpdaters;
using ESLTracker.Utils.IOWrappers;
using ESLTracker.Utils.LiveCharts;
using ESLTracker.Utils.NLog;
using ESLTracker.Utils.SystemWindowsWrappers;
using ESLTracker.ViewModels;
using ESLTracker.ViewModels.Decks;
using ESLTracker.Windows;
using SimpleInjector;
using SimpleInjector.Diagnostics;
using System.Linq;
using System.Text;
using TESLTracker.Utils;
using System;

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
            Register<OverlayWindowRepository>(Lifestyle.Singleton);  //ensre only one repo
            Register<MainWindow>(Lifestyle.Singleton);  //ensure only one main window
            Register<UserInfoMessages>(Lifestyle.Singleton); //one list for info messages

            Register<ITrackerFactory, TrackerFactory>();
            Register<IFileSaver, FileSaver>();
            Register<ITracker>(() => GetInstance<TrackerFactory>().GetTrackerInstance());
            Register<IMessenger, Messenger>(Lifestyle.Singleton);
            Register<IDeckService, DeckService>();
            Register<ICardsDatabaseFactory, CardsDatabaseFactory>(Lifestyle.Singleton); 
            Register<ICardInstanceFactory, CardInstanceFactory>(Lifestyle.Singleton); 
            Register<ICardsDatabase>(() => GetInstance<ICardsDatabaseFactory>().GetCardsDatabase());  // can be reloaded, singelton is not good here 
            Register<DoubleCardsCalculator, DoubleCardsCalculator>(Lifestyle.Singleton); 
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
            Register<IRewardFactory, RewardFactory>();
            Register<IResources, Resources>();
            Register<IGuidProvider, GuidProvider>();
            Register<IGameFactory, GameFactory>();
            Register<IScreenShot, ScreenShot>();
            Register<IWinDialogs, WinDialogs>();
            Register<IClipboardWrapper, ClipboardWrapper>();
            Register<IDeckTextExport, BBCodeExport>();
            Register<IDeckExporter, DeckExporterText>();
            Register<IDataToSeriesTranslator, DataToSeriesTranslator>();
            Register<ICardSPCodeProvider, CardSPCodeProvider>();
            Register<IDeckEditImportDeckViewModel, DeckEditImportDeckViewModel>();
            

            Collection.Register<OverlayWindowBase>(typeof(App).Assembly);  //overlay windows
            Collection.Register<ViewModelBase>(typeof(App).Assembly);  //all view models, not needed but allows to verify all view models when Verify() is called
            Collection.Register<UpdateBase>(typeof(App).Assembly); //file updates

            Collection.Register<IDeckImporter>(typeof(App).Assembly); //text, web and code importers
            Collection.Register<IDeckExporter>(typeof(App).Assembly); //text, web and code importers

            RegisterConditional(
                typeof(ILogger),
                c => typeof(NLogLoggerProxy<>).MakeGenericType(c.Consumer.ImplementationType),
                Lifestyle.Singleton,
                c => c.Consumer != null);

            //for static classes - needs to be refactored later on
            RegisterConditional(
                typeof(ILogger),
                c => typeof(NLogLoggerProxy<>).MakeGenericType(typeof(object)),
                Lifestyle.Singleton,
                c => c.Consumer == null);
        }

        public string AnalizeDependecies()
        {
            try
            {
                Verify();
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
            var results = Analyzer.Analyze(this)
                .OfType<ShortCircuitedDependencyDiagnosticResult>();

            StringBuilder dependecies = new StringBuilder();
            foreach (var result in results)
            {
                dependecies.AppendLine(result.Description);
                dependecies.AppendLine(
                    "Lifestyle of service with the short circuited dependency: " +
                    result.Relationship.Lifestyle.Name);

                dependecies.AppendLine("One of the following types was expected instead:");
                foreach (var expected in result.ExpectedDependencies)
                {
                    dependecies.AppendLine("-" + expected.ServiceType.FullName);
                }
            }

            return dependecies.ToString();
        }
    }
}
