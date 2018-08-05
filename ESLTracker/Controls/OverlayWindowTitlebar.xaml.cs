using ESLTracker.BusinessLogic.General;
using ESLTracker.Utils;
using ESLTracker.Utils.Extensions;
using ESLTracker.Utils.SimpleInjector;
using ESLTracker.Windows;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace ESLTracker.Controls
{
    /// <summary>
    /// Interaction logic for OverlayWindowTitlebar.xaml
    /// </summary>
    public partial class OverlayWindowTitlebar : UserControl
    {



        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Title.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register("Title", typeof(string), typeof(OverlayWindowTitlebar), new PropertyMetadata(String.Empty));



        public IEnumerable<UIElement> CustomButtons
        {
            get { return (IEnumerable<UIElement>)GetValue(CustomButtonsProperty); }
            set { SetValue(CustomButtonsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CustomButtons.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CustomButtonsProperty =
            DependencyProperty.Register("CustomButtons", typeof(IEnumerable<UIElement>), typeof(OverlayWindowTitlebar), new PropertyMetadata(null));

        public OverlayWindowTitlebar()
        {
            InitializeComponent();
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            var window = this.FindParent<Window>();
            ((IOverlayWindow)window).ShowOnScreen = false;
        }

        private double originalHeight;

        private void btnCollapse_Click(object sender, RoutedEventArgs e)
        {
            ToggleButton tb = sender as ToggleButton;
            if (tb != null)
            {
                Window currWindow = Window.GetWindow(this);
                var content = ((StackPanel)Window.GetWindow(this).Content);
                var childElelemt = ((FrameworkElement)((StackPanel)content.Children[1]).Children[0]);
                if (tb.IsChecked.Value)
                {
                    //tab control wont collapse when just changed height of window
                    content.Children[1].Visibility = Visibility.Collapsed;

                    originalHeight = currWindow.ActualHeight;
                    currWindow.Height = ((UserControl)content.Children[0]).ActualHeight;
                    currWindow.ResizeMode = ResizeMode.NoResize;
                }
                else
                {
                    content.Children[1].Visibility = Visibility.Visible;

                    currWindow.Height = originalHeight;
                    currWindow.ResizeMode = ResizeMode.CanResizeWithGrip;

                }
            }
            e.Handled = false;
        }

        private void btnShowMainWindow_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = (MainWindow)Application.Current.MainWindow;
            if (mainWindow.WindowState == WindowState.Minimized)
            {
                mainWindow.WindowState = WindowState.Normal;
            }
            mainWindow.Focus();
        }
    }
}
