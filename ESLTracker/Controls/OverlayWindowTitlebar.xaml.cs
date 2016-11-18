using System;
using System.Collections.Generic;
using Drawing=System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ESLTracker.Controls
{
    /// <summary>
    /// Interaction logic for OverlayWindowTitlebar.xaml
    /// </summary>
    public partial class OverlayWindowTitlebar : UserControl
    {

        public OverlayWindowTitlebar()
        {
            InitializeComponent();
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Window.GetWindow(this).Close();
        }

        private void btnCollapse_Click(object sender, RoutedEventArgs e)
        {
            ToggleButton tb = sender as ToggleButton;
            if (tb != null)
            {
                Utils.WindowExtensions.FindVisualChildren<TabControl>(this);
                ((System.Windows.Controls.StackPanel)Window.GetWindow(this).Content).Children[1].Visibility =
                    tb.IsChecked.Value ?  Visibility.Hidden : Visibility.Visible;
                ((System.Windows.Controls.StackPanel)Window.GetWindow(this).Content).Background =
                    tb.IsChecked.Value ? null : SystemColors.ControlBrush;
            }
            e.Handled = false;
        }

        private void btnScreenShot_Click(object sender, RoutedEventArgs e)
        {
            IntPtr? eslHandle = WindowsUtils.GetEslProcess()?.MainWindowHandle;
            if (eslHandle.HasValue)
            {
                var rect = new WindowsUtils.Rect();
                WindowsUtils.GetWindowRect(eslHandle.Value, ref rect);

                int width = rect.right - rect.left;
                int height = rect.bottom - rect.top;

                var bmp = new Drawing.Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                Drawing.Graphics gfxBmp = Drawing.Graphics.FromImage(bmp);

                gfxBmp.CopyFromScreen(rect.left, rect.top, 0, 0, new Drawing.Size(width, height), Drawing.CopyPixelOperation.SourceCopy);
                //IntPtr hdcBitmap = gfxBmp.GetHdc();

                //WindowsUtils.PrintWindow(eslHandle.Value, hdcBitmap, 0);
                bmp.Save("./screenshot"+ DateTime.Now.ToString("yyyyMMddHHmmss") + ".png", System.Drawing.Imaging.ImageFormat.Png);

                //gfxBmp.ReleaseHdc(hdcBitmap);
                gfxBmp.Dispose();

            }
        }
    }
}
