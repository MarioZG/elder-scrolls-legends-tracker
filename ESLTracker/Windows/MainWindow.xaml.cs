using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using ESLTracker.Utils;
using ESLTracker.Utils.Messages;
using ESLTracker.ViewModels.Windows;

namespace ESLTracker.Windows
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


        public MainWindow(IMessenger messenger)
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