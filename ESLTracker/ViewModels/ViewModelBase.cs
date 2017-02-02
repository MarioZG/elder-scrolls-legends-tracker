using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ESLTracker.ViewModels
{
    public class ViewModelBase : INotifyPropertyChanged
    {
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

        protected object GetPropertyValue(object obj, string propertyName)
        {
            object ret = obj;
            foreach (string prop in propertyName.Split(new char[] { '.' }))
            {
                ret = ret.GetType().GetProperty(prop).GetValue(ret, null);
            }
            return ret;
        }

        protected bool SetProperty<T>(
           ref T backingStore,
           T value,
           [CallerMemberName]string propertyName = "",
           Action onChanged = null)
        {
            if (EqualityComparer<T>.Default.Equals(backingStore, value))
                return false;

            backingStore = value;
            onChanged?.Invoke();
            RaisePropertyChangedEvent(propertyName);
            return true;
        }

    }
}
