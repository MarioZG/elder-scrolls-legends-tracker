using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ESLTracker.ViewModels
{
    public class RelayCommand : ICommand
    {
        Action<object> action;
        Func<object, bool> canExecute;

        public RelayCommand(Action<object> action) : this (action, null)
        {
        }

        public RelayCommand(Action<object> action, Func<object, bool> canExecute)
        {
            this.action = action;
            this.canExecute = canExecute;
        }

        #region ICommand Members
        public bool CanExecute(object parameter)
        {
            if (canExecute != null)
            {
                return canExecute(parameter);
            }
            else
            {
                return true;
            }
        }

        public event EventHandler CanExecuteChanged
        { 
            // wire the CanExecutedChanged event only if the canExecute func
            // is defined (that improves perf when canExecute is not used)
            add
            {
                if (this.canExecute != null)
                {
                    CommandManager.RequerySuggested += value;
                }
            }
            remove
            {

                if (this.canExecute != null)
                {
                    CommandManager.RequerySuggested -= value;
                }
            }
        }

        public void Execute(object parameter)
        {
            if (parameter != null)
            {
                action(parameter);
            }
            else
            {
                action(null);
            }
        }

        #endregion
    }
}
