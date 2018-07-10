using ESLTracker.BusinessLogic.DataFile;
using ESLTracker.Properties;
using ESLTracker.Services;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ESLTracker.BusinessLogic.General
{
    public class ScreenShot : IScreenShot
    {
        string ScreenShotFolder = "Screenshot";

        IWinAPI winApi;
        ISettings settings;
        PathManager pathManager;

        public ScreenShot(IWinAPI winApi, ISettings settings, PathManager pathManager)
        {
            this.winApi = winApi;
            this.settings = settings;
            this.pathManager = pathManager;
        }

        public Task SaveScreenShot(string fileName)
        {
            IntPtr? eslHandle = winApi.GetEslProcess()?.MainWindowHandle;
            if (eslHandle.HasValue)
            {
                var rect = new WinAPI.Rect();
                WinAPI.GetWindowRect(eslHandle.Value, ref rect);

                int width = rect.right - rect.left;
                int height = rect.bottom - rect.top;

                var bmp = new Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                Graphics gfxBmp = Graphics.FromImage(bmp);

                List<Window> hiddenWindows = new List<Window>();
                Application.Current.Dispatcher.Invoke(() =>
                {
                    foreach (Window w in Application.Current.Windows)
                    {
                        // System.Diagnostics.Debugger.Log(1, "", "w"+ w.Title);
                        // System.Diagnostics.Debugger.Log(1, "", "  w.IsActive" + w.IsActive);
                        // System.Diagnostics.Debugger.Log(1, "", "   w.Topmost" + w.Topmost);
                        // System.Diagnostics.Debugger.Log(1, "", Environment.NewLine) ;
                        if (w.IsActive) //if other if visible - cannot do anything; otherwise if it was in back, it would be show at the top:/...
                        {
                            w.Hide();
                            hiddenWindows.Add(w);
                        }
                    }
                });
                WinAPI.SetForegroundWindow(eslHandle.Value);
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
    }
}
