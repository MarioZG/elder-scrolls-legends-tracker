using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using ESLTracker.Properties;
using ESLTracker.Utils;

namespace ESLTracker.Controls
{
    abstract public class OverlayWindowBase: Window,  IOverlayWindow, INotifyPropertyChanged
    {

        public abstract bool ShowOnScreen { get; set; }
        public abstract void UpdateVisibilty(bool isGameActive, bool isMainWIndowActive, bool isOtherWindowActive);

        protected ISettings Settings = TrackerFactory.DefaultTrackerFactory.GetSettings();

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
    }
}
