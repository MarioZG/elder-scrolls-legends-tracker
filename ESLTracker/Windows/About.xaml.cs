using ESLTracker.ViewModels.Windows;
using System.Diagnostics;
using System.Windows;
using System.Windows.Documents;

namespace ESLTracker.Windows
{
    /// <summary>
    /// Interaction logic for About.xaml
    /// </summary>
    public partial class About : Window
    {
        
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
            Clipboard.SetText(((AboutViewModel)this.DataContext).Address.Substring(7));
        }
    }
}
