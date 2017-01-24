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
        public DeckOverlay()
        {
            InitializeComponent();
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
                contentScale.ScaleX += (args.Delta > 0) ? 0.1 : -0.1;
                contentScale.ScaleY += (args.Delta > 0) ? 0.1 : -0.1;
            }
        }
    }
}
