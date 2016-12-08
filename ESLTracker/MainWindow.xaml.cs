using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ESLTracker.Utils;
using ESLTracker.ViewModels;

namespace ESLTracker
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        new public MainWindowViewModel DataContext
        {
            get
            {
                return (MainWindowViewModel)base.DataContext;
            }
            set
            {
                base.DataContext = value;
            }
        }

        public static bool UpdateOverlay { get; internal set; }

        public MainWindow()
        {
            InitializeComponent();
            UpdateOverlayAsync(this);
            Application.Current.MainWindow = this;
        }

        internal static OverlayToolbar ot { get; set; } = new OverlayToolbar();

        private static async void UpdateOverlayAsync(Window mainWindow)
        {            
            ot.Show();
            UpdateOverlay = true;
            while (UpdateOverlay && ! ot.IsDisposed())
            {
                ot.Visibility = WindowsUtils.IsGameActive() || ot.IsActive || mainWindow.IsActive ? Visibility.Visible : Visibility.Hidden;
               // ot.Topmost = WindowsUtils.IsGameActive();
                await Task.Delay(1000);
            }
            ot.Hide();
            UpdateOverlay = false;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (! ((App)Application.Current).IsApplicationClosing)
            {
                if (Properties.Settings.Default.MinimiseOnClose)
                {
                    this.DataContext.WindowState = WindowState.Minimized;
                    this.DataContext.ShowInTaskBar = false;

                    e.Cancel = true;
                }
                else
                {
                    if (MainWindow.ot.CanClose(this.DataContext.CommandExit))
                    {
                        this.DataContext.Exit(true, Properties.Settings.Default);
                    }
                    else
                    {
                        e.Cancel = true;
                    }
                }
            }
        }

        internal void RestoreOverlay()
        {
            if (!UpdateOverlay)
            {
                UpdateOverlayAsync(this);
            }
        }

        private void mainWindow_StateChanged(object sender, EventArgs e)
        {
            if (this.WindowState == WindowState.Normal)
            {
                this.Activate();
                this.Focus();
            }
        }
    }
}