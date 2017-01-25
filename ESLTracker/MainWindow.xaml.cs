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
using ESLTracker.Utils.Messages;
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
            Task.Run( () =>  UpdateOverlayAsync(this));
            Application.Current.MainWindow = this;

            TrackerFactory.DefaultTrackerFactory.GetMessanger().Register<ApplicationShowBalloonTip>(this, ShowBaloonRequested);

        }

        private static async Task UpdateOverlayAsync(MainWindow mainWindow)
        {
            IWinAPI winAPI = new WinAPI();
            mainWindow.Dispatcher.Invoke(() => {
                foreach (Window w in mainWindow.DataContext.OverlayWindows)
                {
                    w.Show();
                }
            });
            UpdateOverlay = true;
            while (UpdateOverlay)
            {
                mainWindow.Dispatcher.Invoke(() =>
                {
                    bool isAnyOverlayActive = mainWindow.DataContext.OverlayWindows.IsAnyActive();
                    foreach (IOverlayWindow window in mainWindow.DataContext.OverlayWindows)
                    {
                        ((IOverlayWindow)window).UpdateVisibilty(winAPI.IsGameActive(), winAPI.GetEslProcess() != null, mainWindow.IsActive, isAnyOverlayActive);
                    }
                });
                await Task.Delay(1000);
            }
            mainWindow.Dispatcher.Invoke(() => {
                foreach (Window w in mainWindow.DataContext.OverlayWindows)
                {
                    w.Hide();
                }
            });
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
                    OverlayToolbar ot = this.DataContext.OverlayWindows.GetWindowByType<OverlayToolbar>();
                    if (ot.CanClose(this.DataContext.CommandExit))
                    {
                        this.DataContext.Exit(true);
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
                Task.Run(() => UpdateOverlayAsync(this));
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

        private void ShowBaloonRequested(ApplicationShowBalloonTip baloonRequest)
        {
            this.taskBarIcon.ShowBalloonTip(baloonRequest.Title, baloonRequest.Message, Hardcodet.Wpf.TaskbarNotification.BalloonIcon.None);
        }
    }
}