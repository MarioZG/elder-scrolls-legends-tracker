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
using ESLTracker.Utils.SimpleInjector;
using ESLTracker.ViewModels;
using ESLTracker.ViewModels.Windows;
using NLog;

namespace ESLTracker.Windows
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private static Logger Logger = LogManager.GetCurrentClassLogger();

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


        public MainWindow(IMessenger messenger, OverlayWindowRepository overlayWindowRepository)
        {
            InitializeComponent();
            messenger.Register<ApplicationShowBalloonTip>(this, ShowBaloonRequested);
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