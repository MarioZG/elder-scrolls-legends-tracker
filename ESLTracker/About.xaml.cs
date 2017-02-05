using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using ESLTracker.Utils;

namespace ESLTracker
{
    /// <summary>
    /// Interaction logic for About.xaml
    /// </summary>
    public partial class About : Window
    {
        public string AppVersion
        {
            get
            {
                return TrackerFactory.DefaultTrackerFactory.GetService<IApplicationService>().GetAssemblyInformationalVersion();
            }
        }

        public SerializableVersion FileVersion
        {
            get
            {
                return DataModel.Tracker.Instance.Version;
            }
        }
        public string Address
        {
            get
            {
                return new String(
                    (((char)(109)).ToString() +
                    ((char)(111)).ToString() +
                    ((char)(99)).ToString() +
                    ((char)(46)).ToString() +
                    ((char)(108)).ToString() +
                    ((char)(105)).ToString() +
                    ((char)(97)).ToString() +
                    ((char)(109)).ToString() +
                    ((char)(103)).ToString() +
                    ((char)(64)).ToString() +
                    ((char)(114)).ToString() +
                    ((char)(101)).ToString() +
                    ((char)(107)).ToString() +
                    ((char)(99)).ToString() +
                    ((char)(97)).ToString() +
                    ((char)(114)).ToString() +
                    ((char)(116)).ToString() +
                    ((char)(108)).ToString() +
                    ((char)(115)).ToString() +
                    ((char)(101)).ToString() +
                    ((char)(116)).ToString() +
                    ((char)(58)).ToString() +
                    ((char)(111)).ToString() +
                    ((char)(116)).ToString() +
                    ((char)(108)).ToString() +
                    ((char)(105)).ToString() +
                    ((char)(97)).ToString() +
                    ((char)(109)).ToString())
                    .Reverse().ToArray());
            }
        }

        public string SendMail
        {
            get
            {
                return Address + "?body=Application version: "+this.AppVersion;
            }
        }

        public string CardsDatabaseVersion
        {
            get
            {
                return string.Format("{0} ({1} from {2:d})",
                    Utils.CardsDatabase.Default.Version,
                    Utils.CardsDatabase.Default.VersionInfo,
                    Utils.CardsDatabase.Default.VersionDate);
            }
        }

        public About()
        {
            this.Owner = Application.Current.MainWindow;
            InitializeComponent();
        }

        private void Hyperlink_Click(object sender, RoutedEventArgs e)
        {
            var link = (Hyperlink)sender;
            Process.Start(link.NavigateUri.ToString());
        }

        private void Hyperlink_Click_1(object sender, RoutedEventArgs e)
        {
            System.Windows.Clipboard.SetText(Address.Substring(7));
        }
    }
}
