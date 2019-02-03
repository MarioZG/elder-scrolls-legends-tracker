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

                WinAPI.Rect rect = FindWindowCoordiantes(eslHandle);

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

        private WinAPI.Rect FindWindowCoordiantes(IntPtr? eslHandle)
        {
            var rect = new WinAPI.Rect();
            WinAPI.GetWindowRect(eslHandle.Value, ref rect);

            System.Drawing.Point point = new System.Drawing.Point(rect.left + 1, rect.top + 1);
            IntPtr monitorPtr = WinAPI.MonitorFromPoint(point, 2);

            uint dpix, dpiy;
            WinAPI.GetDpiForMonitor(monitorPtr, WinAPI.DpiType.Effective, out dpix, out dpiy);

            var appDpi = WinAPI.GetDpiForWindow(eslHandle.Value);

            if (appDpi != dpix)
            {
                rect = AdjustForMultimonitorDPI(rect, (int)dpix);
            }
            return rect;
        }

        public WinAPI.Rect AdjustForMultimonitorDPI(WinAPI.Rect rect, int dpi)
        {
            //https://docs.microsoft.com/en-us/windows/desktop/hidpi/high-dpi-desktop-application-development-on-windows#updating-existing-applications
            if(dpi  == 0)
            {
                throw new ArgumentException("dpi cannot be {dpi}");
            }
            var dpiRatio = dpi / 96D;
            rect.right = (int)(rect.right / dpiRatio);
            rect.left = (int)(rect.left / dpiRatio);
            rect.top = (int)(rect.top / dpiRatio);
            rect.bottom = (int)(rect.bottom / dpiRatio);
            return rect;
        }
    }
}
