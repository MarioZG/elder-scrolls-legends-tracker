using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
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
using ESLTracker.Controls;
using ESLTracker.DataModel;
using ESLTracker.Properties;
using ESLTracker.Utils;
using ESLTracker.Utils.SimpleInjector;
using ESLTracker.ViewModels.Windows;
using NLog;

namespace ESLTracker.Windows
{
    /// <summary>
    /// Interaction logic for DeckOverlay.xaml
    /// </summary>
    public partial class SessionOverlay : OverlayWindowBase
    {
        Logger Logger = LogManager.GetCurrentClassLogger();

        public override bool ShowOnScreen
        {
            get { return settings.SessionOverlay_ShowOnStart; }
            set
            {
                settings.SessionOverlay_ShowOnStart = value;
                RaisePropertyChangedEvent();
            }
        }

        private readonly ITracker tracker;
        private readonly ISettings settings;

        public SessionOverlay(ITracker tracker, ISettings settings)
        {
            this.tracker = tracker;
            this.settings = settings;

            InitializeComponent();
            this.WindowStartupLocation = WindowStartupLocation.Manual;
            if (this.Left == -1 || this.Top == -1)
            {
                //first run with clear settings
                this.Left = System.Windows.SystemParameters.PrimaryScreenWidth - this.Width;
                this.Top = System.Windows.SystemParameters.PrimaryScreenHeight / 4;
            }
        }

        public override void UpdateVisibilty(
            bool isGameActive, 
            bool isGameProcessRunning, 
            bool isMainWIndowActive, 
            bool isOtherWindowActive)
        {
            Logger.Trace($"UpdateVisibilty check IsDisposed={this.IsDisposed()};isGameActive={isGameActive};isOtherWindowActive={isOtherWindowActive};IsActive={IsActive};");
            this.Visibility = ShowOnScreen 
                                && !this.IsDisposed()
                                && (tracker.ActiveDeck?.SelectedVersion?.Cards?.Count > 0)
                                && (isGameActive || isMainWIndowActive || isOtherWindowActive)
                                ? Visibility.Visible : Visibility.Hidden;
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);

            if (e.ButtonState == MouseButtonState.Pressed)
            {
                // Begin dragging the window
                this.DragMove();
            }
        }

        protected override void OnPreviewMouseWheel(MouseWheelEventArgs args)
        {
            base.OnPreviewMouseWheel(args);
            if (Keyboard.IsKeyDown(Key.LeftCtrl) ||
                Keyboard.IsKeyDown(Key.RightCtrl))
            {

                settings.OverlayDeck_Scale += (args.Delta > 0) ? 0.1 : -0.1;
            }
        }
    }
}
