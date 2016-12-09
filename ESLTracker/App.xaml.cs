using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Markup;

namespace ESLTracker
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public bool IsApplicationClosing { get; set; } = false;

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

        private void Application_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            System.IO.File.WriteAllText("./crash" + DateTime.Now.ToString("yyyyMMddHHmm") + ".txt", e.Exception.ToString());
            MessageBox.Show("Applicaiton encountered unhandled exception. Log fiel has been created in file " + "./crash" + DateTime.Now.ToString("yyyyMMddHHmm") + ".txt with details.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            e.Handled = false;
        }
    }
}
