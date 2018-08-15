using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using ESLTracker.BusinessLogic.General;
using ESLTracker.Controls;
using ESLTracker.Properties;
using ESLTracker.Utils;
using ESLTracker.Utils.Extensions;
using ESLTracker.ViewModels;

namespace ESLTracker.Windows
{
    /// <summary>
    /// Interaction logic for OverlayToolbar.xaml
    /// </summary>
    public partial class OverlayToolbar : OverlayWindowBase
    {
        private readonly ILogger logger;
        private readonly ISettings settings;
        private readonly IScreenShot screenShot;
        private readonly ScreenshotNameProvider screenshotNameProvider;

        public override bool ShowOnScreen
        {
            get { return settings.OverlayWindow_ShowOnStart; }
            set
            {
                settings.OverlayWindow_ShowOnStart = value;
                RaisePropertyChangedEvent();
            }
        }

        public OverlayToolbar(
            ILogger logger,
            ISettings settings, 
            IScreenShot screenShot,
            ScreenshotNameProvider screenshotNameProvider)
        {
            this.logger = logger;
            this.settings = settings;
            this.screenShot = screenShot;
            this.screenshotNameProvider = screenshotNameProvider;

            CreateScreenshot = new RelayCommand(CreateScreenshotExecute);

            InitializeComponent();
            this.WindowStartupLocation = WindowStartupLocation.Manual;
            if (this.Left == -1 || this.Top == -1)
            {
                //first run with clear settings
                this.Left = System.Windows.SystemParameters.PrimaryScreenWidth - this.Width;
                this.Top = System.Windows.SystemParameters.PrimaryScreenHeight / 4;
            }
        }

        public override void UpdateVisibilty(bool isGameActive, bool isGameProcessRunning, bool isMainWIndowActive, bool isOtherWindowActive)
        {
            logger.Trace($"UpdateVisibilty check IsDisposed={this.IsDisposed()};isGameActive={isGameActive};isOtherWindowActive={isOtherWindowActive};IsActive={IsActive};");
            this.Visibility = ShowOnScreen && !this.IsDisposed() &&
                                (isGameActive || isMainWIndowActive || isOtherWindowActive)
                                ? Visibility.Visible : Visibility.Hidden;
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);

            // Begin dragging the window
            this.DragMove();
        }

        public ICommand CreateScreenshot { get; private set; }

        private void CreateScreenshotExecute(object obj)
        {
            string fileName = screenshotNameProvider.GetScreenShotName(ScreenshotNameProvider.ScreenShotType.Regular);
            Task.Factory.StartNew(() => screenShot.SaveScreenShot(fileName));
        }

        internal bool CanClose(ICommand commandExit)
        {
            if (this.editGame.DataContext.IsDirty())
            {
                this.editGame.DataContext.ErrorMessage = "There is unsaved game data. Save game before exit!  ";
                this.editGame.DataContext.CommandExecuteWhenContinueOnError = commandExit;
                this.editGameTab.Focus();
                this.ShowOnScreen = true;
                this.Show();
                this.Activate();
                this.Focus();
                return false;
            }
            return true;
        }
    }
}
