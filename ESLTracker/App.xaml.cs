using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Markup;
using ESLTracker.Utils;
using ESLTracker.Properties;
using System.Windows.Input;
using ESLTracker.BusinessLogic.GameClient;
using ESLTracker.Utils.SimpleInjector;
using ESLTracker.Windows;
using ESLTracker.BusinessLogic.Cards;
using ESLTracker.BusinessLogic.General;
using ESLTracker.Controls;
using ESLTracker.BusinessLogic.DataFile;

namespace ESLTracker
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public bool IsApplicationClosing { get; set; } = false;

        SingleInstanceApp singleInstance;
        private const string NewVersionAvailable = "New version of tracker is available.";
        private const string OpenChangelog = "Open changelog";
        private const string Download = "Download";
        private const string CardsDatabaseUpdated = "Cards database has been updated to latest version (v{0} from {1}: {2})";

        static App()
        {
            FrameworkElement.LanguageProperty.OverrideMetadata(
                typeof(FrameworkElement),
                new FrameworkPropertyMetadata(
                    XmlLanguage.GetLanguage(CultureInfo.CurrentCulture.IetfLanguageTag)));
        }

        public void CloseApplication()
        {
            IsApplicationClosing = true;
            this.Shutdown();
        }


        private void Application_Exit(object sender, ExitEventArgs e)
        {
            singleInstance.Dispose();
        }

        private void HandleUnhandledException(Exception ex, string source)
        {
            string filename = "./crash" + DateTime.Now.ToString("yyyyMMddHHmm") + ".txt";
            string verInfo = String.Join(";",Assembly.GetEntryAssembly().CustomAttributes.Where(ca => ca.AttributeType == typeof(AssemblyInformationalVersionAttribute)).FirstOrDefault()?.ConstructorArguments);
            System.IO.File.WriteAllText(filename, "APP VERSION: "+ verInfo +Environment.NewLine);
            System.IO.File.AppendAllText(filename, ex.ToString());
            MessageBox.Show("Application encountered unhandled exception. Log file has been created in " + "./crash" + DateTime.Now.ToString("yyyyMMddHHmm") + ".txt with details.", "Error", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, MessageBoxOptions.ServiceNotification);
        }


        private void Application_Startup(object sender, StartupEventArgs e)
        {

            SplashScreenManager splash = new SplashScreenManager();
            splash.ShowSplash();
            splash.UpdateProgress("Loading loggers");

            splash.UpdateProgress("Configuring dependecies");

            var container = new MasserContainer();
            container.Verify();  //must be here, otherwise lot of XAML errors in unit tests


            #if DEBUG

            if (System.Diagnostics.Debugger.IsAttached)
            {
                System.Diagnostics.PresentationTraceSources.Refresh();
                System.Diagnostics.PresentationTraceSources.DataBindingSource.Listeners.Add(new System.Diagnostics.ConsoleTraceListener());
                System.Diagnostics.PresentationTraceSources.DataBindingSource.Listeners.Add(new DebugTraceListener());
                System.Diagnostics.PresentationTraceSources.DataBindingSource.Switch.Level = System.Diagnostics.SourceLevels.Warning | System.Diagnostics.SourceLevels.Error;
            }

            #endif



            AppDomain.CurrentDomain.UnhandledException += (s, ex) =>
                HandleUnhandledException((Exception)ex.ExceptionObject, "AppDomain.CurrentDomain.UnhandledException");

            DispatcherUnhandledException += (s, ex) =>
                HandleUnhandledException(ex.Exception, "Application.Current.DispatcherUnhandledException");

            TaskScheduler.UnobservedTaskException += (s, ex) =>
                HandleUnhandledException(ex.Exception, "TaskScheduler.UnobservedTaskException");

            splash.UpdateProgress("Checking other instances");
            if (CheckSingleInstance())
            {
                splash.UpdateProgress("Other instance is running. Cancel start-up process");
                IsApplicationClosing = true;
                return; //if alrady running just stop
            }

            splash.UpdateProgress("Checking data file");
            CheckDataFile(container.GetInstance<FileLoader>());

            splash.UpdateProgress("Checking for new version");
            IVersionService vc = container.GetInstance<IVersionService>();
            var settings = container.GetInstance<ISettings>();
            var newVersion = vc.CheckNewAppVersionAvailable();
            if (newVersion.IsAvailable)
            {
                var userMessages = container.GetInstance<UserInfoMessages>();
                userMessages.AddMessage(
                        NewVersionAvailable,
                        new Dictionary<string, string> {
                            { OpenChangelog, settings.VersionCheck_LatestBuildUserUrl },
                            { Download, newVersion.DownloadUrl }
                        });
            }

            splash.UpdateProgress("Checking for card database updates");
            if (vc.IsNewCardsDBAvailable())
            {
                ICardsDatabase cardsDB = vc.GetLatestCardsDB();
                if (cardsDB != null)
                {
                    var userMessages = container.GetInstance<UserInfoMessages>();
                    userMessages.AddMessage(
                            string.Format(CardsDatabaseUpdated,
                                        new object[] { cardsDB.Version, cardsDB.VersionDate.ToShortDateString(), cardsDB.VersionInfo }
                            )
                        );
                }
            }

            bool isShiftPressed = (Keyboard.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift;
            if (settings.General_StartGameWithTracker && !isShiftPressed)
            {
                splash.UpdateProgress("Starting game");
                container.GetInstance<ILauncherService>().StartGame();
            }

            splash.UpdateProgress("Creating main form");
            MainWindow main = container.GetInstance<MainWindow>();
            this.MainWindow = main;
            this.MainWindow.Show();

            //init overlays
            splash.UpdateProgress("Creating overlays");
            var overlaysRepo = container.GetInstance<OverlayWindowRepository>();
            IEnumerable<OverlayWindowBase> overlayWindowsList = container.GetInstance<IEnumerable<OverlayWindowBase>>();
            overlaysRepo.RegisterWindows(overlayWindowsList);
            Task.Run(() => overlaysRepo.UpdateOverlayAsync(main, container.GetInstance<IWinAPI>()));

            splash.CloseSplash();
            this.MainWindow.Activate();

        }

        private static void CheckDataFile(FileLoader fileLoader)
        {
            try
            {
                //try to open data file
                fileLoader.LoadDatabase(true);
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

        private bool CheckSingleInstance()
        {
            singleInstance = new SingleInstanceApp();
            if (!singleInstance.CheckInstance())
            {
                MessageBox.Show("ESL Tracker is alrady running", "Error", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK,MessageBoxOptions.ServiceNotification);
                Application.Current.Shutdown();
                return true;
            }
            return false;
        }
    }
}
