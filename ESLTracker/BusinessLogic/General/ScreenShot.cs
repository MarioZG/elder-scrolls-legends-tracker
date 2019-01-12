using ESLTracker.BusinessLogic.DataFile;
using ESLTracker.Properties;
using ESLTracker.Utils;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ESLTracker.BusinessLogic.General
{
    public class ScreenShot : IScreenShot
    {
        string ScreenShotFolder = "Screenshot";

        private readonly IWinAPI winApi;
        private readonly ISettings settings;
        private readonly PathManager pathManager;
        private readonly OverlayWindowRepository overlayWindowRepository;

        public ScreenShot(
            IWinAPI winApi, 
            ISettings settings, 
            PathManager pathManager,
            OverlayWindowRepository overlayWindowRepository)
        {
            this.winApi = winApi;
            this.settings = settings;
            this.pathManager = pathManager;
            this.overlayWindowRepository = overlayWindowRepository;
        }

        public Task SaveScreenShot(string fileName)
        {
            IntPtr? eslHandle = winApi.GetEslProcess()?.MainWindowHandle;
            if (eslHandle.HasValue)
            {
                WinAPI.SetForegroundWindow(eslHandle.Value);


                var rect = new WinAPI.Rect();
                WinAPI.GetWindowRect(eslHandle.Value, ref rect);

                int dpi = WinAPI.GetDpiForWindow(eslHandle.Value);

                rect = AdjustForMultimonitorDPI(rect, dpi);

                int width = rect.right - rect.left;
                int height = rect.bottom - rect.top;

                var bmp = new Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                Graphics gfxBmp = Graphics.FromImage(bmp);

                List<Window> hiddenWindows = new List<Window>();
                Window activeWindow = null;
                Application.Current.Dispatcher.Invoke(() =>
                {
                    foreach (Window w in overlayWindowRepository)
                    {
                        //System.Diagnostics.Debugger.Log(1, "", "w" + w.Title);
                        //System.Diagnostics.Debugger.Log(1, "", "  w.IsActive=" + w.IsActive);
                        //System.Diagnostics.Debugger.Log(1, "", "   w.IsVisible=" + w.IsVisible);
                        //System.Diagnostics.Debugger.Log(1, "", "   w.Topmost=" + w.Topmost);
                        //System.Diagnostics.Debugger.Log(1, "", Environment.NewLine);
                        if (w.IsActive) //if other if visible - cannot do anything; otherwise if it was in back, it would be show at the top:/...
                        {
                            activeWindow = w;
                            w.Hide();
                            hiddenWindows.Add(w);
                        }
                        else if (w.IsVisible)
                        {
                            w.Hide();
                            hiddenWindows.Add(w);
                        }
                    }
                });
                gfxBmp.CopyFromScreen(
                    rect.left,
                    rect.top,
                    0,
                    0,
                    new System.Drawing.Size(width, height),
                    CopyPixelOperation.SourceCopy);

                foreach (Window w in hiddenWindows)
                {
                    w.Dispatcher.Invoke(() => w.Show());
                }

                activeWindow?.Dispatcher.Invoke(() => activeWindow?.Activate());

                string path = Path.Combine(
                    pathManager.DataPath,
                    ScreenShotFolder,
                    Path.ChangeExtension(fileName, "png")
                    );
                if (!Directory.Exists(Path.GetDirectoryName(path)))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(path));
                }
                bmp.Save(path, System.Drawing.Imaging.ImageFormat.Png);

                gfxBmp.Dispose();

            }

            return Task.FromResult<object>(null);
        }

        private WinAPI.Rect AdjustForMultimonitorDPI(WinAPI.Rect rect, int dpi)
        {
            //https://docs.microsoft.com/en-us/windows/desktop/hidpi/high-dpi-desktop-application-development-on-windows#updating-existing-applications
            // /2 - not sure why - it returns doubled values!
            rect.right = MulDiv(rect.right, dpi, 96) / 2;
            rect.left = MulDiv(rect.left, dpi, 96) / 2;
            rect.bottom = MulDiv(rect.bottom, dpi, 96) / 2;
            rect.top = MulDiv(rect.top, dpi, 96) / 2;
            return rect;
        }

        public int MulDiv(int number, int numerator, int denominator)
        {
            return (int)(((long)number * numerator + (denominator >> 1)) / denominator);
        }
    }
}
