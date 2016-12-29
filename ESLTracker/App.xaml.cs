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

namespace ESLTracker
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public bool IsApplicationClosing { get; set; } = false;
        SingleInstanceApp singleInstance;

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

        private void Application_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            string filename = "./crash" + DateTime.Now.ToString("yyyyMMddHHmm") + ".txt";
            string verInfo = String.Join(";",Assembly.GetEntryAssembly().CustomAttributes.Where(ca => ca.AttributeType == typeof(AssemblyInformationalVersionAttribute)).FirstOrDefault()?.ConstructorArguments);
            System.IO.File.WriteAllText(filename, "APP VERSION: "+ verInfo +Environment.NewLine);
            System.IO.File.AppendAllText(filename, e.Exception.ToString());
            MessageBox.Show("Application encountered unhandled exception. Log file has been created in " + "./crash" + DateTime.Now.ToString("yyyyMMddHHmm") + ".txt with details.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            e.Handled = false;
        }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            CheckSingleInstance();
            CheckDataFile();
        }

        private static void CheckDataFile()
        {
            try
            {
                //try to open data file
                new FileManager().LoadDatabase(true);
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

        private void CheckSingleInstance()
        {
            singleInstance = new SingleInstanceApp();
            if (!singleInstance.CheckInstance())
            {
                MessageBox.Show("ESL Tracker is alrady running", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                App.Current.Shutdown();
            }
        }
    }
}
