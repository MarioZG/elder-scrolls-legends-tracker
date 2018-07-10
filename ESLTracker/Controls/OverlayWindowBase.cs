using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using ESLTracker.Properties;
using ESLTracker.Services;
using ESLTracker.Utils;
using ESLTracker.Utils.SimpleInjector;

namespace ESLTracker.Controls
{
    abstract public class OverlayWindowBase: Window,  IOverlayWindow, INotifyPropertyChanged
    {

        public abstract bool ShowOnScreen { get; set; }
        public abstract void UpdateVisibilty(bool isGameActive, bool isGameProcessRunning, bool isMainWIndowActive, bool isOtherWindowActive);

        protected ISettings Settings = MasserContainer.Container.GetInstance<ISettings>();

        public event PropertyChangedEventHandler PropertyChanged;

        protected void RaisePropertyChangedEvent([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            handler?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected void ClearPropertyChanged()
        {
            PropertyChanged = null;
        }

        /// <summary>
        /// Ensure overlays not visible in alt_tab screen
        /// https://stackoverflow.com/questions/357076/best-way-to-hide-a-window-from-the-alt-tab-program-switcher/551847#551847
        /// </summary>
        /// <param name="e"></param>
        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
           
            IntPtr hwnd = new WindowInteropHelper(this).Handle;

            int extendedStyle = WinAPI.GetWindowLong(hwnd, WinAPI.GWL_EXSTYLE);
            WinAPI.SetWindowLong(hwnd, WinAPI.GWL_EXSTYLE, extendedStyle | WinAPI.WS_EX_TOOLWINDOW);
        }
    }
}
