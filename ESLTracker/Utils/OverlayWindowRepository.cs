using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using ESLTracker.Controls;
using ESLTracker.Utils.SimpleInjector;
using ESLTracker.Windows;

namespace ESLTracker.Utils
{
    public class OverlayWindowRepository : PropertiesObservableCollection<OverlayWindowBase>
    {

        private bool isInitialased = false;

        public bool UpdateOverlay { get; internal set; }


        public OverlayWindowRepository()
        {
        }

        public void RegisterWindows(IEnumerable<OverlayWindowBase> overlayWindows)
        {
            if(isInitialased)
            {
                throw new InvalidOperationException($"{nameof(OverlayWindowRepository)} was already Initialased ");
            }
            foreach (var ow in overlayWindows)
            {
                Add(ow);
            }
            isInitialased = true;

        }

        internal T GetWindowByType<T>() where T: IOverlayWindow
        {
            return (T)GetWindowByType(typeof(T));
        }

        internal object GetWindowByType(Type obj)
        {
            return this.Where(w => w.GetType() == obj).FirstOrDefault();
        }

        internal bool IsAnyActive()
        {
            return this.Any(ow => ((Window)ow).IsActive);
        }

        public async Task UpdateOverlayAsync(MainWindow mainWindow, IWinAPI winAPI)
        {
            mainWindow.Dispatcher.Invoke(() => {
                foreach (OverlayWindowBase w in this)
                {
                    w.Show();
                }
            });
            UpdateOverlay = true;
            while (UpdateOverlay)
            {
                mainWindow.Dispatcher.Invoke(() =>
                {
                    var win = Application.Current.Windows.OfType<Window>().SingleOrDefault(x => x.IsActive);
                    foreach (IOverlayWindow window in this)
                    {
                        window.UpdateVisibilty(winAPI.IsGameActive(), winAPI.GetEslProcess() != null, mainWindow.IsActive, winAPI.IsTrackerActive());
                    }
                });
                await Task.Delay(1000);
            }
            mainWindow.Dispatcher.Invoke(() => {
                foreach (Window w in this)
                {
                    w.Hide();
                }
            });
            UpdateOverlay = false;
        }
    }
}
