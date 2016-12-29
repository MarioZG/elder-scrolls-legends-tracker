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

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            singleInstance = new SingleInstanceApp();
            if (! singleInstance.CheckInstance())
            {
                MessageBox.Show("ESL Tracker is alrady running", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                App.Current.Shutdown();
            }
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
    }
}
