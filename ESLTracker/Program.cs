using ESLTracker.BusinessLogic.GameClient;
using ESLTracker.DataModel;
using ESLTracker.Properties;
using ESLTracker.Services;
using ESLTracker.Utils;
using ESLTracker.Utils.DiagnosticsWrappers;
using ESLTracker.Utils.SimpleInjector;
using ESLTracker.ViewModels;
using NLog;
using NLog.Config;
using SimpleInjector;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace ESLTracker
{
    static class Program
    {
        public static bool IsApplicationClosing { get; set; } = false;
        static SingleInstanceApp singleInstance;
        internal const string UserInfoLogger = "UserInfoLogger";
        private const string NewVersionAvailable = "New version of tracker is available.";
        private const string OpenChangelog = "Open changelog";
        private const string Download = "Download";
        private const string CardsDatabaseUpdated = "Cards database has been updated to latest version (v{0} from {1}: {2})";


        [STAThread]
        static void Main()
        {
            InitNLog();

            //AppDomain.CurrentDomain.UnhandledException += (s, ex) =>
            //    HandleUnhandledException((Exception)ex.ExceptionObject, "AppDomain.CurrentDomain.UnhandledException");

            //DispatcherUnhandledException += (s, ex) =>
            //    HandleUnhandledException(ex.Exception, "Application.Current.DispatcherUnhandledException");

            //TaskScheduler.UnobservedTaskException += (s, ex) =>
            //    HandleUnhandledException(ex.Exception, "TaskScheduler.UnobservedTaskException");

            var container = new MasserContainer();

            CheckSingleInstance();
            CheckDataFile(container.GetInstance<IFileManager>());
            IVersionService vc = container.GetInstance<IVersionService>();
            var settings = container.GetInstance<ISettings>();
            var newVersion = vc.CheckNewAppVersionAvailable();
            if (newVersion.IsAvailable)
            {
                Logger userInfo = LogManager.GetLogger(App.UserInfoLogger);
                userInfo.Info(NewVersionAvailable, new Dictionary<string, string> {
                    { OpenChangelog, settings.VersionCheck_LatestBuildUserUrl },
                    { Download, newVersion.DownloadUrl }
                });
            }
            if (vc.IsNewCardsDBAvailable())
            {
                ICardsDatabase cardsDB = vc.GetLatestCardsDB();
                Logger log = LogManager.GetLogger(App.UserInfoLogger);
                log.Info(CardsDatabaseUpdated, new object[] { cardsDB.Version, cardsDB.VersionDate.ToShortDateString(), cardsDB.VersionInfo });
            }

            bool isShiftPressed = (Keyboard.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift;
            if (settings.General_StartGameWithTracker && !isShiftPressed)
            {
                var winApi = container.GetInstance<IWinAPI>();
                var messanger = container.GetInstance<IMessenger>();
                container.GetInstance<ILauncherService>().StartGame();
            }

            RunApplication(container);
        }

        private static void InitNLog()
        {
            ConfigurationItemFactory.Default.Targets
                .RegisterDefinition("UserInfoLogger", typeof(ESLTracker.Utils.NLog.UserInfoLoggerTarget));
        }

        private static void RunApplication(Container container)
        {
            try
            {
                ResourceDictionary myResourceDictionary = new ResourceDictionary();

                myResourceDictionary.Source = new Uri("ControlStyle.xaml", UriKind.Relative);
                Application.Current.Resources.MergedDictionaries.Add(myResourceDictionary);


                var app = new App();
                var mainWindow = container.GetInstance<MainWindow>();
                app.Run(mainWindow);
            }
            catch (Exception ex)
            {
                Debugger.Launch();
                //Log the exception and exit
            }
        }

        private static void CheckDataFile(IFileManager fileManager)
        {
            try
            {
                //try to open data file
                fileManager.LoadDatabase(true);
            }
            catch (DataFileException ex)
            {
                bool shutdown = true;
                if (ex.CanContinue)
                {
                    MessageBoxResult res = MessageBox.Show(ex.Message, "Datafile problem", MessageBoxButton.YesNo, MessageBoxImage.Exclamation, MessageBoxResult.No);
                    shutdown = res == MessageBoxResult.No;
                }
                else
                {
                    MessageBox.Show("Application encountered problems opening data file: " + ex.Message);
                }
                if (shutdown)
                {
                    Environment.Exit(1); //app.shutdown still init mainwindow and othe cmponents :/
                }
            }
        }

        private static void CheckSingleInstance()
        {
            singleInstance = new SingleInstanceApp();
            if (!singleInstance.CheckInstance())
            {
                MessageBox.Show("ESL Tracker is alrady running", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Application.Current.Shutdown();
            }
        }
    }
}
