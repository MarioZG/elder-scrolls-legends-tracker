using System.Windows;
using System.Windows.Input;
using ESLTracker.Controls;
using ESLTracker.DataModel;
using ESLTracker.Properties;
using ESLTracker.Utils;
using ESLTracker.Utils.Extensions;

namespace ESLTracker.Windows
{
    /// <summary>
    /// Interaction logic for DeckOverlay.xaml
    /// </summary>
    public partial class DeckOverlay : OverlayWindowBase
    {
        public override bool ShowOnScreen
        {
            get { return settings.OverlayDeck_ShowOnStart; }
            set
            {
                settings.OverlayDeck_ShowOnStart = value;
                RaisePropertyChangedEvent();
            }
        }

        private readonly ILogger logger;
        private readonly ITracker tracker;
        private readonly ISettings settings;

        public DeckOverlay(ILogger logger, ITracker tracker, ISettings settings)
        {
            this.logger = logger;
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
            logger.Trace($"UpdateVisibilty check IsDisposed={this.IsDisposed()};isGameActive={isGameActive};isOtherWindowActive={isOtherWindowActive};IsActive={IsActive};");
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
