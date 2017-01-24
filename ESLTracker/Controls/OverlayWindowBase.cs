using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using ESLTracker.Utils;

namespace ESLTracker.Controls
{
    abstract public class OverlayWindowBase: Window,  IOverlayWindow, INotifyPropertyChanged
    {
        protected bool showOnScreen = true;
        public bool ShowOnScreen
        {
            get { return showOnScreen; }
            set { showOnScreen = value; RaisePropertyChangedEvent(); }
        }

        public void UpdateVisibilty(bool isGameActive, bool isMainWIndowActive, bool isOtherWindowActive)
        {
            this.Visibility = ShowOnScreen && !this.IsDisposed() &&
                                (isGameActive || isMainWIndowActive || isOtherWindowActive)
                                ? Visibility.Visible : Visibility.Hidden;
        }

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
