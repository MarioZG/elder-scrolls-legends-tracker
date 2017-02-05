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
using ESLTracker.Utils;

namespace ESLTracker
{
    /// <summary>
    /// Interaction logic for DeckOverlay.xaml
    /// </summary>
    public partial class DeckOverlay : OverlayWindowBase
    {
        public override bool ShowOnScreen
        {
            get { return Settings.OverlayDeck_ShowOnStart; }
            set
            {
                Settings.OverlayDeck_ShowOnStart = value;
                RaisePropertyChangedEvent();
            }
        }


        public DeckOverlay()
        {
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
            this.Visibility = ShowOnScreen 
                                && !this.IsDisposed()
                                && (TrackerFactory.DefaultTrackerFactory.GetTracker().ActiveDeck?.SelectedVersion?.Cards?.Count > 0)
                                && (isGameActive || isOtherWindowActive || this.IsActive)
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

                TrackerFactory.DefaultTrackerFactory.GetSettings().OverlayDeck_Scale += (args.Delta > 0) ? 0.1 : -0.1;
            }
        }
    }
}
