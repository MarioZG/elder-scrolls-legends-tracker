using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ESLTracker.ViewModels
{
    public class RealyAsyncCommand<TResult> : ViewModelBase, IAsyncCommand<TResult>
    {
        //general implemenation
        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        Func<object, bool> _canExecute;
        public bool CanExecute(object parameter)
        {
            return _canExecute != null ? _canExecute(parameter) : true;
        }

        protected void RaiseCanExecuteChanged()
        {
            CommandManager.InvalidateRequerySuggested();
        }

        public async void Execute(object parameter)
        {
            await ExecuteAsync(parameter);
        }

        //Async bits here

        private readonly Func<object,Task<TResult>> _command;

        // Raises PropertyChanged
        private NotifyTaskCompletion<TResult> _execution;
        public NotifyTaskCompletion<TResult> Execution {
            get { return _execution; }
            private set { SetProperty<NotifyTaskCompletion<TResult>>(ref _execution, value); }
        }

        public RealyAsyncCommand(Func<object, Task<TResult>> command)
        {
            _command = command;
        }

        public RealyAsyncCommand(Func<object,Task<TResult>> command, Func<object, bool> canExecute)
        {
            _command = command;
            _canExecute = canExecute;
        }

        public Task ExecuteAsync(object parameter)
        {
            Execution = new NotifyTaskCompletion<TResult>(_command(parameter));
            return Execution.TaskCompletion != null ? Execution.TaskCompletion : Execution.Task;
        }


    }
}
